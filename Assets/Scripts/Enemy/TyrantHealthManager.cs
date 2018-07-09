using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MyResidentEvil {

    using MotionState = TyrantMotionController.MotionState;

    public class TyrantHealthManager : EnemyHealthManager {

        private float health = 2500;

        private float attackCount = 0;

        private TyrantMotionController motionController;

        private TyrantSensorController sensorController;

        void Start() {
            motionController = GetComponent<TyrantMotionController>();
            sensorController = GetComponent<TyrantSensorController>();
        }

        void Update() {
            if (motionController.CurrentMotionState == MotionState.Dead) {
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
            if (motionController.WasHitFlyTrigger(position, distance)) {
                health -= damage;
            }
        }

        public override void UnderAttack(Vector3 position, Vector3 shotPoint, float damage) {
            switch(motionController.CurrentMotionState){
                case MotionState.Dead:
                case MotionState.HitFly:
                case MotionState.StandingUp:
                    return;
            }
            attackCount += damage;
            health -= damage;
            if (IsShotHead(shotPoint)) {
                attackCount += damage;
                health -= damage;
                attackCount = 0;
                motionController.AgonizeTrigger();
                return;
            }
            if (attackCount > 200) {
                motionController.UnderAttackTrigger(position);
                attackCount = 0;
            }
        }

        // 是否击中了头部
        private bool IsShotHead(Vector3 shotPoint) {
            if (shotPoint.y - transform.position.y >= 1.8f) {
                return true;
            }
            return false;
        }

    }

}

