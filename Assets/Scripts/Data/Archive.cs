using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Mono.Data.Sqlite;

using System.Globalization;

using UnityEngine;

namespace MyResidentEvil.Data {

    // 存档类
    public class Archive : BaseEntity {

        private static Archive currentArchive;

        public static Archive CurrentArchive {
            get { return currentArchive; }
            set { currentArchive = value; }
        }

        // 存档 ID
        private string archiveId;

        // 存档创建的时间
        private DateTime createTime;

        // 当前正在播放的场景
        private string sceneId;

        // 当前已经玩了多长时间
        private long playTime;

        // 表示电源是否已经开启
        private bool powerSource = false;

        // 表示前厅电脑是否已经解锁
        private bool unlock = false;

        // 是否已经完结
        private bool finish = false;

        // 玩家数据
        private Player player;

        public Player Player {
            get {
                return player;
            }
        }

        public string ArchiveId {
            get {
                return archiveId;
            }
        }

        public DateTime CreateTime {
            get {
                return createTime;
            }
        }

        public string SceneId {
            get {
                return sceneId;
            }
            set {
                sceneId = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public long PlayTime {
            get {
                return playTime;
            }

            set {
                playTime = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public bool PowerSource {
            get {
                return powerSource;
            }

            set {
                powerSource = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public bool Unlock {
            get {
                return unlock;
            }

            set {
                unlock = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public bool Finish {
            get { return finish; }
            set {
                finish = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        private SceneData currentSceneData;

        public SceneData CurrentSceneData {
            get {
                return currentSceneData;
            }

            set {
                currentSceneData = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public string CreateTimeToStr {
            get {
                return createTime.ToString(LongDatePattern);
            }
        }

        public string PlayTimeToStr {
            get {
                StringBuilder sb = new StringBuilder();
                long seconds = PlayTime / 1000;
                long minutes = seconds / 60;
                sb.Append(string.Format("{0:0#}", minutes / 60)).Append(':');
                seconds = seconds % 60;
                minutes = minutes % 60;
                sb.Append(string.Format("{0:0#}", minutes)).Append(':').Append(string.Format("{0:0#}", seconds));
                return sb.ToString();
            }
        }

        public string SceneName {
            get {
                return Scene.GetScene(sceneId).SceneName;
            }
        }

        // 创建一个新的存档
        // 1. 创建新存档
        // 2. 创建玩家
        // 3. 创建场景数据
        public static Archive CreateNewArchive() {
            Archive archive = new Archive {
                archiveId = Guid.NewGuid().ToString(),
                sceneId = "FrontPolicementGate",
                createTime = DateTime.Now,
                playTime = 0
            };
            Repository.Instance.Submit(new Command(archive.CreateCommandText(), null));
            archive.player = Player.CreatePlayer(archive.archiveId);
            archive.currentSceneData = SceneData.CreateSceneData(archive.archiveId, archive.sceneId);
            return archive;
        }

        // 载入存档中的玩家和场景数据
        public static void LoadArchiveData(Archive archive) {
            archive.player = Player.LoadPlayer(archive.archiveId);
            archive.currentSceneData = SceneData.LoadSceneData(archive.archiveId, archive.sceneId);
        }

        // 获取所有的存档 , 这个是同步方法
        public static List<Archive> GetAllArchiveSync() {
            List<Archive> list = new List<Archive>();
            Command cmd = Repository.Instance.Submit(
                new Command("select * from archives", (dataReader) => {
                    Archive a;
                    while (dataReader.Read()) {
                        a = new Archive();
                        a.Load(dataReader);
                        list.Add(a);
                    }
                })
            );
            if (!cmd.WaitForExecute()) {
                throw new System.Exception(cmd.ExceptionMessage);
            }
            return list;
        }

        private void Load(SqliteDataReader sqliteDataReader) {
            archiveId = sqliteDataReader.GetString(0);
            // 这个解析日期我真的是醉了
            string[] datetime = sqliteDataReader.GetString(1).Split(' ');
            string[][] tmp = {datetime[0].Split('/'), datetime[1].Split(':')};
            createTime = new DateTime(int.Parse(tmp[0][0]), int.Parse(tmp[0][1]), int.Parse(tmp[0][2]), int.Parse(tmp[1][0]), int.Parse(tmp[1][1]), int.Parse(tmp[1][2]));
            sceneId = sqliteDataReader.GetString(2);
            playTime = sqliteDataReader.GetInt64(3);
            powerSource = sqliteDataReader.GetString(4) == "0" ? false : true;
            unlock = sqliteDataReader.GetString(5) == "0" ? false : true;
            finish = sqliteDataReader.GetString(6) == "0" ? false : true;
        }

        protected override string CreateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into archives (archive_id, scene_id, create_time, play_time, power_source, unlock, finish) values ( ");
            sb.Append("'").Append(archiveId).Append("',");
            sb.Append("'").Append(sceneId).Append("',");
            sb.Append("'").Append(createTime.ToString(LongDatePattern)).Append("',");
            sb.Append(playTime).Append(",");
            sb.Append("'0',");
            sb.Append("'0',");
            sb.Append("'0'");
            sb.Append(")");
            return sb.ToString();
        }

        protected override string UpdateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append(" update archives set ");
            sb.Append(" scene_id = '").Append(sceneId).Append("' , ");
            sb.Append(" play_time = ").Append(playTime).Append(" , ");
            sb.Append(" power_source = '").Append(powerSource?'1':'0').Append("' , ");
            sb.Append(" unlock = '").Append(unlock?'1':'0').Append("', ");
            sb.Append(" finish = '").Append(finish ? '1' : '0').Append("' ");
            sb.Append(" where archive_id = '").Append(archiveId).Append("' ");
            return sb.ToString();
        }

        protected override string RetrieveCommandText() {
            throw new NotImplementedException();
        }

        protected override string DeleteCommandText() {
            throw new NotImplementedException();
        }

        public override void Persistence() {
            if (persistenceState == PersistenceState.Modify) {
                Repository.Instance.Submit(new Command(UpdateCommandText(), null));
            }
            player.Persistence();
            currentSceneData.Persistence();
        }

        public override bool Equals(object obj) {
            var archive = obj as Archive;
            return archive != null &&
                   archiveId == archive.archiveId;
        }

        public override int GetHashCode() {
            return -853481728 + EqualityComparer<string>.Default.GetHashCode(archiveId);
        }

        public static bool operator ==(Archive archive1, Archive archive2) {
            return EqualityComparer<Archive>.Default.Equals(archive1, archive2);
        }

        public static bool operator !=(Archive archive1, Archive archive2) {
            return !(archive1 == archive2);
        }

    }

}

