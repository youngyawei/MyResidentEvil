using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    public class HelicopterController : MonoBehaviour {

        private Animator animator;

        private AudioSource audioSource;

        private Light spotlight;

        private bool fly = false;

        private float inteval = 0;

        private byte flyFlag = 0;

        private float flyCount = 0;

        private bool playAudio = true;

        void Start () {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            spotlight = transform.Find("Spotlight").GetComponent<Light>();
            AudioClipLoader.Instance.LoadAudioClip(audioSource, "Helicopter.wav");
            animator.SetBool("Fly", true);
        }

        void Update() {
            if (playAudio) {
                if (inteval == 0) {
                    audioSource.Play();
                }
                inteval += Time.unscaledDeltaTime;
                if (inteval >= 2.0f) {
                    inteval = 0;
                }
            }
            if (fly) {
                switch (flyFlag) {
                    case 0:
                        if (flyCount >= 3.0f) {
                            flyFlag++;
                            flyCount = 0;
                            break;
                        }
                        transform.Translate(Vector3.up * Time.deltaTime);
                        flyCount += Time.deltaTime;
                        break;
                    case 1:
                        if (flyCount >= 180.0f) {
                            flyFlag++;
                            flyCount = 0;
                            break;
                        }
                        transform.Rotate(Vector3.up * Time.deltaTime * 30);
                        flyCount += Time.deltaTime * 30;
                        break;
                    case 2:
                        if (flyCount >= 10) {
                            flyFlag++;
                            flyCount = 0;
                            break;
                        }
                        transform.Translate(Vector3.forward * Time.deltaTime);
                        flyCount += Time.deltaTime;
                        break;
                }
            }
        }

        public void Fly() {
            fly = true;
            playAudio = false;
            audioSource.Stop();
            inteval = 0;
        }

        public void StopFly() {
            fly = false;
        }

        public void OpenLight() {
            spotlight.intensity = 3;
        }

        public void CloseLight() {
            spotlight.intensity = 0;
        }



    }

}
