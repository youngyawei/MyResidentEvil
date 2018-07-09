using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyResidentEvil {

    public class TyrantMotionController : EnemyMotionController {

        public float shoutingInterval = 15.0f;

        private float shoutingCount = 0.0f;

        // 0 是行走 , 1 是奔跑
        private float moveStyle = 0;

        private TyrantSensorController sensorController;

        private TyrantAttackController attackController;

        private AudioSource audioSource;

        void Start () {
            sensorController = GetComponent<TyrantSensorController>();
            attackController = GetComponent<TyrantAttackController>();
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();
            AudioClipLoader.Instance.LoadAudioClip(audioSource, "SB_VoxPat_Angry Zombie.wav");
            shoutingCount = shoutingInterval / 2;
        }

	    void Update () {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }

            if (currentMotionState == MotionState.Dead) {
                return;
            }

            shoutingCount += Time.deltaTime;
            if (shoutingCount >= shoutingInterval) {
                if (currentMotionState == MotionState.Idling || currentMotionState == MotionState.Moving) {
                    StopMove();
                    currentMotionState = MotionState.Shouting;
                }
            }

            switch (currentMotionState) {
                case MotionState.Idling:
                case MotionState.Moving:
                    MoveAndRotate();
                    break;
                case MotionState.Shouting:
                    Shouting();
                    break;
                case MotionState.Attacking:
                    Attack();
                    break;
                case MotionState.UnderAttacking:
                    UnderAttack();
                    break;
                case MotionState.Agonizing:
                    Agonize();
                    break;
                case MotionState.Dying:
                    Dead();
                    break;
            }
	    }

        void FixedUpdate() {
            if (currentMotionState == MotionState.HitFly || currentMotionState == MotionState.StandingUp) {
                WasHitFly();
            }
        }

        public override bool UnderAttackTrigger(Vector3 position) {
            switch (currentMotionState) {
                case MotionState.Idling:
                case MotionState.Moving:
                case MotionState.Attacking:
                case MotionState.UnderAttacking:
                case MotionState.Agonizing:
                    transform.LookAt(position);
                    animator.SetBool("Attack", false);
                    animator.SetBool("Agonize", false);
                    StopMove();
                    currentMotionState = MotionState.UnderAttacking;
                    return true;
            }
            return false;
        }

        protected override void UnderAttack() {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.ReactionHit")) {
                if (state.normalizedTime > 0.9f) {
                    currentMotionState = MotionState.Idling;
                    animator.SetBool("ReactionHit", false);
                } else {
                    transform.position = transform.position - transform.forward * Time.deltaTime;
                }
            } else {
                animator.SetBool("ReactionHit", true);
            }
        }

        protected override void Attack() {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.Attack")) {
                if (state.normalizedTime > 0.9) {
                    currentMotionState = MotionState.Idling;
                    animator.SetBool("Attack", false);
                }
            } else {
                animator.SetBool("Attack", true);
            }
        }

        private void Shouting() {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.Shouting")) {
                if (state.normalizedTime > 0.9) {
                    currentMotionState = MotionState.Idling;
                    animator.SetBool("Shouting", false);
                    shoutingCount = 0;
                }
                if (state.normalizedTime > 0.20f && state.normalizedTime < 0.55f) {
                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                }
            } else {
                animator.SetBool("Shouting", true);
                transform.LookAt(sensorController.GetNearByPlayer());
            }
        }

        protected override void MoveAndRotate() {
            Transform player = sensorController.GetNearByPlayer();
            if (player == null) {
                return;
            }
            if (currentMotionState == MotionState.Idling) {
                navMeshAgent.speed = 1;
                moveStyle = 0;
            }
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= attackController.attackRadius) {
                StopMove();
                currentMotionState = MotionState.Attacking;
            } else {
                if (distance >= 8) {
                    moveStyle = 1;
                }
                Move(player);
            }
        }

        private void Move(Transform player) {
            navMeshAgent.speed = moveStyle == 0 ? 1 : 2;
            navMeshAgent.destination = player.position;
            animator.SetBool("Move", true);
            animator.SetFloat("MoveStyle", moveStyle);
            currentMotionState = MotionState.Moving;
        }

        private void StopMove() {
            navMeshAgent.speed = 1;
            navMeshAgent.ResetPath();
            animator.SetBool("Move", false);
            moveStyle = 0;
            animator.SetFloat("MoveStyle", moveStyle);
            currentMotionState = MotionState.Idling;
        }

        public override bool WasHitFlyTrigger(Vector3 position, float distance) {
            if (base.WasHitFlyTrigger(position, distance)) {
                animator.SetBool("Shouting", false);
                return true;
            }
            return false;
        }


        protected override void Dead() {
            animator.SetTrigger("Die");
            StopMove();
            animator.SetBool("Shouting", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Agonize", false);
            animator.SetBool("HitFly", false);
            animator.SetBool("ReactionHit", false);
            currentMotionState = MotionState.Dead;
        }

    }

}

