using System.Globalization;
using System;
using Mono.Data.Sqlite;

namespace MyResidentEvil.Data {

    public abstract class BaseEntity {

        public enum PersistenceState {
            NoChange,                       // 没有变化
            New,                            // 新建
            Modify,                         // 被修改
            Delete                          // 删除
        }

        // 根据这个状态决定调用 Persistence 方法时执行的操作
        protected PersistenceState persistenceState = PersistenceState.NoChange;

        protected static DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo {
            LongDatePattern = "yyyy/MM/dd HH:mm:dd",
            ShortDatePattern = "yyyy/MM/dd",
            DateSeparator = "/",
            TimeSeparator = ":"
        };

        protected static string LongDatePattern = "yyyy/M/d H:m:d";

        protected static string ShortDatePattern = "yyyy/M/d";

        public abstract void Persistence();

        // 生成新建实体的 sql 语句
        protected abstract string CreateCommandText();

        // 生成更新实体的 sql 语句
        protected abstract string UpdateCommandText();

        // 生成查询实体的 sql 语句
        protected abstract string RetrieveCommandText();

        // 生成删除实体的 sql 语句
        protected abstract string DeleteCommandText();

    }

}
