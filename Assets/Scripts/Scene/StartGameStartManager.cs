using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    // 打开了 StartPanel 后
    public class StartGameStartManager : MonoBehaviour {

        private List<Archive> archives;

        public GameObject archivePanel;

        private StartGameArchiveManager archiveManager;

        public GameObject loadArchiveButton;
	    
	    void Start () {
            archiveManager = archivePanel.GetComponent<StartGameArchiveManager>();
            loadArchiveButton.SetActive(false);
            try {
                archives = Archive.GetAllArchiveSync();
                if (archives.Count > 0) {
                    loadArchiveButton.SetActive(true);
                }
            } catch (System.Exception e) {
                GameManager.Instance.ShowTips(e.Message);
            }
        }

        public void ShowArchivePanel() {
            archivePanel.SetActive(true);
            archiveManager.SetArchives(archives);
        }

        // 新建一个存档并开始游戏
        public void NewGame() {
            GameManager.Instance.StartNewGame();
        }

    }

}

