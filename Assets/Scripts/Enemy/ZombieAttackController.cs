using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    using MotionState = ZombieMotionController.MotionState;

    public class ZombieAttackController : MonoBehaviour {

        // 攻击距离
        public float attackRadius = 1.0f;

        // 攻击的有效角度
        public float attackRightSideAngle = 60.0f;

        // 攻击的有效角度
        public float attackLeftSideAngle = -30.0f;

        public float damage = 20.0f;

        // 表示本次攻击动画播放的过程中是否攻击到玩家
        private bool attackFlag = false;

        private ZombieMotionController motionController;

        private ZombieSensorController sensorController;

        private Animator animator;

	    void Start () {
            motionController = GetComponent<ZombieMotionController>();
            sensorController = GetComponent<ZombieSensorController>();
            animator = GetComponent<Animator>();
	    }

	    void Update () {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }
            if (motionController.CurrentMotionState != MotionState.Attacking) {
                return;
            }
            Transform player = sensorController.GetNearByPlayer();
            if (player == null) {
                return;
            }
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.Attack")) {
                if (state.normalizedTime > 0.3f && state.normalizedTime < 0.5f) {
                    if (attackFlag) {
                        return;
                    }
                    if (Vector3.Distance(player.position, transform.position) > attackRadius) {
                        return;
                    }
                    Vector3 direction = player.position - transform.position;
                    float angle = Vector3.Angle(transform.forward, direction);
                    if (angle <= attackRightSideAngle && angle >= attackLeftSideAngle) {
                        PlayerHealthManager playerHealth = player.GetComponent<PlayerHealthManager>();
                        playerHealth.ReduceHealth(transform.position, damage);
                        attackFlag = true;
                    }
                }
                if (state.normalizedTime > 0.9f) {
                    attackFlag = false;
                }
            }

	    }
    }

}
