using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MyResidentEvil {

    using MotionState = HumanoidBaseMotionController.MotionState;

    public abstract class EnemyHealthManager : MonoBehaviour {
        
        // 受到攻击
        public abstract void UnderAttack(Vector3 position, Vector3 shotPoint, float damage);

        // 被回旋踢踢到
        public abstract void SufferSpinKick(Vector3 position, float distance, float damage);

        // 是否能被回旋踢攻击
        protected bool SpinKickAble(Transform player, MotionState enemyState) {
            if (enemyState != MotionState.Agonizing) {
                return false;
            }
            if(Vector3.Distance(transform.position, player.position) > PlayerSpinKick.spinKickRadius) {
                return false;
            }
            return true;
        }


    }

}

