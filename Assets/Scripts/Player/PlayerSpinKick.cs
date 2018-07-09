using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    using MotionState = PlayerMotionController.MotionState;

    public class PlayerSpinKick : MonoBehaviour {

        // 回旋踢的有效角度
        public float spinKickAngle = 120.0f;

        // 回旋踢的有效距离
        public static float spinKickRadius = 1.5f;

        // 回旋踢的伤害
        public float damage = 60.0f;

        // 将目标踢飞的距离
        public float hitFlyDistance = 8.0f;

        private PlayerMotionController motionController;

        private Animator animator;

        private class EnemyInfo {
            private GameObject enemy;
            private EnemyHealthManager enemyHealth;
            private bool fly;

            public EnemyInfo(GameObject enemy) {
                this.enemy = enemy;
                enemyHealth = enemy.GetComponent<EnemyHealthManager>();
                fly = false;
            }

            public Vector3 Position {
                get { return enemy.transform.position; }
            }
            public EnemyHealthManager EnemyHealthManager {
                get { return enemyHealth; }
            }
            public bool Fly {
                get { return fly; }
                set { fly = value; }
            }

        }

        private EnemyInfo[] enemyInfos;

        private float interval;

	    void Start () {
            motionController = GetComponent<PlayerMotionController>();
            animator = transform.Find("SWAT_Female").GetComponent<Animator>();
            FindEnemy();
        }

        private void FindEnemy() {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            enemyInfos = new EnemyInfo[enemies.Length];
            for (int i = 0; i < enemies.Length; i++) {
                enemyInfos[i] = new EnemyInfo(enemies[i]);
            }
        }
	
	    void Update () {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }

            if (enemyInfos.Length == 0) {
                interval += Time.deltaTime;
                if (interval >= 3.0f) {
                    FindEnemy();
                    interval = 0;
                }
                return;
            }

            if (motionController.CurrentMotionState == MotionState.SpinKicking) {
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("BaseLayer.Kick")) {
                    if (state.normalizedTime > 0.3 && state.normalizedTime < 0.6) {
                        for (int i = 0; i < enemyInfos.Length; i++) {
                            if (enemyInfos[i].Fly) {
                                continue;
                            }
                            if (HitFlyAble(enemyInfos[i].Position)) {
                                enemyInfos[i].Fly = true;
                                enemyInfos[i].EnemyHealthManager.SufferSpinKick(transform.position, hitFlyDistance, damage);
                            }
                        }
                    }
                }
                if (state.normalizedTime > 0.9f) {
                    for (int i = 0; i < enemyInfos.Length; i++) {
                        enemyInfos[i].Fly = false;
                    }
                }
            }
	    }

        private bool HitFlyAble(Vector3 enemyPosition) {
            if (Vector3.Distance(transform.position, enemyPosition) > spinKickRadius) {
                return false;
            }
            Vector3 direction = enemyPosition - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            if (angle <= spinKickAngle/2 && angle >= -spinKickAngle/2) {
                return true;
            }
            return false;
        }


    }

}
