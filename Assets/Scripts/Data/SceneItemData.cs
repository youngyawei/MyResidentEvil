using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;

namespace MyResidentEvil.Data {

    public class SceneItemData : BaseEntity {

        private string archiveId;

        private string sceneId;

        private string sceneItemId;

        private string itemId;

        private bool pickUp;

        public string ArchiveId {
            get {
                return archiveId;
            }
        }

        public string SceneId {
            get { return sceneId; }
        }

        public string SceneItemId {
            get {
                return sceneItemId;
            }
        }

        public string ItemId {
            get { return itemId; }
        }

        public bool PickUp {
            get {
                return pickUp;
            }
            set {
                pickUp = value;
                if (persistenceState != PersistenceState.New) {
                    persistenceState = PersistenceState.Modify;
                }
            }
        }

        public static SceneItemData CreateSceneItemData(SceneData sd, string sceneItemId, string itemId) {
            SceneItemData sid = new SceneItemData {
                archiveId = sd.ArchiveId,
                sceneId = sd.SceneId,
                sceneItemId = sceneItemId,
                itemId = itemId,
                pickUp = false,
                persistenceState = PersistenceState.New
            };
            sd.SceneItemsDatas.Add(sceneItemId, sid);
            return sid;
        }

        public static void LoadSceneItemDatas(SceneData sd) {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from scene_item_datas where archive_id = '").Append(sd.ArchiveId).Append("' and scene_id = '").Append(sd.SceneId).Append("'");
            Repository.Instance.Submit(
                new Command(sb.ToString(), (dataReader) => {
                    sd.SceneItemsDatas.Clear();
                    SceneItemData sid;
                    while (dataReader.Read()) {
                        sid = new SceneItemData {
                            archiveId = dataReader.GetString(0),
                            sceneId = dataReader.GetString(1),
                            sceneItemId = dataReader.GetString(2),
                            itemId = dataReader.GetString(3),
                            pickUp = dataReader.GetString(4) == "1" ? true : false
                        };
                        sd.SceneItemsDatas.Add(sid.SceneItemId, sid);
                    }
                })
            );

        }

        public override void Persistence() {
            string commandText = "";
            if (persistenceState == PersistenceState.New) {
                commandText = CreateCommandText();
            } else if (persistenceState == PersistenceState.Modify) {
                commandText = UpdateCommandText();
            }
            if (commandText != "") {
                Repository.Instance.Submit(new Command(commandText, null));
                persistenceState = PersistenceState.NoChange;
            }
        }

        protected override string CreateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into scene_item_datas values(");
            sb.Append("'").Append(archiveId).Append("',");
            sb.Append("'").Append(sceneId).Append("',");
            sb.Append("'").Append(sceneItemId).Append("',");
            sb.Append("'").Append(itemId).Append("',");
            sb.Append("'").Append(pickUp?'1':'0').Append("'");
            sb.Append(")");
            return sb.ToString();
        }

        protected override string DeleteCommandText() {
            throw new System.NotImplementedException();
        }

        protected override string RetrieveCommandText() {
            throw new System.NotImplementedException();
        }

        protected override string UpdateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append("update scene_item_datas set pick_up = ");
            sb.Append("'").Append(pickUp?'1':'0').Append("'");
            sb.Append(" where archive_id = '").Append(archiveId).Append("' ");
            sb.Append(" and scene_id = '").Append(sceneId).Append("' ");
            sb.Append(" and scene_item_id = '").Append(sceneItemId).Append("' ");
            return sb.ToString();
        }
    }

}
