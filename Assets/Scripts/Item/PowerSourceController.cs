using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class PowerSourceController : InteractionItemController {

        void Start () {
            player = GameObject.FindGameObjectWithTag("Player");
        }
	
	    void Update () {
            if (CanInteraction()) {
                Interaction();
            }
        }

        protected override void Interaction() {
            if (!Archive.CurrentArchive.PowerSource) {
                Archive.CurrentArchive.PowerSource = true;
                GameManager.Instance.ShowTips("电源打开了", () => {
                    GameManager.Instance.ShowTips("没必要再开着手电筒了");
                });
            }
        }
    }

}
