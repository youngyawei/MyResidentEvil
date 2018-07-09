using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {
    
    // 用于在过度场景中加载游戏场景
    public class TransitionSceneManager : MonoBehaviour {

	    void Start () {
            Archive a = Archive.CurrentArchive;
            Scene s = Scene.GetScene(a.SceneId);
            if (a.SceneId != a.CurrentSceneData.SceneId) {
                // 加载下一个场景的数据
                a.CurrentSceneData = SceneData.LoadSceneData(a.ArchiveId, a.SceneId);
            }
            string assetBundle = s.AssetBundle, sceneName = s.SceneId;
            if (s.PowerSource) {
                if (Archive.CurrentArchive.PowerSource) {
                    assetBundle += "_light";
                    sceneName += "_Light";
                } else {
                    assetBundle += "_dark";
                    sceneName += "_Dark";
                }
            }
            AssetBundleManager.Instance.ShowProgress();
            AssetBundleManager.Instance.LoadAssetBundleAsnyc(assetBundle, () => {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            });
            System.GC.Collect();
        }

    }

}
