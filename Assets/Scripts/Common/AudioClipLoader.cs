using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MyResidentEvil {

    public class AudioClipLoader : MonoBehaviour {

        private Dictionary<string, AudioClipWrapper> audioClips = new Dictionary<string, AudioClipWrapper>();

        private class AudioClipWrapper {
            public AudioClip audioClip;
            public bool loaded;
        }

        private string pathPrefix;

        private static AudioClipLoader instance;

        public static AudioClipLoader Instance {
            get { return instance; }
        }

        void Start () {
#if UNITY_EDITOR || UNITY_STANDALONE
            pathPrefix = "file://" + Application.streamingAssetsPath + "/Audios/";
#elif UNITY_ANDROID
            pathPrefix = Application.streamingAssetsPath + "/Audios/";
#endif
            instance = this;
	    }
        
        // 载入 audioClip 将其设置到 audioSource 上
        public void LoadAudioClip(AudioSource audioSource, string audioClipName) {
            StartCoroutine(GetAudioClip(audioSource, pathPrefix + audioClipName, false));
        }

        // 载入 audioClip 将其设置到 audioSource 上 , 并进行播放
        public void LoadAudioClipAndPlay(AudioSource audioSource, string audioClipName) {
            StartCoroutine(GetAudioClip(audioSource, pathPrefix + audioClipName, true));
        }

        private IEnumerator GetAudioClip(AudioSource audioSource, string path, bool play) {
            if (audioClips.ContainsKey(path)) {
                AudioClipWrapper acw = audioClips[path];
                while (!acw.loaded) {
                    yield return null;
                }
                audioSource.clip = acw.audioClip;
                if (play) {
                    audioSource.Play();
                }
                yield break;
            } else {
                audioClips.Add(path, new AudioClipWrapper {
                    audioClip = null,
                    loaded = false
                });
            }
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, GetAudioType(path))) {
                yield return www.SendWebRequest();
                if (www.isNetworkError) {
                    Debug.Log(www.error);
                } else {
                    AudioClip audio = DownloadHandlerAudioClip.GetContent(www);
                    audioClips[path].audioClip = audio;
                    audioClips[path].loaded = true;
                    audioSource.clip = audio;
                    if (play) {
                        audioSource.Play();
                    }
                }
            }
        }

        private AudioType GetAudioType(string path) {
            string path2 = path.ToLower();
            if (path2.EndsWith(".wav")) {
                return AudioType.WAV;
            } else if (path2.EndsWith(".mp3")) {
                return AudioType.MPEG;
            }
            return AudioType.WAV;
        }

        void OnDestroy() {
            instance = null;
            foreach (AudioClipWrapper ac in audioClips.Values) {
                Destroy(ac.audioClip);
            }
            audioClips.Clear();
        }

    }

}

