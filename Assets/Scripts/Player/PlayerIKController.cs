using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    using MotionState = PlayerMotionController.MotionState;

    public class PlayerIKController : MonoBehaviour {

        private Animator playerAnimator;

        public bool isIKActive = true;

        public GameObject player;

        public GameObject gunShell;

        private PlayerMotionController playerMotionController;

        private GunShellController gunShellController;

        void Start () {
            playerAnimator = GetComponent<Animator>();
            playerMotionController = player.GetComponent<PlayerMotionController>();
            gunShellController = gunShell.GetComponent<GunShellController>();
	    }

        void Update() {
            MotionState state = playerMotionController.CurrentMotionState;
            if (state == MotionState.Idling || state == MotionState.Moving || state == MotionState.Attacking) {
                isIKActive = true;
            } else {
                isIKActive = false;
            }
        }

        void OnAnimatorIK(int layerIndex) {
            if (playerAnimator == null) {
                return;
            }
            GunController gun = gunShellController.CurrentGunController;
            if (isIKActive && gun != null) {
                // 对头手躯干增加逆向动力控制
                if (gun.lookAtPosition != null) {
                    playerAnimator.SetLookAtPosition(gun.lookAtPosition.position);
                    playerAnimator.SetLookAtWeight(1);
                }
                if (gun.bodyIKPosition != null) {
                    playerAnimator.bodyRotation = gun.bodyIKPosition.rotation;
                }
                if (gun.leftHandIKPosition != null) {
                    playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, gun.leftHandIKPosition.position);
                    playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, gun.leftHandIKPosition.rotation);
                    playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                }
                if (gun.rightHandIKPosition != null) {
                    playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, gun.rightHandIKPosition.position);
                    playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, gun.rightHandIKPosition.rotation);
                    playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                }
            } else {
                // 取消逆向动力控制
                playerAnimator.SetLookAtWeight(0);
                playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }

    }

}

