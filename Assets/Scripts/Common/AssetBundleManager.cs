using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ICSharpCode.SharpZipLib.Zip;

namespace MyResidentEvil {

    public class AssetBundleManager : MonoBehaviour {

        private static AssetBundleManager instance;

        // 标识资源是否已经初始化了
        private static bool init = false;

        private bool isShowProgress = false;

        public static AssetBundleManager Instance {
            get { return instance; }
        }

        private static Dictionary<string, AssetBundleMap> assetBundleMaps = new Dictionary<string, AssetBundleMap>();

        private class AssetBundleMap {
            public string assetBundleName;
            public AssetBundle assetBundle;
            public string[] dependencies;
            public int count;

            public bool Unload() {
                count--;
                if (count == 0) {
                    assetBundle.Unload(false);
                    return true;
                }
                return false;
            }

            public override bool Equals(object obj) {
                if (!(obj is global::MyResidentEvil.AssetBundleManager.AssetBundleMap)) {
                    return false;
                }
                var obj1 = (AssetBundleMap)obj;
                return assetBundleName == obj1.assetBundleName;
            }

            public override int GetHashCode() {
                var hashCode = -143168864;
                hashCode = hashCode * -1521134295 + base.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(assetBundleName);
                return hashCode;
            }

        }

        private string pathPrefix;

        private AssetBundleManifest manifest;

        private GameObject assetBundleInitCanvas;

        private AssetBundleInitCanvasController controller;

        void Start () {
#if UNITY_EDITOR 
            // 如果使用 loadFromFile 就不需要使用 file:// 
            pathPrefix = Application.dataPath + "/AssetBundles/";
#elif UNITY_ANDROID
            //pathPrefix = Application.persistentDataPath + "/AssetBundles/";
            pathPrefix = Application.streamingAssetsPath + "/AssetBundles/";
#endif
            instance = this;
            LoadManifest();
            /*
#if UNITY_EDITOR
            LoadManifest();
#elif UNITY_ANDROID
            if(init){
                LoadManifest();
            } else {
                assetBundleInitCanvas = Instantiate(Resources.Load<GameObject>("UI/AssetBundleInitCanvas"));
                controller = assetBundleInitCanvas.GetComponent<AssetBundleInitCanvasController>();
                StartCoroutine(InitAssetBundles());
            }
#endif
            */
        }

        // 发现放到外面也没有什么性能提升 , 反而使用 ICSharpCode.SharpZipLib 还产生了内存泄漏
        private IEnumerator InitAssetBundles() {
            // 判断是否需要更新
            controller.UpdateInfo("检查资源文件中......");
            string txt = null;
            using (WWW www = new WWW(Application.streamingAssetsPath + "/AssetBundle.txt")) {
                yield return www;
                using (StreamReader sr = new StreamReader(new MemoryStream(www.bytes))) {
                    txt = sr.ReadLine();
                }
            }
            // 先检查标志文件是否存在
            if (File.Exists(Application.persistentDataPath + "/AssetBundle.txt")) {
                using (StreamReader sr = File.OpenText(Application.persistentDataPath + "/AssetBundle.txt")) {
                    string str = sr.ReadLine();
                    if (str != txt) {
                        File.Delete(Application.persistentDataPath + "/AssetBundle.txt");
                        using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "/AssetBundle.txt")) {
                            sw.Write(txt.ToCharArray());
                            sw.Flush();
                        }
                    } else {
                        init = true;
                        LoadManifest();
                        Destroy(assetBundleInitCanvas);
                        yield break;
                    }
                }
            } else {
                // 不存在则创建标志文件
                using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "/AssetBundle.txt")) {
                    sw.Write(txt.ToCharArray());
                    sw.Flush();
                }
            }

            controller.UpdateInfo("正在加载资源压缩包......");
            using (MemoryStream ms = new MemoryStream()) {
                using (WWW www = new WWW(Application.streamingAssetsPath + "/AssetBundles.zip")) {
                    yield return www;
                    ms.Write(www.bytes, 0, www.bytesDownloaded);
                    ms.Seek(0L, SeekOrigin.Begin);
                    controller.InitSlider(www.bytesDownloaded);
                }
                controller.UpdateInfo("资源解压中......");
                using (ZipInputStream zipInput = new ZipInputStream(ms)) {
                    ZipEntry zipEntry = null;
                    string fileName = "";
                    byte[] datas = new byte[4 * 1024];
                    while ((zipEntry = zipInput.GetNextEntry()) != null) {
                        if (zipEntry.Name.EndsWith(".meta") || zipEntry.Name.EndsWith(".manifest")) {
                            zipInput.CloseEntry();
                            continue;
                        }
                        fileName = Application.persistentDataPath + "/" + zipEntry.Name;
                        if (zipEntry.IsFile) {
                            if (File.Exists(fileName)) {
                                File.Delete(fileName);          // 如果存在则先删除 , 然后创建一个新的
                            }
                            using (FileStream fs = File.Create(fileName)) {
                                int count = 0;
                                while ((count = zipInput.Read(datas, 0, datas.Length)) > 0) {
                                    fs.Write(datas, 0, count);
                                }
                                fs.Flush();
                            }
                            controller.UpdateProgress((int)zipEntry.Size);      // 更新进度条
                        } else {
                            if(!Directory.Exists(fileName)) Directory.CreateDirectory(fileName);
                        }
                        zipInput.CloseEntry();
                        yield return null;
                    }
                }
            }
            LoadManifest();
            init = true;
            Destroy(assetBundleInitCanvas);
        }

        // 载入总的 manifest 文件
        private void LoadManifest() {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(pathPrefix + "AssetBundles");
            manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            assetBundle.Unload(false);
        }

        public void LoadAssetBundle(string assetBundleName) {
            if (assetBundleMaps.ContainsKey(assetBundleName)) {
                AssetBundleMap map = assetBundleMaps[assetBundleName];
                map.count++;
                foreach (string s in map.dependencies) {
                    if (assetBundleMaps.ContainsKey(s)) {
                        AssetBundleMap map0 = assetBundleMaps[s];
                        map0.count++;
                    }
                }
                return;
            }
            string[] dependencies = manifest.GetAllDependencies(assetBundleName);
            for(int i = 0; i < dependencies.Length; i++) {
                if (assetBundleMaps.ContainsKey(dependencies[i])) {
                    AssetBundleMap map = assetBundleMaps[dependencies[i]];
                    map.count++;
                    continue;
                }
                assetBundleMaps.Add(dependencies[i], new AssetBundleMap {
                    assetBundleName = dependencies[i],
                    assetBundle = AssetBundle.LoadFromFile(pathPrefix + dependencies[i]),
                    dependencies = manifest.GetAllDependencies(dependencies[i]),
                    count = 1
                });
            }
            AssetBundle assetBundle = AssetBundle.LoadFromFile(pathPrefix + assetBundleName);

            assetBundleMaps.Add(assetBundleName, new AssetBundleMap {
                assetBundleName = assetBundleName,
                assetBundle = assetBundle,
                dependencies = dependencies,
                count = 1
            });
        }

        // 在加载 assetbundle 时显示加载的进度 , 这个只能在异步加载的时候才能使用
        public void ShowProgress() {
            if (!isShowProgress) {
                assetBundleInitCanvas = Instantiate(Resources.Load<GameObject>("UI/AssetBundleInitCanvas"));
                controller = assetBundleInitCanvas.GetComponent<AssetBundleInitCanvasController>();
                isShowProgress = true;
            }
        }

        public void HideProgress() {
            if(isShowProgress) {
                controller = null;
                Destroy(assetBundleInitCanvas);
                assetBundleInitCanvas = null;
                isShowProgress = false;
            }
        }

        public void LoadAssetBundleAsnyc(string assetBundleName, Action callback) {
            StartCoroutine(LoadAssetBundle(assetBundleName, callback));
        }

        private IEnumerator LoadAssetBundle(string assetBundleName, Action callback) {
            if (assetBundleMaps.ContainsKey(assetBundleName)) {
                AssetBundleMap map = assetBundleMaps[assetBundleName];
                map.count++;
                foreach (string s in map.dependencies) {
                    if (assetBundleMaps.ContainsKey(s)) {
                        AssetBundleMap map0 = assetBundleMaps[s];
                        map0.count++;
                    }
                }
                callback();
                yield break;
            }
            string[] dependencies = manifest.GetAllDependencies(assetBundleName);
            if (isShowProgress && controller != null) {
                // 如果要显示进度 , 则对进度条进行初始化
                controller.InitSlider(dependencies.Length + 1);
                controller.UpdateInfo("加载依赖资源......");
            }
            for (int i = 0; i < dependencies.Length; i++) {
                if (assetBundleMaps.ContainsKey(dependencies[i])) {
                    AssetBundleMap map = assetBundleMaps[dependencies[i]];
                    map.count++;
                } else {
                    AssetBundleCreateRequest abcr0 = AssetBundle.LoadFromFileAsync(pathPrefix + dependencies[i]);
                    yield return abcr0;
                    assetBundleMaps.Add(dependencies[i], new AssetBundleMap {
                        assetBundleName = dependencies[i],
                        assetBundle = abcr0.assetBundle,
                        dependencies = manifest.GetAllDependencies(dependencies[i]),
                        count = 1
                    });
                }
                if (isShowProgress && controller != null) {
                    // 更新加载进度
                    controller.UpdateProgress(1);
                }
            }
            if (isShowProgress && controller != null) {
                controller.UpdateInfo("依赖资源加载完成");
            }
            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(pathPrefix + assetBundleName);
            yield return abcr;
            assetBundleMaps.Add(assetBundleName, new AssetBundleMap {
                assetBundleName = assetBundleName,
                assetBundle = abcr.assetBundle,
                dependencies = dependencies,
                count = 1
            });
            if (isShowProgress && controller != null) {
                // 更新加载进度
                controller.UpdateProgress(1);
                controller.UpdateInfo("场景实例化中......");
            }
            callback();
        }

        public object LoadAssetByType(string assetBundleName, string assetName, Type t) {
            if (!assetBundleMaps.ContainsKey(assetBundleName)) return null;
            AssetBundleMap map = assetBundleMaps[assetBundleName];
            return map.assetBundle.LoadAsset(assetName, t);
        }

        public GameObject LoadAsset(string assetBundleName, string assetName) {
            if (!assetBundleMaps.ContainsKey(assetBundleName)) return null;
            AssetBundleMap map = assetBundleMaps[assetBundleName];
            return map.assetBundle.LoadAsset<GameObject>(assetName);
        }

        public void LoadAssetAsync(string assetBundleName, string assetName, Action<GameObject> callback) {
            StartCoroutine(LoadAsset(assetBundleName, assetName, callback));
        }

        private IEnumerator LoadAsset(string assetBundleName, string assetName, Action<GameObject> callback) {
            if (!assetBundleMaps.ContainsKey(assetBundleName)) yield break;
            AssetBundleMap map = assetBundleMaps[assetBundleName];
            AssetBundleRequest abq = map.assetBundle.LoadAssetAsync<GameObject>(assetName);
            yield return abq;
            callback(abq.asset as GameObject);
            UnLoadAssetBundle(assetBundleName);
        }

        public void UnLoadAssetBundle(string assetBundleName) {
            if (!assetBundleMaps.ContainsKey(assetBundleName)) return;
            AssetBundleMap map = assetBundleMaps[assetBundleName];
            if (map.Unload()) {
                assetBundleMaps.Remove(assetBundleName);
            }
            // 尝试卸载依赖
            foreach (string a in map.dependencies) {
                UnLoadAssetBundle(a);
            }
        }

        void OnDestroy() {
            instance = null;
        }
        
    }

}
