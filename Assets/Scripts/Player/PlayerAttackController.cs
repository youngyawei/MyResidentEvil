using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MyResidentEvil {

    using MotionState = PlayerMotionController.MotionState;

    public class PlayerAttackController : MonoBehaviour {

        private PlayerMotionController motionController;

        private GameObject gunShell;

        private GunShellController gunShellController;

        private Transform aimPoint;

        private Ray ray = new Ray();
        private RaycastHit raycastHit;

        private GameObject sparkGameObject;
        private ParticleSystem spark;

	    void Start () {
            motionController = GetComponent<PlayerMotionController>();
            gunShell = transform.Find("GunWithCamera").Find("GunShell").gameObject;
            gunShellController = gunShell.GetComponent<GunShellController>();
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            aimPoint = mainCamera.transform.Find("AimPoint");

            GameObject sparkPrefab = Resources.Load<GameObject>("Effects/Spark");
            sparkGameObject = Instantiate(sparkPrefab);
            spark = sparkGameObject.GetComponent<ParticleSystem>();
        }

	    void FixedUpdate () {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }
            if (motionController.CurrentMotionState == MotionState.Dead) {
                return;
            }
            switch (motionController.CurrentMotionState) {
                case MotionState.Idling:
                case MotionState.Moving:
                case MotionState.Attacking:
                    if (gunShell.activeInHierarchy) {
                        if (CrossPlatformInputManager.GetButton("K") && gunShellController.Fire()) {
                            Fire();
                        }
                    }
                    break;
            }
	    }

        private void Fire() {
            ray.origin = aimPoint.position;
            ray.direction = aimPoint.forward;
            if (Physics.Raycast(ray, out raycastHit, gunShellController.GunShot)) {
                EnemyHealthManager healthManager = raycastHit.transform.GetComponent<EnemyHealthManager>();
                if (raycastHit.transform.tag == "Enemy" || raycastHit.transform.tag == "EnemyHead") {
                    healthManager.UnderAttack(transform.position, raycastHit.point, gunShellController.GunDamage);
                }
                spark.transform.position = raycastHit.point;
                spark.Play();
            }
        }

    }

}

