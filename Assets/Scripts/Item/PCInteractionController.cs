using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class PCInteractionController : InteractionItemController {

        public GameObject canvas;

        public GameObject inputField;

        public GameObject tips;

        public Text tipsText;

        private InputField input;

        public string password = "3154";

        private bool flag = false;

        private bool tipsFlag = false;

        private float countTime = 0;

        void Start() {
            player = GameObject.FindGameObjectWithTag("Player");
            input = inputField.GetComponent<InputField>();
        }

	    void Update () {
            if (flag) {
                if (tipsFlag) {
                    countTime += Time.unscaledDeltaTime;
                    if (countTime >= 2.0f) {
                        HideTips();
                        countTime = 0;
                    }
                }
                return;
            }
            if (CanInteraction()) {
                Interaction();
            }
	    }

        public void Confirm() {
            tips.SetActive(true);
            tipsFlag = true;
            if (input.text == password) {
                tipsText.text = "UNLOCK SUCCESS";
                tipsText.color = Color.green;
                Archive.CurrentArchive.Unlock = true;
            } else {
                tipsText.text = "UNLOCK FAIL";
                tipsText.color = Color.red;
            }
        }

        private void HideTips() {
            tips.SetActive(false);
            tipsFlag = false;
            if (Archive.CurrentArchive.Unlock) {
                canvas.SetActive(false);
                flag = false;
                GameManager.Instance.Resume();
            }
        }

        public void Cancel() {
            canvas.SetActive(false);
            flag = false;
            GameManager.Instance.Resume();
        }

        protected override void Interaction() {
            if (!Archive.CurrentArchive.PowerSource) {
                GameManager.Instance.ShowTips("需要开启电源");
                return;
            }
            if (Archive.CurrentArchive.Unlock) {
                GameManager.Instance.ShowTips("已经解锁了");
                return;
            }
            canvas.SetActive(true);
            flag = true;
            GameManager.Instance.Pause();
        }

    }

}
