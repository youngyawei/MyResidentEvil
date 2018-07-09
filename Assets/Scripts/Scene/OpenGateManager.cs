using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyResidentEvil {

    public class OpenGateManager : MonoBehaviour {

        public GateController gateController;

        public GameObject player;

        private NavMeshAgent navMeshAgent;

        public GameObject mainCamera;

        private AudioSource audioSource;

        public Vector3 destination = new Vector3(0, 0, -2.59f);

        private bool walking = false;

        private float interval;
        
	    void Start () {
            audioSource = mainCamera.GetComponent<AudioSource>();
            Instantiate(Resources.Load<GameObject>("UI/LoadingTextCanvas"));
            AudioClipLoader.Instance.LoadAudioClip(audioSource, "Open Gate.mp3");
            StartCoroutine(OpenGate());
            interval = player.transform.position.z - mainCamera.transform.position.z;
            navMeshAgent = player.GetComponent<NavMeshAgent>();
            AssetBundleManager.Instance.UnLoadAssetBundle("scene/open_gate");
        }

        private IEnumerator OpenGate() {
            yield return new WaitForSeconds(2.0f);
            gateController.OpenGate();
            audioSource.Play();
        }

        void Update() {
            if (gateController.Open && !walking) {
                walking = true;
                navMeshAgent.destination = destination;
                audioSource.Stop();
            }
        }

        void LateUpdate() {
            mainCamera.transform.position = new Vector3(0, mainCamera.transform.position.y, player.transform.position.z - interval);
        }

    }

}

