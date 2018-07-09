using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class WeaponItemController : PlayerItemController {

        private GunShellController gunShellController;

	    void Start () {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Transform gunshell = player.transform.Find("GunWithCamera").Find("GunShell");
            gunShellController = gunshell.gameObject.GetComponent<GunShellController>();
        }
        
        public override void UseItem(AudioSource audioSource) {
            if (playerItem.ItemId == Archive.CurrentArchive.Player.Weapon) {
                return;
            }
            AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, "reload.mp3");
            gunShellController.UseWeapon(playerItem.ItemId);
            audioSource.Play();
        }

    }

}
