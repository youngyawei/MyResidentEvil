using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MyResidentEvil {

    using MotionState = ZombieMotionController.MotionState;

    public class ZombieHealthManager : EnemyHealthManager {

        public float resurgenceTime = 240.0f;

        private float resurgenceCount = 0;

        private float health = 200;

        private float damageCount = 0;

        private ZombieMotionController motionController;

        private ZombieSensorController sensorController;

        void Start () {
            motionController = GetComponent<ZombieMotionController>();
            sensorController = GetComponent<ZombieSensorController>();
        }
	    
	    void Update () {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }
            if (motionController.CurrentMotionState == MotionState.Dead) {
                resurgenceCount += Time.deltaTime;
                if (resurgenceCount >= resurgenceTime) {
                    motionController.Resurgence();
                    health = 200;
                    resurgenceCount = 0;
                }
                return;
            }
            if (health <= 0) {
                motionController.DeadTrigger();
                return;
            }
            Transform player = sensorController.GetNearByPlayer();
            if (player == null) {
                return;
            }
            if (SpinKickAble(player, motionController.CurrentMotionState)) {
                if (CrossPlatformInputManager.GetButton("J")) {
                    PlayerMotionController pmc = player.GetComponent<PlayerMotionController>();
                    pmc.SpinKickTrigger();
                }
            }

        }

        public override void SufferSpinKick(Vector3 position, float distance, float damage) {
            if(motionController.WasHitFlyTrigger(position, distance)) {
                health -= damage;
            }
        }

        public override void UnderAttack(Vector3 position, Vector3 shotPoint, float damage) {
            switch (motionController.CurrentMotionState) {
                case MotionState.Sleeping:
                    motionController.UnderAttackTrigger(position);
                    break;
                default:
                    if (IsShotHead(shotPoint)) {
                        if (motionController.AgonizeTrigger()) {
                            health -= 2*damage;
                            damageCount = 0;
                            transform.LookAt(position);
                        }
                    } else {
                        switch (motionController.CurrentMotionState) {
                            case MotionState.WakingUp:
                            case MotionState.HitFly:
                            case MotionState.StandingUp:
                            case MotionState.Dead:
                                return;
                        }
                        transform.LookAt(position);
                        damageCount += damage;
                        health -= damage;
                        if (damageCount > 50.0f) {
                            motionController.UnderAttackTrigger(position);
                            damageCount = 0;
                        }
                    }
                    break;
            }
        }

        // 是否击中了头部
        private bool IsShotHead(Vector3 shotPoint) {
            if (shotPoint.y - transform.position.y >= 1.6f) {
                return true;
            }
            return false;
        }

    }

}

