using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class FlashlightItemController : PlayerItemController {

        private Light flashLight;

	    void Start () {
            flashLight = GetComponent<Light>();
        }

	    void Update () {
            if (flashLight == null) {
                return;
            }
            if (Archive.CurrentArchive.Player.Health <= 0) {
                flashLight.intensity = 0;
            } else {
                if (Archive.CurrentArchive.Player.FlashLight) {
                    flashLight.range = 30;
                    flashLight.intensity = 3;
                } else {
                    flashLight.intensity = 0;
                }
            }
	    }

        public override void UseItem(AudioSource audioSource) {
            AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, "Open Door.mp3");
            Archive.CurrentArchive.Player.FlashLight = !Archive.CurrentArchive.Player.FlashLight;
        }

    }

}
