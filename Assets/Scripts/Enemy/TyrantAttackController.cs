using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    using MotionState = TyrantMotionController.MotionState;

    public class TyrantAttackController : MonoBehaviour {

        public float attackRadius = 1.5f;

        public float attackAngle = 150.0f;

        public float hitFlyDistance = 8.0f;

        public float damage = 40.0f;

        private TyrantMotionController motionController;

        private TyrantSensorController sensorController;

        private Animator animator;

        private bool attackFlag = false;

        private PlayerHealthManager playerHealth;

        void Start() {
            motionController = GetComponent<TyrantMotionController>();
            sensorController = GetComponent<TyrantSensorController>();
            animator = GetComponent<Animator>();
        }

        void Update() {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }
            Transform player = sensorController.GetNearByPlayer();
            if (player == null) {
                return;
            }
            if(attackFlag) {
                return;
            }
            if (motionController.CurrentMotionState == MotionState.Attacking) {
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("BaseLayer.Attack")) {
                    if (state.normalizedTime >= 0.2 && state.normalizedTime <= 0.4) {
                        if (Vector3.Distance(player.position, transform.position) > attackRadius) {
                            return;
                        }
                        Vector3 direction = player.position - transform.position;
                        float angle = Vector3.Angle(transform.forward, direction);
                        if (angle <= attackAngle/2 && angle >= -attackAngle/2) {
                            if (playerHealth == null) {
                                playerHealth = player.GetComponent<PlayerHealthManager>();
                            }
                            playerHealth.WasHitFly(transform.position, hitFlyDistance, damage);
                        }

                    }
                }
            }
        }

    }

}

