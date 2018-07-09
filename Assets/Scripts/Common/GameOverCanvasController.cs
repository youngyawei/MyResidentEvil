using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyResidentEvil {

    public class GameOverCanvasController : MonoBehaviour {

        public Image background;

        public GameObject failPanel;

        public GameObject successPanel;

        private bool finish = false;

        void Update () {
            if(!finish) {
                background.color = Color.Lerp(background.color, Color.black, 0.01f);
                if (background.color.a >= 0.99f) {
                    finish = true;
                    if (GameManager.Instance.CurrentGameState == GameManager.GameState.Fail) {
                        failPanel.SetActive(true);
                    } else if (GameManager.Instance.CurrentGameState == GameManager.GameState.Success) {
                        successPanel.SetActive(true);
                        Instantiate(Resources.Load<GameObject>("UI/EventSystem"));
                    }
                    GameManager.Instance.Pause();
                }
            }
	    }

        public void ReStartGame() {
            GameManager.Instance.ReStartGame();
        }

        public void ExitGame() {
            GameManager.Instance.ExitToStart();
        }

        public void FinishGame() {
            GameManager.Instance.SaveAndExit();
        }

    }

}
