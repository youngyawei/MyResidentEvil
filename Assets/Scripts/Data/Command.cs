using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;


using Mono.Data.Sqlite;

namespace MyResidentEvil.Data {

    public class Command {

        private string commandText;

        private Action<SqliteDataReader> callback;

        private AutoResetEvent waitHandler;

        private string exceptionMessage;

        private bool success = false;

        public Command(string commandText, Action<SqliteDataReader> callback) {
            this.commandText = commandText;
            this.callback = callback;
            waitHandler = new AutoResetEvent(false);
        }

        public void Execute(SqliteConnection conn) {
            using (SqliteCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = commandText;
                try {
                    if (callback == null) {
                        cmd.ExecuteNonQuery();
                    } else {
                        using (SqliteDataReader sdr = cmd.ExecuteReader()) {
                            callback(sdr);
                        }
                    }
                    success = true;
                } catch(Exception e) {
                    exceptionMessage = e.Message;
                    success = false;
                } finally {
                    waitHandler.Set();
                }
            }
        }
	    
        public bool WaitForExecute() {
            waitHandler.WaitOne();
            return success;
        }

        public string ExceptionMessage {
            get { return exceptionMessage; }
        }

    }


}
