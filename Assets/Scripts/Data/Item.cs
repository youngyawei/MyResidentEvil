using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

namespace MyResidentEvil.Data {

    public class Item : BaseEntity {

        private static Dictionary<string, Item> items = new Dictionary<string, Item>();

        public static void LoadItem(bool reload = false) {
            if (items.Count == 0 || reload) {
                Repository.Instance.Submit(
                    new Command("select * from items", (dataReader) => {
                        items.Clear();
                        Item i = null;
                        while (dataReader.Read()) {
                            i = new Item {
                                itemId = dataReader.GetString(0),
                                itemName = dataReader.GetString(1),
                                sprite = dataReader.GetString(2),
                                prefab = dataReader.GetString(3),
                                assetBundle = dataReader.GetString(4),
                                script = dataReader.GetString(5)
                            };
                            items.Add(i.itemId, i);
                        }
                    })
                );
            }
        }

        public static int ItemSize() {
            return items.Count;
        }

        public static Item GetItem(string itemId) {
            return items[itemId];
        }

        // 物品 ID
        private string itemId;

        // 物品名称
        private string itemName;

        // 物品图标
        private string sprite;

        // 物品的预制件
        private string prefab;

        // assetbundle
        private string assetBundle;

        // 脚本类名
        private string script;

        public string ItemId {
            get {
                return itemId;
            }
        }

        public string ItemName {
            get {
                return itemName;
            }
        }

        public string Sprite {
            get {
                return sprite;
            }
        }

        public string Prefab {
            get {
                return prefab;
            }
        }

        public string AssetBundle {
            get { return assetBundle; }
        }

        public string Script {
            get { return script; }
        }

        protected override string CreateCommandText() {
            throw new System.NotImplementedException();
        }

        protected override string DeleteCommandText() {
            throw new System.NotImplementedException();
        }

        protected override string RetrieveCommandText() {
            throw new System.NotImplementedException();
        }

        protected override string UpdateCommandText() {
            throw new System.NotImplementedException();
        }

        public override void Persistence() {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object obj) {
            var item = obj as Item;
            return item != null &&
                   itemId == item.itemId;
        }

        public override int GetHashCode() {
            return 1284995219 + EqualityComparer<string>.Default.GetHashCode(itemId);
        }

        public static bool operator ==(Item item1, Item item2) {
            return EqualityComparer<Item>.Default.Equals(item1, item2);
        }

        public static bool operator !=(Item item1, Item item2) {
            return !(item1 == item2);
        }
    }

}
