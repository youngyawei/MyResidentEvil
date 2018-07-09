using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class FrontPolicementGateManager : BaseSceneManager {
	
	    void Start () {
            Init();
            if (!Archive.CurrentArchive.CurrentSceneData.Entry) {
                StartCoroutine(ShowTips());
            }
        }

        private IEnumerator ShowTips() {
            yield return new WaitForSeconds(2.0f);
            Archive.CurrentArchive.CurrentSceneData.Entry = true;
            GameManager.Instance.ShowTips("终于到了，队友都牺牲了，现在只剩下我一个人", () => {
                GameManager.Instance.ShowTips("基于我们提供的情报，政府最终决定用核弹将这座城市彻底摧毁\n不过也无所谓了，毕竟这已经是一座死城了", ()=> {
                    GameManager.Instance.ShowTips("印象中有一架直升机停在这里，我必须尽快找到逃离这座城市", () => {
                        GameManager.Instance.ShowTips("提示：打开物品和选项后，使用手电筒即可对手电筒进行开关", () => {
                            GameManager.Instance.ShowTips("提示：敌人被杀死后过一段时间就会复活");
                        });
                    });
                });
            });
        }
	
    }

}
