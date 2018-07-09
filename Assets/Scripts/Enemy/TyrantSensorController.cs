using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    public class TyrantSensorController : EnemySensorController {

        private GameObject player;

        void Start() {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        void Update() {
            if (player == null) {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }

        public override Transform GetNearByPlayer() {
            if (player != null) {
                return player.transform;
            }
            return null;
        }
        
    }
}
