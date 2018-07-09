using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityStandardAssets.CrossPlatformInput;

namespace MyResidentEvil {

    // 可交互的物品控制器
    public abstract class InteractionItemController : MonoBehaviour {

        protected GameObject player;

        // 场景物品 ID
        public string sceneItemId;

        // 物品ID
        public string itemId;

        // 可交互的距离
        public float distance = 1.0f;

        // 可交互的角度
        public float angle = 90.0f;

        protected abstract void Interaction();

        protected virtual bool CanInteraction() {
            if (Vector3.Distance(transform.position, player.transform.position) > distance) {
                return false;
            }
            if (CrossPlatformInputManager.GetButton("J")) {
                return true;
            }
            return false;
        }

    }

}


