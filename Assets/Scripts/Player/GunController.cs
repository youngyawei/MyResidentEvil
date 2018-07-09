using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MyResidentEvil {

    public class GunController : MonoBehaviour {

        public Transform leftHandIKPosition;

        public Transform rightHandIKPosition;

        public Transform lookAtPosition;

        // 不需要设置
        public Transform bodyIKPosition;

        // HandGun Vector3(0, 0.227f, 0.784f);
        // MechinaGun Vector3(0.195, -0.237, 0.306);
        public Vector3 gunPosition;

        private ParticleSystem ps;

        private Light fireLight;

        private AudioSource audioSource;

        public GameObject gunFireEffect;

        private bool isReady = false;

        // 射击间隔
        public float shootInterval = 1.0f;
        private float shootCount = 0;
        // 射程
        public float gunShot = 50.0f;
        // 伤害
        public float damage = 15.0f;

        // 枪声
        public string gunSound;

        void Start() {
            ps = gunFireEffect.GetComponent<ParticleSystem>();
            fireLight = gunFireEffect.GetComponent<Light>();
            audioSource = GetComponent<AudioSource>();
            AudioClipLoader.Instance.LoadAudioClip(audioSource, gunSound);
        }

        void Update() {
            if (!isReady) {
                return;
            }
            shootCount += Time.deltaTime;
            if (ps.isStopped) {
                fireLight.intensity = 0;
            }
	    }

        // 开枪 , 成功开火则返回 true
        public bool Fire() {
            if (shootCount >= shootInterval) {
                audioSource.Play();
                ps.Play(true);
                fireLight.intensity = 1;
                shootCount = 0;
                return true;
            }
            return false;
        }

        // 装配武器
        public void Fit() {
            // 调整武器的位置
            transform.localPosition = gunPosition;
            isReady = true;
        }

    }

}

