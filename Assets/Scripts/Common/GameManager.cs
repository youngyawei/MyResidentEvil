using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyResidentEvil.Data;

namespace MyResidentEvil {

    // 游戏管理器
    public class GameManager : MonoBehaviour {

        private static GameManager gameManager;

        public static GameManager Instance {
            get { return gameManager; }
        }

        public enum GameState {Start, Play, Pause,  Success, Fail}

        private GameState currentState = GameState.Start;

        public GameState CurrentGameState {
            get { return currentState; }
        }

        private GameObject tips;

        private TipsController tipsController;

        private GameObject confirmWindow;

        private ConfirmWindowController confirmWindowController;

        void Start () {
            gameManager = this;
            GameObject tipsPrefab = Resources.Load<GameObject>("UI/Tips");
            tips = Instantiate(tipsPrefab);
            tipsController = tips.GetComponent<TipsController>();
            GameObject cfwp = Resources.Load<GameObject>("UI/ConfirmWindow");
            confirmWindow = Instantiate(cfwp);
            confirmWindowController = confirmWindow.GetComponent<ConfirmWindowController>();
            Instantiate(Resources.Load<GameObject>("UI/FadeoutMaskFCanvas"));
            StartCoroutine(SwitchToPlay());
        }

        private IEnumerator SwitchToPlay() {
            yield return new WaitForSeconds(1.5f);
            if (currentState == GameState.Start) {
                currentState = GameState.Play;
            }
        }

        // 暂停游戏
        public void Pause() {
            currentState = GameState.Pause;
            Time.timeScale = 0;
        }

        // 恢复游戏
        public void Resume() {
            currentState = GameState.Play;
            Time.timeScale = 1;
        }

        public void StartNewGame() {
            Instantiate(Resources.Load("UI/FadeinMaskCanvas"));
            Archive archive = Archive.CreateNewArchive();
            Archive.CurrentArchive = archive;
            AssetBundleManager.Instance.LoadAssetBundleAsnyc("scene/open_door", () => {
                SceneManager.LoadSceneAsync("OpenDoor", LoadSceneMode.Single);
            });
        }

        public void StartGame(Archive archive) {
            Instantiate(Resources.Load("UI/FadeinMaskCanvas"));
            Archive.CurrentArchive = archive;
            Archive.LoadArchiveData(Archive.CurrentArchive);
            AssetBundleManager.Instance.LoadAssetBundleAsnyc("scene/open_door", () => {
                SceneManager.LoadSceneAsync("OpenDoor", LoadSceneMode.Single);
            });
        }

        // 不保存数据直接重新加载
        public void ReStartGame() {
            Instantiate(Resources.Load("UI/FadeinMaskCanvas"));
            Archive.LoadArchiveData(Archive.CurrentArchive);
            AssetBundleManager.Instance.LoadAssetBundleAsnyc("scene/open_door", () => {
                SceneManager.LoadSceneAsync("OpenDoor", LoadSceneMode.Single);
            });
        }

        // 保存并退出
        public void SaveAndExit() {
            Archive.CurrentArchive.Persistence();
            SceneManager.LoadScene("StartGame", LoadSceneMode.Single);
        }

        // 退出到开始游戏场景
        public void ExitToStart() {
            SceneManager.LoadScene("StartGame", LoadSceneMode.Single);
        }

        public void GameSuccess() {
            currentState = GameState.Success;
            Archive.CurrentArchive.Finish = true;
            Instantiate(Resources.Load<GameObject>("UI/GameOverCanvas"));
        }

        public void GameFail() {
            currentState = GameState.Fail;
            Instantiate(Resources.Load<GameObject>("UI/GameOverCanvas"));
        }

        public void ShowTips(string tips, Action callOnClose = null) {
            tipsController.ShowTips(tips, callOnClose);
        }

        public void ShowConfirmWindow(Action confirm, Action cancel = null) {
            confirmWindowController.ShowConfirmWindow(confirm, cancel);
        }

        void OnDestroy() {
            gameManager = null;
            Resume();
        }

        void OnApplicationQuit() {
            Repository.Instance.Dispose();
        }

    }

}
