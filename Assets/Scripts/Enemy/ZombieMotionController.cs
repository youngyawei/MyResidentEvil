using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyResidentEvil {

    public class ZombieMotionController : EnemyMotionController {

        // 感知不到玩家的最长时间
        public float returnTime = 10.0f;

        public MotionState initMotionState = MotionState.Sleeping;

        private ZombieSensorController zombieSensorController;

        private ZombieAttackController zombieAttackController;

        private float loseTime = 0;                 // 没有感知到玩家的持续时间

        private float stuckTime = 0;                // 在导航状态下在某个地方停留的持续时间

        private Vector3 prePosition;

        private Vector3 bornPosition;               // 僵尸的出生点        

        private AudioSource audioSource;

        void Start () {
            currentMotionState = MotionState.Sleeping;
            zombieSensorController = GetComponent<ZombieSensorController>();
            zombieAttackController = GetComponent<ZombieAttackController>();
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();
            bornPosition = transform.position;
            prePosition = transform.position;
            if (initMotionState != MotionState.Sleeping) {
                animator.SetTrigger("Idle");
                currentMotionState = MotionState.Idling;
            }
	    }

	    void Update () {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }
            if (currentMotionState == MotionState.Dead) {
                return;
            }
            switch(currentMotionState) {
                case MotionState.Sleeping:
                case MotionState.WakingUp:
                    WakeUp();
                    break;
                case MotionState.Idling:
                case MotionState.Moving:
                    MoveAndRotate();
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

        // 唤醒敌人
        private void WakeUp() {
            if (currentMotionState == MotionState.Sleeping) {
                Transform player = zombieSensorController.GetNearByPlayer();
                if (player == null) {
                    return;
                }
                currentMotionState = MotionState.WakingUp;
                animator.SetTrigger("WakeUp");
            }

            if (currentMotionState == MotionState.WakingUp) {
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("BaseLayer.StandUpFromSleep")) {
                    if (state.normalizedTime > 0.9f) {
                        currentMotionState = MotionState.Idling;
                    }
                    if (state.normalizedTime < 0.2f) {
                        if (!audioSource.isPlaying) {
                            AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, "ZombieAttack.wav");
                        }
                    }
                }
            }
        }

        public override bool UnderAttackTrigger(Vector3 position) {
            switch (currentMotionState) {
                case MotionState.WakingUp:
                case MotionState.HitFly:
                case MotionState.StandingUp:
                case MotionState.Dead:
                    return false;
            }
            if (currentMotionState == MotionState.Moving) {
                navMeshAgent.ResetPath();
            }
            if (currentMotionState != MotionState.Sleeping) {
                transform.LookAt(position);
            }
            currentMotionState = MotionState.UnderAttacking;
            return true;
        }

        protected override void UnderAttack() {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.Sleep")) {
                currentMotionState = MotionState.WakingUp;
                animator.SetTrigger("WakeUp");
            } else if (state.IsName("BaseLayer.ReactionHit")) {
                if (state.normalizedTime > 0.9f) {
                    currentMotionState = MotionState.Idling;
                    animator.SetBool("ReactionHit", false);
                } else {
                    transform.position = transform.position - transform.forward * Time.deltaTime;
                }
            } else {
                animator.SetBool("Attack", false);
                animator.SetBool("Move", false);
                animator.SetBool("Agonize", false);
                animator.SetBool("ReactionHit", true);
            }
        }

        protected override void Attack() {
            if (currentMotionState != MotionState.Attacking) {
                return;
            }
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.Attack")) {
                if (state.normalizedTime > 0.9f) {
                    currentMotionState = MotionState.Idling;
                    animator.SetBool("Attack", false);
                }
                if (state.normalizedTime > 0.2f && state.normalizedTime < 0.3f) {
                    AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, "ZombieAttack.wav");
                }
                //transform.Rotate(Vector3.up * -15 * Time.deltaTime);
            } else {
                animator.SetBool("Attack", true);
                transform.LookAt(zombieSensorController.GetNearByPlayer());
            }
        }

        protected override void MoveAndRotate() {
            Transform player = zombieSensorController.GetNearByPlayer();
            if (currentMotionState == MotionState.Idling) {
                stuckTime = 0;
                if (player != null) {
                    loseTime = 0;
                    navMeshAgent.destination = player.position;
                    currentMotionState = MotionState.Moving;
                    animator.SetBool("Move", true);
                } else {
                    loseTime += Time.deltaTime;
                    if (loseTime > returnTime) {
                        ReturnToBornPosition();
                        loseTime = 0;
                    }
                }
            }
            if (currentMotionState == MotionState.Moving) {
                if (Vector3.Distance(prePosition, transform.position) > 0.2f ) {
                    stuckTime = 0;
                    prePosition = transform.position;
                } else {
                    stuckTime += Time.deltaTime;
                }
                if (player == null) {
                    if (Vector3.Distance(transform.position, navMeshAgent.destination) <= navMeshAgent.stoppingDistance) {
                        navMeshAgent.ResetPath();
                        currentMotionState = MotionState.Idling;
                        animator.SetBool("Move", false);
                        return;
                    }
                } else {
                    float distance = Vector3.Distance(transform.position, player.position);
                    navMeshAgent.destination = player.position;
                    if (distance > zombieAttackController.attackRadius) {
                        if (currentMotionState == MotionState.Idling) {
                            currentMotionState = MotionState.Moving;
                        }
                        animator.SetBool("Move", true);
                    } else {
                        currentMotionState = MotionState.Attacking;
                        animator.SetBool("Move", false);
                        navMeshAgent.ResetPath();
                    }
                }
                if (stuckTime >= 5.0f) {
                    navMeshAgent.ResetPath();
                    animator.SetBool("Move", false);
                    currentMotionState = MotionState.Idling;
                }
            }
        }

        // 返回出生点
        private void ReturnToBornPosition() {
            if (Vector3.Distance(transform.position, bornPosition) > 0.1f) {
                navMeshAgent.destination = bornPosition;
                currentMotionState = MotionState.Moving;
                animator.SetBool("Move", true);
            }
        }

        // 使僵尸复生
        public void Resurgence() {
            if (currentMotionState == MotionState.Dead) {
                currentMotionState = MotionState.Sleeping;
            }
        }

        protected override void Dead() {
            animator.SetTrigger("Die");
            currentMotionState = MotionState.Dead;
            if (!audioSource.isPlaying) {
                AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, "ZombieHurt.wav");
            }
            navMeshAgent.ResetPath();
            animator.SetBool("Move", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Agonize", false);
            animator.SetBool("HitFly", false);
            animator.SetBool("ReactionHit", false);
        }

    }

}

