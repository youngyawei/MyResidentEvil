using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    using MotionState = HumanoidBaseMotionController.MotionState;
    /*
     * 类人模型的动作控制器基类
     */
    public abstract class HumanoidBaseMotionController : MonoBehaviour {

        // 动作状态
        public enum MotionState {
            Sleeping,                   // 睡觉中
            WakingUp,                   // 唤醒中
            Idling,                     // 空闲中
            Interaction,                // 交互中
            Moving,                     // 移动中
            Agonizing,                  // 挣扎中
            Eluding,                    // 躲闪中
            HitFly,                     // 被击飞
            StandingUp,                 // 从地面爬起站立中
            Attacking,                  // 攻击中
            UnderAttacking,             // 受到攻击中
            SpinKicking,                // 回旋踢状态中
            Shouting,                   // 咆哮中
            Dying,                      // 垂死中
            Dead                        // 死亡
        };

        // 当前所处的动作状态
        protected MotionState currentMotionState = MotionState.Idling;

        public MotionState CurrentMotionState {
            get { return currentMotionState; }
        }

        // 行走速度
        public float walkSpeed = 2.0f;

        // 跑步速度
        public float runSpeed = 4.0f;

        // 移动时的旋转速度
        public float rotateSpeed = 90.0f;

        // 跳起时施加的力
        public float jumpForce = 5.0f;

        protected WasHitFlyInfo wasHitFlyInfo;

        protected struct WasHitFlyInfo {

            private Vector3 position;       // 攻击者的位置

            private float distance;         // 击飞的距离

            public WasHitFlyInfo(Vector3 position, float distance) {
                this.position = position;
                this.distance = distance;
            }

            // 这个位置的 y 方向是和被击飞的目标相同
            public Vector3 Position {
                get { return position; }
            }

            public float Distance {
                get { return distance; }
            }
        }

        // 攻击时的动作
        protected abstract void Attack();

        // 移动和旋转
        protected abstract void MoveAndRotate();

        // 触发受到攻击
        public abstract bool UnderAttackTrigger(Vector3 position);

        // 受到攻击时相应的动作
        protected abstract void UnderAttack();

        public virtual void DeadTrigger() {
            currentMotionState = MotionState.Dying;
        }

        protected abstract void Dead();

        // 触发被击飞
        // position : 攻击者所处的位置
        // distance : 被击飞的距离
        public virtual bool WasHitFlyTrigger(Vector3 position, float distance) {
            wasHitFlyInfo = new WasHitFlyInfo(new Vector3(position.x, transform.position.y, position.z), distance);
            currentMotionState = MotionState.HitFly;
            return true;
        }

    }
}

