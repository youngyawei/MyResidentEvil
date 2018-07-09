using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class PowerSourceRoomSceneManager : BaseSceneManager {

        void Start() {
            Init();
            if (!Archive.CurrentArchive.CurrentSceneData.Entry) {
                Archive.CurrentArchive.CurrentSceneData.Entry = true;
                StartCoroutine(ShowTips());
            }
        }

        private IEnumerator ShowTips() {
            yield return new WaitForSeconds(2.0f);
            GameManager.Instance.ShowTips("这估计就是电源控制室了吧，看来电源只是被关闭了");
        }


    }

}
