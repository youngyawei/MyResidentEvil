using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using UnityEngine;

using Mono.Data.Sqlite;

namespace MyResidentEvil.Data {

    /**
     * 数据仓库 , 他并不是要去实现一个 Repository 模式 , 目的很简单就是获取持久化的数据和将提交给他的数据持久化
     */
    public class Repository : IDisposable {

        private static string dataFilePath;

        private static string originDataFilePath;

        private static string connectionString;

        // 静态构造函数 , 用于初始化 sqlite 数据文件
        static Repository() {
            dataFilePath = Application.persistentDataPath + "/data.db";
#if UNITY_EDITOR || UNITY_STANDALONE
            originDataFilePath = "file://" + Application.streamingAssetsPath + "/data.db";
            connectionString = "data source=" + dataFilePath;
#elif UNITY_ANDROID
            originDataFilePath = Application.streamingAssetsPath + "/data.db";
            connectionString = "URI=file:" + dataFilePath;
#endif
            if (File.Exists(dataFilePath)) {
                return;
            }
            using (WWW www = new WWW(originDataFilePath)) {
                while (!www.isDone) {

                }
                byte[] data = www.bytes;
                using (FileStream write = File.Create(dataFilePath)) {
                    write.Write(data, 0, data.Length);
                }
            }
        }

        public static void Init() {
            Scene.LoadScenes();
            Item.LoadItem();
        }

        private static Repository instance;

        public static Repository Instance {
            get {
                if (instance == null) {
                    instance = new Repository();
                }
                return instance;
            }
        }

        private SqliteConnection sqliteConnection;

        private Thread thread;

        private AutoResetEvent autoResetEvent;

        private bool run = true;

        private Queue<Command> commandQueue = new Queue<Command>();

        private Repository() {
            sqliteConnection = new SqliteConnection(connectionString);
            sqliteConnection.Open();
            thread = new Thread(Run);
            autoResetEvent = new AutoResetEvent(false);
            thread.Start();
        }

        private void Run() {
            while (run) {
                if (commandQueue.Count == 0) {
                    autoResetEvent.WaitOne(500);
                    if (commandQueue.Count == 0) continue;
                }
                Command cmd = commandQueue.Dequeue();
                cmd.Execute(sqliteConnection);
            }
        }

        public Command Submit(Command cmd) {
            commandQueue.Enqueue(cmd);
            autoResetEvent.Set();
            return cmd;
        }

        public void Close() {
            run = false;
            sqliteConnection.Close();
        }

        public void Dispose() {
            Close();
        }
    }

}
