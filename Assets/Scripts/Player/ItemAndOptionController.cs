using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class ItemAndOptionController : MonoBehaviour {
        
        public PlayerHealthManager healthManager;

        public GameObject content;

        private PlayerItemController selectedItem;

        private List<PlayerItemController> list;

        private AudioSource audioSource;

        void OnEnable() {
            if (audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }
            GameManager.Instance.Pause();
            if (list == null) {
                GameObject prefab = Resources.Load<GameObject>("UI/PlayerItem");
                list = new List<PlayerItemController>();
                foreach (PlayerItem pi in Archive.CurrentArchive.Player.PlayerItems.Values) {
                    if (pi.Amount <= 0) {
                        continue;
                    }
                    AddPlayerItem(prefab, pi);
                }
            }
            if (Archive.CurrentArchive.Player.PlayerItems.Count > list.Count) {
                GameObject prefab = Resources.Load<GameObject>("UI/PlayerItem");
                foreach (PlayerItem pi in Archive.CurrentArchive.Player.PlayerItems.Values) {
                    if (pi.Amount <= 0) {
                        continue;
                    }
                    bool find = false;
                    for (int i = 0; i < list.Count; i++) {
                        if (pi.ItemId == list[i].ItemId) break;
                        if (i == list.Count - 1) {
                            find = true;
                        }
                    }
                    if(find) {
                        AddPlayerItem(prefab, pi);
                    }
                }
            }
            foreach (PlayerItemController pic in list) {
                pic.Fit();
            }
        }

        private void AddPlayerItem(GameObject prefab, PlayerItem pi) {
            GameObject go = Instantiate(prefab, content.transform);
            PlayerItemController pic = go.AddComponent(Type.GetType(Item.GetItem(pi.ItemId).Script)) as PlayerItemController;
            pic.HoldeItem(pi, this);
            pic.Fit();
            list.Add(pic);
        }

        void Update () {            
            if (Archive.CurrentArchive.Player.PlayerItems.Count == list.Count) {
                for (int i = list.Count - 1; i >= 0; i--) {
                    PlayerItemController pic = list[i];
                    if (pic.Amount <= 0) {
                        list.RemoveAt(i);
                        Destroy(pic.gameObject);
                    }
                }
            }
	    }

        public void SelectItem(PlayerItemController pic) {
            selectedItem = pic;
            foreach(PlayerItemController pic0 in list) {
                pic0.UnSelect();
            }
            selectedItem.Select();
        }

        public void UseItem() {
            if (selectedItem != null) {
                selectedItem.UseItem(audioSource);
            }
        }

        public void SaveAndExit() {
            GameManager.Instance.ShowConfirmWindow(()=> {
                GameManager.Instance.SaveAndExit();
            });
        }

        void OnDisable() {
            foreach (PlayerItemController pic0 in list) {
                pic0.UnSelect();
            }
            selectedItem = null;
            if (GameManager.Instance != null) {
                GameManager.Instance.Resume();
            }
        }

    }

}
