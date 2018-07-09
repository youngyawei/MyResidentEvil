using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class PlayerHealthManager : MonoBehaviour {

        private PlayerMotionController motionController;

        private Player player;

	    void Start () {
            motionController = GetComponent<PlayerMotionController>();
            player = Archive.CurrentArchive.Player;
	    }
	
        public void RecoverHealth() {
            player.Health = 100;
        }

        public void WasHitFly(Vector3 position, float distance, float damage) {
            if (motionController.WasHitFlyTrigger(position, distance)) {
                player.Health -= damage;
                if (player.Health <= 0) {
                    motionController.DeadTrigger();
                    player.Health = 0;
                }
            }
        }

        public void ReduceHealth(Vector3 position, float damage) {
            if (motionController.UnderAttackTrigger(position)) {
                player.Health -= damage;
                if (player.Health <= 0) {
                    motionController.DeadTrigger();
                    player.Health = 0;
                }
            }
        }

        public bool IsAlive() {
            return player.Health > 0;
        }

        public float Health {
            get { return player.Health; }
        }

    }

}
