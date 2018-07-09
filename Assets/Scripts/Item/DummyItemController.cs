using UnityEngine;

namespace MyResidentEvil {

    public class DummyItemController : PlayerItemController {

        public override void UseItem(AudioSource audioSource) {
            GameManager.Instance.ShowTips("不需要使用");
        }

        
    }

}
