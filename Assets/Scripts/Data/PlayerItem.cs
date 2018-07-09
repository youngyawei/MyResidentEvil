using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Mono.Data.Sqlite;

namespace MyResidentEvil.Data {

    public class PlayerItem : BaseEntity {

        private string playerId;

        private string itemId;

        private short amount;

        public string PlayerId {
            get {
                return playerId;
            }
        }

        public string ItemId {
            get {
                return itemId;
            }
        }

        public short Amount {
            get {
                return amount;
            }
            set {
                amount = value;
                if (persistenceState == PersistenceState.New) {
                    if (amount <= 0) {
                        persistenceState = PersistenceState.NoChange;
                    }
                } else {
                    if (amount <= 0) {
                        persistenceState = PersistenceState.Delete;
                    } else {
                        persistenceState = PersistenceState.Modify;
                    }
                }
            }
        }

        public static PlayerItem CreatePlayerItem(string playerId, string itemid) {
            return new PlayerItem {playerId = playerId, itemId = itemid, amount = 1, persistenceState = PersistenceState.New };
        }

        public static void LoadPlayerItems(Player player) {
            Repository.Instance.Submit(
                new Command("select * from player_items where player_id = '" + player.PlayerId + "'", (dataReader) => {
                    player.PlayerItems.Clear();
                    PlayerItem pi = null;
                    while (dataReader.Read()) {
                        pi = new PlayerItem {
                            playerId = dataReader.GetString(0),
                            itemId = dataReader.GetString(1),
                            amount = dataReader.GetInt16(2)
                        };
                        player.PlayerItems.Add(pi.itemId, pi);
                    }
                })
            );
        }

        public override void Persistence() {
            string commandText = "";
            switch (persistenceState) {
                case PersistenceState.New:
                    commandText = CreateCommandText();
                    break;
                case PersistenceState.Modify:
                    commandText = UpdateCommandText();
                    break;
                case PersistenceState.Delete:
                    commandText = DeleteCommandText();
                    break;
            }
            if (commandText != "") {
                persistenceState = PersistenceState.NoChange;
                Repository.Instance.Submit(new Command(commandText, null));
            }
        }

        protected override string CreateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append(" insert into player_items values ( ");
            sb.Append("'").Append(playerId).Append("',");
            sb.Append("'").Append(itemId).Append("',");
            sb.Append(amount);
            sb.Append(" ) ");
            return sb.ToString();
        }

        protected override string DeleteCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append("delete player_items where player_id = '").Append(playerId).Append("' ");
            sb.Append(" and item_id = '").Append(itemId).Append("' ");
            return sb.ToString();
        }

        protected override string RetrieveCommandText() {
            throw new System.NotImplementedException();
        }

        protected override string UpdateCommandText() {
            StringBuilder sb = new StringBuilder();
            sb.Append(" update player_items set amount = ").Append(amount).Append(" where ");
            sb.Append(" player_id = '").Append(playerId).Append("' and ");
            sb.Append(" item_id = '").Append(itemId).Append("' ");
            return sb.ToString();
        }

        public override bool Equals(object obj) {
            var item = obj as PlayerItem;
            return item != null &&
                   playerId == item.playerId &&
                   itemId == item.itemId;
        }
        
        public override int GetHashCode() {
            var hashCode = 296272640;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(playerId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(itemId);
            return hashCode;
        }

        public static bool operator ==(PlayerItem item1, PlayerItem item2) {
            return EqualityComparer<PlayerItem>.Default.Equals(item1, item2);
        }

        public static bool operator !=(PlayerItem item1, PlayerItem item2) {
            return !(item1 == item2);
        }
    }

}
