using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyResidentEvil {

    public class OpenDoorManager : MonoBehaviour {

        public GameObject door;

        private Animator doorAnimator;

        public GameObject player;

        private NavMeshAgent navMeshAgent;

        public GameObject mainCamera;

        private AudioSource audioSource;

        public Vector3 destination = new Vector3(0, 0, 5.0f);

        private bool open = false;

        private float interval;

        void Start () {
            doorAnimator = door.GetComponent<Animator>();
            audioSource = mainCamera.GetComponent<AudioSource>();
            Instantiate(Resources.Load<GameObject>("UI/LoadingTextCanvas"));
            AudioClipLoader.Instance.LoadAudioClip(audioSource, "Open Door.mp3");
            StartCoroutine(OpenDoor());
            //player.transform.position = new Vector3(0,0,0.09f);
            interval = player.transform.position.z - mainCamera.transform.position.z;
            navMeshAgent = player.GetComponent<NavMeshAgent>();
            AssetBundleManager.Instance.UnLoadAssetBundle("scene/open_door");
        }

        private IEnumerator OpenDoor() {
            yield return new WaitForSeconds(1.0f);
            doorAnimator.SetBool("open", true);
            audioSource.Play();
            yield return new WaitForSeconds(2.0f);
            AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, "Open Door2.mp3");
        }

        void Update() {
            if (!open) {
                AnimatorStateInfo state = doorAnimator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("Door_open")) {
                    if(state.normalizedTime > 0.9f) {
                        open = true;
                        StartCoroutine(Walk());
                    }
                }
            }
        }

        private IEnumerator Walk() {
            yield return new WaitForSeconds(1.0f);
            navMeshAgent.destination = destination;
        }

        void LateUpdate() {
            mainCamera.transform.position = new Vector3(0, mainCamera.transform.position.y, player.transform.position.z - interval);
        }

    }

}
