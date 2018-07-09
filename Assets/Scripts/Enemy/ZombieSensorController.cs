using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    using MotionState = ZombieMotionController.MotionState;

    // 用于处理僵尸对玩家的感知
    public class ZombieSensorController : EnemySensorController {

        // 视线范围
        public float sightRadius = 15.0f;

        // 视线角度
        public float sightAngle = 120.0f;

        // 最大听觉感知范围
        public float maxListenRadius = 10.0f;

        // 最小听觉感知范围
        public float minListenRadius = 5.0f;

        // 感知间隔
        public float interval = 1.0f;

        private Transform zombieEyes;

        private GameObject player;

        private bool sensor = false;

        private ZombieMotionController zombieMotionController;

        private Ray ray = new Ray();

        private RaycastHit raycastHit;

        private float sensorTime = 0;

        void Start() {
            player = GameObject.FindGameObjectWithTag("Player");
            zombieMotionController = GetComponent<ZombieMotionController>();
            zombieEyes = transform.Find("Eyes");
        }

        private void FixedUpdate() {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }
            if (player == null) {
                player = GameObject.FindGameObjectWithTag("Player");
                if (player == null) return;
            }
            if (sensorTime >= interval) {
                if (zombieMotionController.CurrentMotionState == MotionState.Sleeping) {
                    sensor = Listen();
                } else {
                    sensor = Listen() ? true : Look();
                }
                sensorTime = 0;
            }
            sensorTime += Time.deltaTime;
        }

        private bool Listen() {
            float d = minListenRadius;
            if (Archive.CurrentArchive.PowerSource) {
                d = maxListenRadius;
            }

            if (Vector3.Distance(transform.position, player.transform.position) <= d) {
                return true;
            }
            return false;
        }

        private bool Look() {
            float distance = Vector3.Distance(zombieEyes.transform.position, player.transform.position);
            if (distance > sightRadius) {
                return false;
            }
            Vector3 direction = player.transform.position + Vector3.up*1.5f - zombieEyes.transform.position;
            float angle = Vector3.Angle(zombieEyes.transform.forward, direction);
            if (angle > sightAngle/2 || angle < -sightAngle/2) {
                return false;
            }
            ray.origin = zombieEyes.transform.position;
            ray.direction = direction;
            if (Physics.Raycast(ray, out raycastHit, sightRadius)) {
                if (raycastHit.transform == player.transform) {
                    return true;
                }
            }
            return false;
        }

        public override Transform GetNearByPlayer() {
            if (sensor) {
                return player.transform;
            }
            return null;
        }

    }

}

