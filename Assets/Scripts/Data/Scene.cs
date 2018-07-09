using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;


namespace MyResidentEvil.Data {

    // 用于表示场景的实体数据
    public class Scene : BaseEntity {

        private static Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();

        public static void LoadScenes(bool reload = false) {
            if (scenes.Count == 0 || reload) {
                Repository.Instance.Submit(
                    new Command("select * from scenes", (dataReader)=> {
                        scenes.Clear();
                        Scene s = null;
                        while (dataReader.Read()) {
                            s = new Scene {
                                sceneId = dataReader.GetString(0),
                                sceneName = dataReader.GetString(1),
                                assetBundle = dataReader.GetString(2),
                                bgm = dataReader.GetString(3),
                                powerSource = dataReader.GetString(4) == "1" ? true : false
                            };
                            scenes.Add(s.sceneId , s);
                        }
                    })
                );
            }
        }

        public static int ScenesSize() {
            return scenes.Count;
        }

        public static Scene GetScene(string sceneId) {
            return scenes[sceneId];
        }

        // 场景 ID - 对应了场景文件名
        private string sceneId;

        // 场景名称
        private string sceneName;

        // 场景对应的 AssetBundle
        private string assetBundle;

        // 背景音乐
        private string bgm;

        // 是否和电源有关 , 如果和电源有关 , 则在加载场景时会根据电源是否打开在场景名称后加上 light 或 dark 的后缀
        private bool powerSource;

        public string SceneId {
            get { return sceneId; }
        }
        public string SceneName {
            get { return sceneName; }
        }
        public string AssetBundle {
            get { return assetBundle; }
        }
        public string Bgm {
            get { return bgm; }
        }
        public bool PowerSource {
            get { return powerSource; }
        }

        public override void Persistence() {
            throw new System.NotImplementedException();
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

        public override bool Equals(object obj) {
            var scene = obj as Scene;
            return scene != null &&
                   sceneId == scene.sceneId;
        }

        public override int GetHashCode() {
            return -930902610 + EqualityComparer<string>.Default.GetHashCode(sceneId);
        }

        public static bool operator ==(Scene scene1, Scene scene2) {
            return EqualityComparer<Scene>.Default.Equals(scene1, scene2);
        }

        public static bool operator !=(Scene scene1, Scene scene2) {
            return !(scene1 == scene2);
        }
    }

}
