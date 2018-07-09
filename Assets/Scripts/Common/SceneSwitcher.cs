using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class SceneSwitcher : MonoBehaviour {

        public enum SwitcherCondition {
            None,                   // 无条件
            PowerSource,            // 需要开启电源
            KeySilver,              // 需要持有银色钥匙
            KeyGold,                // 需要持有金色钥匙
            UnLock                  // 需要电脑解锁
        }

        public SwitcherCondition condition = SwitcherCondition.None;

        private GameObject player;

        // 需要加载的目标场景
        public string sceneId;

        // 过度场景
        public string interimScene = "OpenDoor";

        // 目标场景中玩家的位置
        public Vector3 position;

        // 目标场景中玩家的转向
        public float rotate;

        // 
        public float distance = 0.5f;

        public float angle = 30.0f;

        private bool switching = false;

	    void Start () {
            player = GameObject.FindGameObjectWithTag("Player");
	    }
	
	    void Update () {
            if (switching) return;
            if (Vector3.Distance(player.transform.position, transform.position) > distance) {
                return;
            }
            float angle0 = Vector3.Angle(transform.forward, player.transform.forward);
            if (angle0 > angle || angle0 < -angle) {
                return;
            }
            if (CrossPlatformInputManager.GetButton("J")) {
                if (!CanSwitch()) {
                    return;
                }

                switching = true;
                Archive.CurrentArchive.SceneId = sceneId;
                Archive.CurrentArchive.Player.Position = position;
                Archive.CurrentArchive.Player.Rotate = rotate;
                Archive.CurrentArchive.Persistence();               // 保存数据

                Instantiate(Resources.Load("UI/FadeinMaskCanvas"));
                GameManager.Instance.Pause();

                // 加载过渡场景
                Scene scene = Scene.GetScene(interimScene);
                AssetBundleManager.Instance.LoadAssetBundleAsnyc(scene.AssetBundle, () => {
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene.SceneId, UnityEngine.SceneManagement.LoadSceneMode.Single);
                });

            }
	    }

        private bool CanSwitch() {
            switch (condition) {
                case SwitcherCondition.None:
                    return true;
                case SwitcherCondition.PowerSource:
                    if (Archive.CurrentArchive.PowerSource) {
                        return true;
                    } else {
                        GameManager.Instance.ShowTips("门被锁住了，必须通电后才能打开");
                        return false;
                    }
                case SwitcherCondition.KeySilver:
                    if (Archive.CurrentArchive.Player.PlayerItems.ContainsKey("Key_Silver")) {
                        return true;
                    } else {
                        GameManager.Instance.ShowTips("门被锁住了，需要银色钥匙才能打开");
                        return false;
                    }
                case SwitcherCondition.KeyGold:
                    if (Archive.CurrentArchive.Player.PlayerItems.ContainsKey("Key_Gold")) {
                        return true;
                    } else {
                        GameManager.Instance.ShowTips("门被锁住了，需要金色钥匙才能打开");
                        return false;
                    }
                case SwitcherCondition.UnLock:
                    if (Archive.CurrentArchive.Unlock) {
                        return true;
                    } else {
                        GameManager.Instance.ShowTips("这道门似乎是前厅的电脑控制的");
                        return false;
                    }
            }
            return false;
        }
        
    }

}
