using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyResidentEvil.Data;

namespace MyResidentEvil {

    // 刚进入 StartGame 场景
    public class StartGameEntryManager : MonoBehaviour {

        public GameObject entryPanel;

        public GameObject startPanel;

        public GameObject aboutPanel;

        public GameObject mainCamera;

        public GameObject exitButton;

        private AudioSource audioSource;

        void Start() {
            audioSource = mainCamera.GetComponent<AudioSource>();
            AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, "The Beginning Of Story.mp3");
            try {
                Repository.Init();
            } catch(Exception e) {
                GameManager.Instance.ShowTips(e.Message);
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            exitButton.SetActive(true);
#endif
        }

        public void ShowStartPanel() {
            entryPanel.SetActive(false);
            startPanel.SetActive(true);
        }
	
        public void BackToEntryPanel() {
            entryPanel.SetActive(true);
            startPanel.SetActive(false);
        }

        public void ShowAboutPanel() {
            aboutPanel.SetActive(true);
        }

        public void CloseAboutPanel() {
            aboutPanel.SetActive(false);
        }

        public void ExitGame() {
            Application.Quit();
        }

    }

}
