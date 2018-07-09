using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil.Test {

    public class TestData : MonoBehaviour {

        public string sceneId;

	    void Start () {
            Repository.Init();
            List<Archive> archives = Archive.GetAllArchiveSync();
            if(archives.Count == 0) {
                Debug.Log(" 没有档案数据 ");
                return;
            }
            Archive archive = archives[0];
            Archive.CurrentArchive = archive;
            archive.SceneId = sceneId;
            Archive.LoadArchiveData(archive);
	    }
        
    }

}
