using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    public class LaptopController : InteractionItemController {

        void Start () {
            player = GameObject.FindGameObjectWithTag("Player");
        }
	
	    void Update () {
            if (CanInteraction()) {
                Interaction();
            }
        }

        protected override void Interaction() {
            GameManager.Instance.ShowTips("屏幕上显示着 3154 这四个数字");
        }
    }

}
