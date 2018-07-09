using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MyResidentEvil {

    using MotionState = PlayerMotionController.MotionState;

    // 摄像机控制器 , 用于处理在瞄准状态时对镜头进行缩放 , 以及防止摄像机被遮挡
    public class CameraController : MonoBehaviour {

        // 摄像机在瞄准时移动的最终目标位置
        private Vector3 cameraDestrition = new Vector3(0, 0.2f, 1.0f);

        private Vector3 cameraDestOnDead = new Vector3(0, 1.0f, 0);

        private float deadRotate = 60.0f;

        public GameObject player;

        private PlayerMotionController playerMotionController;

        private Collider otherCollider;

        // 表示摄像机是否可以复位
        private bool resetFlag = true;

        void Start () {
            playerMotionController = player.GetComponent<PlayerMotionController>();
	    }


        void Update() {
            switch (playerMotionController.CurrentMotionState) {
                case MotionState.Idling:
                    float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
                    float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");
                    // 在玩家移动或旋转时摄像机可以复位
                    if (horizontal != 0 || vertical != 0) {
                        resetFlag = true;
                    }
                    break;
                case MotionState.Moving:
                case MotionState.Eluding:
                case MotionState.HitFly:
                    resetFlag = true;
                    break;
            }
        }

        void LateUpdate () {
            MotionState state = playerMotionController.CurrentMotionState;
            if (state == MotionState.Dead) {
                // 处理玩家死亡后摄像机的移动
                transform.localPosition = Vector3.Lerp(transform.localPosition, cameraDestOnDead, Time.deltaTime);
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.right*deadRotate, Time.deltaTime);
                return;
            }
            if (state == MotionState.Attacking) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, cameraDestrition, 0.5f);
            } else {
                // 如果没有撞到东西并且可以复位
                if (otherCollider == null && resetFlag) {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
                }
            }
	    }

        void FixedUpdate() {
            MotionState state = playerMotionController.CurrentMotionState;
            if (state == MotionState.Dead) {
                return;
            }
            if (otherCollider != null && state != MotionState.Attacking) {
                if (transform.localPosition.z < cameraDestrition.z) {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0, 0, 0.05f), 0.5f);
                }
            }
        }

        void OnTriggerEnter(Collider other) {
            string tag = other.gameObject.tag;
            if (tag == "Enemy" || tag == "Player") {
                otherCollider = null;
                return;
            }
            otherCollider = other;
            resetFlag = false;
        }

        void OnTriggerStay(Collider other) {
            string tag = other.gameObject.tag;
            if (tag == "Enemy" || tag == "Player") {
                otherCollider = null;
                return;
            }
            otherCollider = other;
            resetFlag = false;
        }

        void OnTriggerExit(Collider other) {
            otherCollider = null;
        }

    }

}

