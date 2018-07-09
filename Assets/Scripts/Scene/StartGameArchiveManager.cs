using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class StartGameArchiveManager : MonoBehaviour {

        public GameObject content;

        private ArchiveController[] archives;

        private ArchiveController archiveController;

        private GameObject archivePrefab;

        private bool init = false;

        void OnEnable() {
            if (archivePrefab == null) {
                archivePrefab = Resources.Load<GameObject>("UI/Archive");
            }
        }

        public void SetArchives(List<Archive> archiveList) {
            if (init) return;
            archives = new ArchiveController[archiveList.Count];
            GameObject a;
            for (int i = 0; i < archiveList.Count; i++) {
                a = Instantiate(archivePrefab, content.transform);
                archives[i] = a.GetComponent<ArchiveController>();
                archives[i].Init(archiveList[i], this);
            }
            init = true;
        }

        public void SelectArchive(ArchiveController ac) {
            UnSelectAll();
            archiveController = ac;
            ac.Select();
        }

        private void UnSelectAll() {
            foreach (ArchiveController ac in archives) {
                ac.UnSelect();
            }
        }

        public void ReturnToStarGamePanel() {
            gameObject.SetActive(false);
        }

        // 载入存档 , 开始游戏
        public void LoadArchive() {
            if (archiveController != null) {
                GameManager.Instance.StartGame(archiveController.Archive);
            }
        }

        void OnDisable() {
            archiveController = null;
            UnSelectAll();
        }

    }

}

