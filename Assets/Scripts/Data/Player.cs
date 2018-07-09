using System.Collections.Generic;
using System.Text;

using UnityEngine;


namespace MyResidentEvil.Data {

    public class Player : BaseEntity {

        // 玩家所属的存档 ID
        private string playerId;

        // 玩家的生命值
        private float health;

        // 当前使用的武器
        private string weapon;

        // 载入场景时玩家所处的坐标
        private Vector3 position;

        // 载入场景时玩家随 y 轴旋转的角度
        private float rotate;
        
        // 表示手电筒开还是关
        private bool flashLight;

        private Dictionary<string, PlayerItem> playerItems = new Dictionary<string, PlayerItem>();

        public string PlayerId {
            get {
                return playerId;
            }
        }

        public float Health {
            get {
                return health;
            }
            set {
                health = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public string Weapon {
            get {
                return weapon;
            }
            set {
                weapon = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public Vector3 Position {
            get { return position; }
            set {
                position = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public float Rotate {
            get { return rotate; }
            set {
                rotate = value;
                persistenceState = PersistenceState.Modify;
            }
        }
        
        public bool FlashLight {
            get { return flashLight; }
            set {
                flashLight = value;
                persistenceState = PersistenceState.Modify;
            }
        }

        public Dictionary<string, PlayerItem> PlayerItems {
            get {
                return playerItems;
            }
        }

        public void PickUpItem(Item item) {
            PlayerItem pi = null;
            if (playerItems.ContainsKey(item.ItemId)) {
                pi = playerItems[item.ItemId];
                pi.Amount += 1;
            } else {
                pi = PlayerItem.CreatePlayerItem(PlayerId, item.ItemId);
                playerItems.Add(item.ItemId, pi);
            }
        }

        // 为存档创建一个玩家
        public static Player CreatePlayer(string archiveId) {
            Player p = new Player {
                playerId = archiveId,
                health = 100,
                weapon = "HandGun",
                position = new Vector3(14.65f, 0, 19.58f),
                rotate = 0,
                flashLight = true,
                persistenceState = PersistenceState.New
            };
            p.playerItems.Add("HandGun", PlayerItem.CreatePlayerItem(archiveId, "HandGun"));
            p.playerItems.Add("Flashlight", PlayerItem.CreatePlayerItem(archiveId, "Flashlight"));
            p.Persistence();
            return p;
        }

        public static Player LoadPlayer(string archiveId) {
            Player p = new Player { playerId = archiveId };
            Repository.Instance.Submit(
                new Command(p.RetrieveCommandText(), (dataReader)=> {
                    if (dataReader.Read()) {
                        p.health = dataReader.GetFloat(1);
                        p.weapon = dataReader.GetString(2);
                        string[] xyz = dataReader.GetString(3).Split('|');
                        p.position = new Vector3(float.Parse(xyz[0]), float.Parse(xyz[1]), float.Parse(xyz[2]));
                        p.rotate = dataReader.GetFloat(4);
                        p.flashLight = dataReader.GetString(5) == "1" ? true : false;
                    }
                })
            );
            PlayerItem.LoadPlayerItems(p);
            return p;
        }

        protected override string CreateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into players values (");
            sb.Append("'").Append(playerId).Append("',");
            sb.Append(health).Append(",");
            sb.Append("'").Append(weapon).Append("',");
            sb.Append("'").Append(position.x).Append('|').Append(position.y).Append('|').Append(position.z).Append("',");
            sb.Append(rotate).Append(",");
            sb.Append("'").Append(flashLight?'1':'0').Append("'");
            sb.Append(")");
            return sb.ToString();
        }

        protected override string DeleteCommandText() {
            throw new System.NotImplementedException();
        }

        protected override string RetrieveCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from players where player_id = '").Append(playerId).Append("'");
            return sb.ToString();
        }

        protected override string UpdateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append(" update players set ");
            sb.Append(" health = ").Append(health).Append(" , ");
            sb.Append(" weapon = '").Append(weapon).Append("' , ");
            sb.Append(" position = '").Append(position.x).Append('|').Append(position.y).Append('|').Append(position.z).Append("' , ");
            sb.Append(" rotate = ").Append(rotate).Append(", ");
            sb.Append(" flashlight = '").Append(flashLight?'1':'0').Append("'");
            sb.Append(" where player_id = '").Append(playerId).Append("' ");
            return sb.ToString();
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

            List<string> removesKeys = new List<string>();
            foreach (PlayerItem pi in playerItems.Values) {
                if (pi.Amount == 0) {
                    removesKeys.Add(pi.ItemId);
                }
                pi.Persistence();
            }
            foreach (string key in removesKeys) {
                playerItems.Remove(key);
            }
        }

        public override bool Equals(object obj) {
            var player = obj as Player;
            return player != null &&
                   playerId == player.playerId;
        }

        public override int GetHashCode() {
            return -40056923 + EqualityComparer<string>.Default.GetHashCode(playerId);
        }

        public static bool operator ==(Player player1, Player player2) {
            return EqualityComparer<Player>.Default.Equals(player1, player2);
        }

        public static bool operator !=(Player player1, Player player2) {
            return !(player1 == player2);
        }
    }

}

