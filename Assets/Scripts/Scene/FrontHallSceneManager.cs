using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class FrontHallSceneManager : BaseSceneManager {

        void Start() {
            Init();
            if (!Archive.CurrentArchive.CurrentSceneData.Entry) {
                StartCoroutine(ShowTips());
            }
        }

        private IEnumerator ShowTips() {
            yield return new WaitForSeconds(2.0f);
            Archive.CurrentArchive.CurrentSceneData.Entry = true;
            GameManager.Instance.ShowTips("大门打不开了，不过我本来也就没有退路了\n得赶在核弹到之前找到直升机逃离才行", () => {
                GameManager.Instance.ShowTips("不过这样漆黑一片的确实不好行动，视野太差了\n电源是出了故障吗，还是被关闭了", () => {
                    GameManager.Instance.ShowTips("提示：开门的时候请尽量的正对着门");
                });
            });
        }

    }

}
