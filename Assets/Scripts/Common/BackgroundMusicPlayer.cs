using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class BackgroundMusicPlayer : MonoBehaviour {

        private AudioSource audioSource;

	    void Start () {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
            Scene scene = Scene.GetScene(Archive.CurrentArchive.SceneId);
            AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, scene.Bgm);
        }

        public void PlayBGM(string bgm) {
            AudioClipLoader.Instance.LoadAudioClipAndPlay(audioSource, bgm);
        }

        public void StopPlay() {
            audioSource.Stop();
        }

    }


}

