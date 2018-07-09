using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    public class ConfirmWindowController : MonoBehaviour {

        public GameObject confirmCanvas;

        private Action confirmAction;

        private Action cancelAction;
        
	    public void ShowConfirmWindow(Action confirm, Action cancel) {
            confirmCanvas.SetActive(true);
            confirmAction = confirm;
            cancelAction = cancel;
            GameManager.Instance.Pause();
        }

        public void Confirm() {
            GameManager.Instance.Resume();
            confirmAction();
            confirmCanvas.SetActive(false);
        }

        public void Cancel() {
            GameManager.Instance.Resume();
            confirmCanvas.SetActive(false);
            if (cancelAction != null) {
                cancelAction();
            }
        }


    }

}
