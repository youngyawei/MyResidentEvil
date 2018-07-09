using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class CasinoRoomSceneManager : BaseSceneManager {

	    void Start () {
            Init();
            if (!Archive.CurrentArchive.CurrentSceneData.Entry) {
                StartCoroutine(ShowTips());
            }
        }

        private IEnumerator ShowTips() {
            yield return new WaitForSeconds(2.0f);
            Archive.CurrentArchive.CurrentSceneData.Entry = true;
            GameManager.Instance.ShowTips("警局里居然会有这样的一个房间，真是腐败啊");
        }

    }

}
