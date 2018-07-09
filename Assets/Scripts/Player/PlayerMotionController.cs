using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MyResidentEvil {

    // 玩家动作控制器
    public class PlayerMotionController : HumanoidBaseMotionController {

        private GameObject playerCanvas;

        private GameObject playerModel;

        private GameObject cameraAndGunShell;

        private Animator playerAnimator;

        private AudioSource footStepAudioSource;

        private GameObject gunShell;

        private Transform flashLight;
        private Vector3 resetflashLight;

        // 在移动时最大转身角度
        public float moveAngle = 30.0f;

        // 控制瞄准时的最大角度
        public float upAngle = 25.0f;
        public float downAngle = -45.0f;

        // 瞄准时的旋转速度
        public float aimRotateSpeed = 45.0f;

        // 翻滚的最大距离
        private sbyte rollDistance = 4;

        private Rigidbody rigidBody;

        void Start () {
            playerCanvas = Instantiate(Resources.Load<GameObject>("UI/PlayerCanvas"));

            playerModel = transform.Find("SWAT_Female").gameObject;
            cameraAndGunShell = transform.Find("GunWithCamera").gameObject;
            gunShell = cameraAndGunShell.transform.Find("GunShell").gameObject;

            flashLight = playerModel.transform.Find("Tops").Find("Flashlight");
            resetflashLight = flashLight.localEulerAngles;

            playerAnimator = playerModel.GetComponent<Animator>();
            playerAnimator.applyRootMotion = false;
            rigidBody = GetComponent<Rigidbody>();
            footStepAudioSource = GetComponent<AudioSource>();
            AudioClipLoader.Instance.LoadAudioClip(footStepAudioSource, "Footstep.wav");
        }

        void Update() {
            // 首先判断游戏是否正在进行中
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play) {
                return;
            }
            // 判断玩家是否存活
            if (currentMotionState == MotionState.Dead) {
                return;
            }

            if (currentMotionState == MotionState.Idling || currentMotionState == MotionState.Attacking) {
                if (CrossPlatformInputManager.GetButtonUp("L")) {
                    if (currentMotionState == MotionState.Idling) {
                        currentMotionState = MotionState.Attacking;
                    } else {
                        currentMotionState = MotionState.Idling;
                        ResetMove();
                        ResetAimAngle();
                    }
                }
            }

            switch (currentMotionState) {
                case MotionState.Idling:
                case MotionState.Moving:
                    MoveAndRotate();
                    break;
                case MotionState.Interaction:
                    Interaction();
                    break;
                case MotionState.Attacking:
                    Attack();
                    break;
                case MotionState.UnderAttacking:
                    UnderAttack();
                    break;
                case MotionState.SpinKicking:
                    SpinKick();
                    break;
                case MotionState.Dying:
                    Dead();
                    break;
            }
        }

        void FixedUpdate() {
            switch (currentMotionState) {
                case MotionState.Idling:
                case MotionState.Moving:
                case MotionState.Attacking:
                case MotionState.Eluding:
                    Elude();                    // 执行躲闪
                    break;
                case MotionState.HitFly:
                case MotionState.StandingUp:
                    WasHitFly();                // 被击飞
                    break;
            }
        }

        private Vector3 EludeLocation;

        // 控制玩家躲闪
        private void Elude() {
            // 如果正在躲闪中 , 则等待躲闪状态结束后将状态设置为 Idling
            if (currentMotionState == MotionState.Eluding) {
                AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("BaseLayer.Roll")) {
                    if (stateInfo.normalizedTime > 0.25f && stateInfo.normalizedTime < 0.3f) {
                        if (!footStepAudioSource.isPlaying) footStepAudioSource.Play();
                    } else if (stateInfo.normalizedTime > 0.75f && stateInfo.normalizedTime < 0.8f) {
                        if (!footStepAudioSource.isPlaying) footStepAudioSource.Play();
                    } else if (stateInfo.normalizedTime > 0.9f) {
                        // 设定状态为空闲
                        currentMotionState = MotionState.Idling;
                        playerAnimator.SetBool("Roll", false);
                        // 重置模型的方向
                        playerModel.transform.localEulerAngles = Vector3.zero;
                        gunShell.SetActive(true);
                    }
                    transform.position = Vector3.Lerp(transform.position, EludeLocation, Time.deltaTime);
                }
                return;
            }

            // 按下按钮 I 躲闪
            if (CrossPlatformInputManager.GetButton("I")) {
                currentMotionState = MotionState.Eluding;
                playerAnimator.SetBool("Roll", true);
                ResetMove();                                    // 重置移动状态
                ResetAimAngle();                                // 重设下瞄准角度
                GetEludeLocation();                             // 获取翻滚的方向和地点
                playerAnimator.SetFloat("Run", 0);
                playerAnimator.SetFloat("Rotate", 0);
                gunShell.SetActive(false);
            }
        }

        // 获得翻滚的方向和地点
        private void GetEludeLocation() {
            // 躲闪默认是向前翻滚
            float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");

            // 优先判断左右翻滚
            if (horizontal != 0.0f && Mathf.Abs(horizontal) >= 0.5f) {
                if(horizontal > 0) {
                    // 向右翻滚
                    EludeLocation = transform.right * rollDistance + transform.position;
                    playerModel.transform.Rotate(Vector3.up * 90);
                } else {
                    // 向左翻滚
                    EludeLocation = transform.right * -rollDistance + transform.position;
                    playerModel.transform.Rotate(Vector3.up * -90);
                }
                return;
            }
            if (vertical < 0.0f) {
               // 向后翻滚
                EludeLocation = transform.forward * -rollDistance + transform.position;
                playerModel.transform.Rotate(Vector3.up * 180);
                return;
            }
            // 默认向前翻滚
            EludeLocation = transform.forward * rollDistance + transform.position;

        }

        private float currentXRotation = 0.0f;
        
        protected override void Attack() {
            float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");
            float mouseX = CrossPlatformInputManager.GetAxisRaw("Mouse X");
            float mouseY = CrossPlatformInputManager.GetAxisRaw("Mouse Y");
            if (mouseY != 0.0f) {
                currentXRotation += mouseY;
                if (currentXRotation >= downAngle && currentXRotation <= upAngle) {
                    cameraAndGunShell.transform.Rotate(Vector3.left * mouseY * aimRotateSpeed * Time.deltaTime);
                    // 瞄准时 , 同时调整手电筒
                    flashLight.Rotate(Vector3.left * mouseY * aimRotateSpeed * Time.deltaTime);
                } else {
                    currentXRotation -= mouseY;
                }
            }
            if (mouseX != 0.0f) {
                transform.Rotate(0, mouseX * aimRotateSpeed * Time.deltaTime, 0);
            }
            if (horizontal != 0 || vertical != 0) {
                transform.Translate((Vector3.forward * vertical + Vector3.right * horizontal) * walkSpeed * Time.deltaTime);
                playerAnimator.SetBool("Move", true);
                playerAnimator.SetFloat("Run", -1);
                AnimatorStateInfo animState = playerAnimator.GetCurrentAnimatorStateInfo(0);
                if (!animState.IsName("BaseLayer.Move")) {
                    return;
                }
                float normalizedTime = animState.normalizedTime - Mathf.Floor(animState.normalizedTime);
                if (normalizedTime > 0.2f && normalizedTime < 0.25f) {
                    if (!footStepAudioSource.isPlaying) footStepAudioSource.Play();
                } else if (normalizedTime > 0.7f && normalizedTime < 0.75f) {
                    if (!footStepAudioSource.isPlaying) footStepAudioSource.Play();
                }
            } else {
                ResetMove();
            }
        }
        
        // 重设瞄准角度
        private void ResetAimAngle() {
            cameraAndGunShell.transform.transform.localEulerAngles = Vector3.zero;
            currentXRotation = 0.0f;
            flashLight.localEulerAngles = resetflashLight;
        }

        // 回旋踢触发器 , 用于通知玩家可以进行回旋踢
        public bool SpinKickTrigger() {
            if (currentMotionState != MotionState.Idling) {
                return false;
            }
            currentMotionState = MotionState.SpinKicking;
            return true;
        }

        private void SpinKick() {
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("BaseLayer.Kick")) {
                // 回旋踢动画播放接近完成时
                if (stateInfo.normalizedTime > 0.9f) {
                    currentMotionState = MotionState.Idling;
                    playerAnimator.SetBool("Kick", false);
                    gunShell.SetActive(true);
                    playerModel.transform.localEulerAngles = Vector3.zero;
                    playerAnimator.applyRootMotion = false;
                }
                playerModel.transform.localPosition = Vector3.zero;
            } else {
                // 播放回旋踢的动画
                playerAnimator.SetBool("Kick", true);
                playerAnimator.applyRootMotion = true;
                // 将枪隐藏
                gunShell.SetActive(false);
            }
        }

        protected override void MoveAndRotate() {
            float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");

            float mouseX = CrossPlatformInputManager.GetAxisRaw("Mouse X");
            transform.Rotate(Vector3.up * mouseX * rotateSpeed * Time.deltaTime);

            if (horizontal != 0.0f || vertical != 0.0f) {

                float angle = vertical != 0 ? Mathf.Atan(Mathf.Abs(horizontal/vertical)) * Mathf.Rad2Deg : moveAngle;
                if (angle > moveAngle) angle = moveAngle;
                if (vertical >= 0) {
                    if (horizontal < 0) {
                        angle = -angle;
                    }
                } else {
                    if (horizontal > 0) {
                        angle = -angle;
                    }
                }
                playerModel.transform.localEulerAngles = new Vector3(0, angle, 0);

                currentMotionState = MotionState.Moving;
                playerAnimator.SetBool("Move", true);
                if (vertical >= 0) {
                    transform.Translate((Vector3.forward * vertical + Vector3.right * horizontal) * runSpeed * Time.deltaTime);
                    playerAnimator.SetFloat("Run", 1);
                } else {
                    transform.Translate((Vector3.forward * vertical + Vector3.right * horizontal) * walkSpeed * Time.deltaTime);
                    playerAnimator.SetFloat("Run", -1);
                }
                // 原本是设置为 vertical 的值 , 但是在 vertical 值较小的时候动画效果很差
                //playerAnimator.SetFloat("Run", vertical);

                AnimatorStateInfo animState = playerAnimator.GetCurrentAnimatorStateInfo(0);
                if (!animState.IsName("BaseLayer.Move")) {
                    return;
                }
                float normalizedTime = animState.normalizedTime - Mathf.Floor(animState.normalizedTime);

                if (vertical > 0) {
                    if (normalizedTime > 0.4f && normalizedTime < 0.45f) {
                        footStepAudioSource.Play();
                    } else if (normalizedTime > 0.90f && normalizedTime < 0.95f) {
                        footStepAudioSource.Play();
                    }
                } else {
                    if (normalizedTime > 0.2f && normalizedTime < 0.25f) {
                        if (!footStepAudioSource.isPlaying) footStepAudioSource.Play();
                    } else if (normalizedTime > 0.7f && normalizedTime < 0.75f) {
                        if (!footStepAudioSource.isPlaying) footStepAudioSource.Play();
                    }
                }
            } else {
                currentMotionState = MotionState.Idling;
                ResetMove();
            }
        }

        private void ResetMove() {
            playerAnimator.SetBool("Move", false);
            playerAnimator.SetFloat("Run", 0);
            playerAnimator.SetFloat("Rotate", 0);
            playerModel.transform.localEulerAngles = Vector3.zero;
        }

        public override bool UnderAttackTrigger(Vector3 position) {
            switch (currentMotionState) {
                case MotionState.Eluding:
                case MotionState.SpinKicking:
                case MotionState.HitFly:
                case MotionState.StandingUp:
                case MotionState.Dead:
                    return false;
            }
            ResetMove();
            ResetAimAngle();
            currentMotionState = MotionState.UnderAttacking;
            return true;
        }

        protected override void UnderAttack() {
            AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.ReactionHit")) {
                if (state.normalizedTime > 0.9f) {
                    currentMotionState = MotionState.Idling;
                    playerAnimator.SetBool("ReactionHit", false);
                    gunShell.SetActive(true);
                }
            } else {
                playerAnimator.SetBool("ReactionHit", true);
                gunShell.SetActive(false);
            }
        }

        // 被击飞后的落地点
        private Vector3 hitFlyDestination;

        public override bool WasHitFlyTrigger(Vector3 position, float distance) {
            switch (currentMotionState) {
                case MotionState.Eluding:
                case MotionState.HitFly:
                case MotionState.StandingUp:
                case MotionState.Dead:
                    return false;
            }
            ResetMove();
            ResetAimAngle();
            base.WasHitFlyTrigger(position, distance);
            return true;
        }

        // 被击飞
        private void WasHitFly() {
            
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("BaseLayer.StandUp")) {
                if (stateInfo.normalizedTime > 0.9f) {
                    currentMotionState = MotionState.Idling;
                    transform.LookAt(wasHitFlyInfo.Position);
                    playerAnimator.SetBool("HitFly", false);
                    gunShell.SetActive(true);
                }
            }

            if (currentMotionState == MotionState.HitFly) {
                Vector3 direction = transform.position - wasHitFlyInfo.Position;
                direction.Normalize();

                rigidBody.AddForce(direction * wasHitFlyInfo.Distance, ForceMode.VelocityChange);

                transform.LookAt(wasHitFlyInfo.Position);
                // 将枪隐藏
                gunShell.SetActive(false);
                currentMotionState = MotionState.StandingUp;
                playerAnimator.SetBool("HitFly", true);
            }
        }

        public override void DeadTrigger() {
            base.DeadTrigger();
            // 结束游戏
            StartCoroutine(GameOver());
        }

        private IEnumerator GameOver() {
            yield return new WaitForSeconds(3.0f);
            GameManager.Instance.GameFail();
        }

        protected override void Dead() {
            playerAnimator.SetTrigger("Die");
            currentMotionState = MotionState.Dead;
            gunShell.SetActive(false);
        }


        public bool InteractionTrigger() {
            if (currentMotionState == MotionState.Idling) {
                currentMotionState = MotionState.Interaction;
                return true;
            }
            return false;
        }

        // 和物品交互
        private void Interaction() {
            AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.Interaction")) {
                if (state.normalizedTime > 0.9) {
                    playerAnimator.SetBool("Interaction", false);
                    currentMotionState = MotionState.Idling;
                }
            } else {
                playerAnimator.SetBool("Interaction", true);
            }
        }

        void OnDestroy() {
            Destroy(playerCanvas);
        }

    }

}
