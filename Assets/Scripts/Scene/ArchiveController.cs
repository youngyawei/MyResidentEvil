using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class ArchiveController : MonoBehaviour, IPointerClickHandler {

        private StartGameArchiveManager archiveManager;

        private Archive archive;

        public Image background;

        public Text createTime;

        public Text playTime;

        public Text sceneName;

        public Color selectColor;

        public Color unSelectColor;

        void Start () {
            background.color = unSelectColor;
	    }

        public void Init(Archive archive, StartGameArchiveManager archiveManager) {
            this.archive = archive;
            createTime.text = archive.CreateTimeToStr;
            playTime.text = archive.PlayTimeToStr;
            sceneName.text = archive.SceneName;
            this.archiveManager = archiveManager;
            if (archive.Finish) {
                background.color = Color.green;
                unSelectColor = Color.green;
            }
        }

        public Archive Archive {
            get { return archive; }
        }

        public void Select() {
            background.color = selectColor;
        }

        public void UnSelect() {
            background.color = unSelectColor;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (archive.Finish) {
                GameManager.Instance.ShowTips("提示：该档案已经完结");
            } else {
                archiveManager.SelectArchive(this);
            }
        }
    }

}
