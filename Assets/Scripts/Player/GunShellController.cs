using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    // 根据玩家数据记录为玩家装配枪械
    public class GunShellController : MonoBehaviour {

        public GameObject player;

        private GameObject gun;

        private GunController gunController;

        void Start() {
            FitWeapon(Archive.CurrentArchive.Player.Weapon);
        }

        private void FitWeapon(string weaponId) {
            Item item = Item.GetItem(weaponId);

            string assetBundleName = item.AssetBundle;
            string prefabName = item.Prefab;
            AssetBundleManager.Instance.LoadAssetBundleAsnyc(assetBundleName, () => {
                AssetBundleManager.Instance.LoadAssetAsync(assetBundleName, prefabName, (prefab) => {
                    if (gun != null) {
                        gunController = null;
                        Destroy(gun);
                    }
                    gun = Instantiate(prefab, transform);
                    gunController = gun.GetComponent<GunController>();
                    gunController.Fit();
                });
            });
        }

        public void UseWeapon(string weaponId) {
            if(Archive.CurrentArchive.Player.Weapon != weaponId) {
                Archive.CurrentArchive.Player.Weapon = weaponId;
                FitWeapon(weaponId);
            }
        }

        public GunController CurrentGunController {
            get {
                return gunController;
            }
        }

        public bool Fire() {
            return gunController.Fire();
        }

        public float GunShot {
            get {
                return gunController.gunShot;
            }
        }

        public float GunDamage {
            get {
                return gunController.damage;
            }
        }

    }

}
