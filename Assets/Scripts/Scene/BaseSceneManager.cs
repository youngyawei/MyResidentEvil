using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class BaseSceneManager : MonoBehaviour {

        // 进入场景后进行简单的初始化 , 设置玩家的位置朝向 , 以及卸载该 scene 的 assetbundle
        protected virtual void Init() {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Archive.CurrentArchive.Player.Position;
            player.transform.eulerAngles = new Vector3(0, Archive.CurrentArchive.Player.Rotate, 0);
            StartCoroutine(UnloadSceneAssetBundle());
        }

        private IEnumerator UnloadSceneAssetBundle() {
            yield return new WaitForSeconds(2.0f);
            Scene s = Scene.GetScene(Archive.CurrentArchive.SceneId);
            string assetBundleName = s.AssetBundle;
            if (s.PowerSource) {
                assetBundleName += Archive.CurrentArchive.PowerSource ? "_light" : "_dark";
            }
            AssetBundleManager.Instance.UnLoadAssetBundle(assetBundleName);
            System.GC.Collect();
        }

    }

}
