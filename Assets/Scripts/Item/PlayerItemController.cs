using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public abstract class PlayerItemController : MonoBehaviour, IPointerClickHandler {

        protected Text itemName;

        protected Text amount;

        protected Image background;

        protected PlayerItem playerItem;

        private ItemAndOptionController itemAndOptionController;

        // 使用物品
        public abstract void UseItem(AudioSource audioSource);

        public virtual void HoldeItem(PlayerItem playerItem, ItemAndOptionController itemAndOptionController) {
            this.playerItem = playerItem;
            this.itemAndOptionController = itemAndOptionController;
            background = GetComponent<Image>();
            itemName = transform.Find("Name").gameObject.GetComponent<Text>();
            amount = transform.Find("Amount").gameObject.GetComponent<Text>();
            background.sprite = Resources.Load<Sprite>("Sprites/" + Item.GetItem(playerItem.ItemId).Sprite);
        }

        public virtual void Fit() {
            itemName.text = Item.GetItem(playerItem.ItemId).ItemName;
            amount.text = playerItem.Amount.ToString();
        }

        public virtual void Select() {
            background.color = Color.green;
        }

        public virtual void UnSelect() {
            background.color = Color.white;
        }

        public virtual short Amount {
            get {
                return playerItem.Amount;
            }   
        }

        public string ItemId {
            get {
                return playerItem.ItemId;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            itemAndOptionController.SelectItem(this);
        }

    }

}


