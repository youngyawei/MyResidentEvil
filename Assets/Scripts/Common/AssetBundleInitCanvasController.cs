using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyResidentEvil {

    public class AssetBundleInitCanvasController : MonoBehaviour {

        public GameObject mask;

        public Text info;

        public Slider slider;

        // 设置进度条的最大值 , 同时决定是否需要 mask 幕布
        public void InitSlider(int max, bool showMask = false) {
            mask.SetActive(showMask);
            slider.gameObject.SetActive(true);
            slider.maxValue = max;
            slider.minValue = 0;
            slider.value = 0;
        }

        public void UpdateInfo(string infoTxt) {
            info.text = infoTxt;
        }

        public void UpdateProgress(int value) {
            slider.value += value;
        }

    }

}
