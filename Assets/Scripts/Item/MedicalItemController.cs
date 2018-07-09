using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class MedicalItemController : PlayerItemController {

        private PlayerHealthManager healthManager;

	    void Start () {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            healthManager = player.GetComponent<PlayerHealthManager>();
        }

        public override void UseItem(AudioSource audioSource) {
            if (playerItem.Amount <= 0) {
                return;
            }
            AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, "collected.wav");
            playerItem.Amount--;
            healthManager.RecoverHealth();
            Fit();
        }

    }

}
