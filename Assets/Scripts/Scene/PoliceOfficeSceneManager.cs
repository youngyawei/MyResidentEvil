using System.Collections;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class PoliceOfficeSceneManager : BaseSceneManager {

        void Start() {
            Init();
            if (!Archive.CurrentArchive.CurrentSceneData.Entry) {
                Archive.CurrentArchive.CurrentSceneData.Entry = true;
            }
        }

    }

}
