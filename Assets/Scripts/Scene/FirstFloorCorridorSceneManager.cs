using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class FirstFloorCorridorSceneManager : BaseSceneManager {

        void Start () {
            Init();
            if (Archive.CurrentArchive.PowerSource) {
                if (!Archive.CurrentArchive.CurrentSceneData.Entry) {
                    Archive.CurrentArchive.CurrentSceneData.Entry = true;
                    StartCoroutine(ShowTips());
                }
            }
	    }

        private IEnumerator ShowTips() {
            yield return new WaitForSeconds(2.0f);
            GameManager.Instance.ShowTips("电源打开后视野变得更好了，但是这些丧尸感知的范围似乎也变大了");
        }

    }

}
