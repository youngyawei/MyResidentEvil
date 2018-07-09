using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    using MotionState = HumanoidBaseMotionController.MotionState;

    public class HelicopterPakingStattionManager : BaseSceneManager {

        public GameObject helicopter;

        public GameObject minorCamera;

        public GameObject tyrant;

        private GameObject player;

        private TyrantMotionController tyrantMC;

        private HelicopterController helicopterController;

        private BackgroundMusicPlayer bgmPlayer;

        private AudioSource audioSource;

        void Start() {
            tyrantMC = tyrant.GetComponent<TyrantMotionController>();
            helicopterController = helicopter.GetComponent<HelicopterController>();
            player = GameObject.FindGameObjectWithTag("Player");
            bgmPlayer = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BackgroundMusicPlayer>();
            audioSource = minorCamera.GetComponent<AudioSource>();
            AudioClipLoader.Instance.LoadAudioClip(audioSource, "And After That.mp3");
            Instantiate(Resources.Load<GameObject>("Effects/DustStormMobile"));
            Init();
            if (!Archive.CurrentArchive.CurrentSceneData.Entry) {
                StartCoroutine(ShowTips());
            }
        }

        private IEnumerator ShowTips() {
            yield return new WaitForSeconds(2.0f);
            Archive.CurrentArchive.CurrentSceneData.Entry = true;
            GameManager.Instance.ShowTips("太好了，找到直升机了，等等，直升机旁边那个是...", () => {
                GameManager.Instance.ShowTips("可恶，我们明明已经打倒他了，没想到居然还活着\n而且还是在这里，想把我们赶尽杀绝吗", () => {
                    GameManager.Instance.ShowTips("不过看样子之前的攻击也给他造成了巨大的伤害\n没有装甲的保护他身上的弱点全暴露出来了", () => {
                        GameManager.Instance.ShowTips("这样一来或许我就有机会把他干掉");
                    });
                });
            });
        }

        void Update() {
            if (minorCamera.activeInHierarchy) {
                minorCamera.transform.RotateAround(helicopter.transform.position, Vector3.up, Time.deltaTime*5);
                minorCamera.transform.LookAt(helicopter.transform);
            }
            if (tyrantMC.CurrentMotionState != MotionState.Dead) {
                return;
            }
            if (tyrant != null) {
                tyrant = null;
                bgmPlayer.StopPlay();
                GameManager.Instance.ShowTips("去死吧，怪物");
            }
            if (player == null) {
                return;
            }
            if (Vector3.Distance(helicopter.transform.position, player.transform.position) < 2.0f) {
                if (CrossPlatformInputManager.GetButton("J")) {
                    GameManager.Instance.ShowTips("终于能离开这个地狱般的城市了", () => {
                        Instantiate(Resources.Load<GameObject>("UI/FadeoutMaskFCanvas"));
                        Destroy(player);
                        player = null;
                        minorCamera.SetActive(true);
                        audioSource.Play();
                        helicopterController.OpenLight();
                        helicopterController.Fly();
                        StartCoroutine(FinishGame());
                    });
                }
            }
        }

        private IEnumerator FinishGame() {
            yield return new WaitForSeconds(15.0f);
            helicopterController.StopFly();
            GameManager.Instance.GameSuccess();
        }


    }

}
