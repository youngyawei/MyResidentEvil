using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    // 1. 用于更新界面上的生命值计量条
    // 2. 增加游戏时间
    // 3. 控制选项和物品面板的显示和隐藏
    public class PlayerCanvasController : MonoBehaviour {

        public Slider healthSlider;

        public Text sceneName;

        public Text playTime;

        private float interval;

        public GameObject itemAndOption;

        private PlayerHealthManager healthManager;

        public int MaxHealth = 100;

        public int MinHealth = 0;

	    void Start () {
            healthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
            healthSlider.minValue = MinHealth;
            healthSlider.maxValue = MaxHealth;
            playTime.text = Archive.CurrentArchive.PlayTimeToStr;
            sceneName.text = Scene.GetScene(Archive.CurrentArchive.SceneId).SceneName;
        }
	
	    void Update () {
            Archive.CurrentArchive.PlayTime += (long)(Time.unscaledDeltaTime * 1000);   // 在 scaleTime = 0 之后也能用 , 即便暂停的时候也在计时
            interval += Time.deltaTime;                                                 // 在 scaleTime = 0 之后就是 0 , 这样在暂停的时候就不会刷时间了
            if (interval > 1.0f) {
                playTime.text = Archive.CurrentArchive.PlayTimeToStr;
                interval = 0;
            }
            healthSlider.value = healthManager.Health;
            if (CrossPlatformInputManager.GetButton("O")) {
                itemAndOption.SetActive(true);
            }
	    }

        public void ShowItemAndOption() {
            itemAndOption.SetActive(true);
        }

        public void HideItemAndOption() {
            itemAndOption.SetActive(false);
        }

    }

}
