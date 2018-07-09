using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class PickUpItemController : InteractionItemController {

        void Start () {
            if (Archive.CurrentArchive.CurrentSceneData.SceneItemsDatas.ContainsKey(sceneItemId)) {
                Destroy(gameObject);
                return;
            }
            player = GameObject.FindGameObjectWithTag("Player");
        }
	
	    void Update () {
            if (CanInteraction()) {
                Interaction();
            }
	    }

        // 捡起物品
        protected override void Interaction() {
            Item item = Item.GetItem(itemId);
            GameManager.Instance.ShowTips(item.ItemName + " 捡起来了");
            Archive.CurrentArchive.Player.PickUpItem(item);
            Archive.CurrentArchive.CurrentSceneData.PickUpItem(sceneItemId, itemId);
            Destroy(gameObject);
        }

    }

}
