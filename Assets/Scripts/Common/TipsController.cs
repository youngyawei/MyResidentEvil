using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyResidentEvil {

    public class TipsController : MonoBehaviour {

        public GameObject tipsCanvas;

        public Text content;

        private Action callOnClose;

        void Start() {
            tipsCanvas.SetActive(false);
        }

        public void ShowTips(string tips, Action callOnClose = null) {
            tipsCanvas.SetActive(true);
            content.text = tips;
            this.callOnClose = callOnClose;
            GameManager.Instance.Pause();
        }

        public void HideTips() {
            GameManager.Instance.Resume();
            tipsCanvas.SetActive(false);
            if (callOnClose != null) {
                callOnClose();
            }
        }
        
    }

}

