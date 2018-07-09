using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Mono.Data.Sqlite;

namespace MyResidentEvil.Data {

    // 
    public class SceneData : BaseEntity {

        // 存档 ID
        private string archiveId;

        // 场景 ID
        private string sceneId;

        // 表示场景是否已经进入过了
        private bool entry = false;

        private Dictionary<string, SceneItemData> sceneItemsDatas = new Dictionary<string, SceneItemData>();

        public string ArchiveId {
            get {
                return archiveId;
            }
        }

        public string SceneId {
            get {
                return sceneId;
            }
        }

        public bool Entry {
            get {
                return entry;
            }
            set {
                entry = value;
                if (persistenceState != PersistenceState.New) {
                    persistenceState = PersistenceState.Modify;
                }
            }
        }

        public Dictionary<string, SceneItemData> SceneItemsDatas {
            get {
                return sceneItemsDatas;
            }
        }

        public void PickUpItem(string sceneItemId, string itemId) {
            SceneItemData sid = SceneItemData.CreateSceneItemData(this, sceneItemId, itemId);
            sid.PickUp = true;
        }

        public static SceneData CreateSceneData(string archiveId, string sceneId) {
            SceneData sd = new SceneData { archiveId = archiveId, sceneId = sceneId };
            Repository.Instance.Submit(new Command(sd.CreateCommandText(), null));
            return sd;
        }

        public static SceneData LoadSceneData(string archiveId, string sceneId) {
            SceneData sd = new SceneData { archiveId = archiveId, sceneId = sceneId , persistenceState = PersistenceState.New};
            Repository.Instance.Submit(
                new Command(sd.RetrieveCommandText(), (dataReader) => {
                    if (dataReader.Read()) {
                        sd.entry = dataReader.GetString(0) == "1" ? true : false;
                        sd.persistenceState = PersistenceState.NoChange;
                    }
                })
            );
            SceneItemData.LoadSceneItemDatas(sd);
            return sd;
        }

        public override bool Equals(object obj) {
            var data = obj as SceneData;
            return data != null &&
                   archiveId == data.archiveId &&
                   sceneId == data.sceneId;
        }

        public override int GetHashCode() {
            var hashCode = -1280549122;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(archiveId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(sceneId);
            return hashCode;
        }

        public override void Persistence() {
            string commandText = "";
            if (persistenceState == PersistenceState.New) {
                commandText = CreateCommandText();
            } else if (persistenceState == PersistenceState.Modify) {
                commandText = UpdateCommandText();
            }
            if (commandText != "") {
                persistenceState = PersistenceState.NoChange;
                Repository.Instance.Submit(new Command(commandText, null));
            }
            foreach (SceneItemData sid in sceneItemsDatas.Values) {
                sid.Persistence();
            }
        }

        protected override string CreateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into scene_datas values (");
            sb.Append("'").Append(archiveId).Append("',");
            sb.Append("'").Append(sceneId).Append("',");
            sb.Append("'").Append(entry?'1':'0').Append("'");
            sb.Append(")");
            return sb.ToString();
        }

        protected override string DeleteCommandText() {
            throw new NotImplementedException();
        }

        // 只查询 entry 这一个字段
        protected override string RetrieveCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select entry from scene_datas where ");
            sb.Append(" archive_id = '").Append(archiveId).Append("'");
            sb.Append(" and scene_id = '").Append(sceneId).Append("'");
            return sb.ToString();
        }

        protected override string UpdateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append(" update scene_datas set entry = '").Append(entry?'1':'0').Append("' ");
            sb.Append(" where archive_id = '").Append(archiveId).Append("' ");
            sb.Append(" and scene_id = '").Append(sceneId).Append("' ");
            return sb.ToString();
        }

        public static bool operator ==(SceneData data1, SceneData data2) {
            return EqualityComparer<SceneData>.Default.Equals(data1, data2);
        }

        public static bool operator !=(SceneData data1, SceneData data2) {
            return !(data1 == data2);
        }
    }

}

