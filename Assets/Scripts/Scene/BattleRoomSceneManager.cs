﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyResidentEvil.Data;

namespace MyResidentEvil {

    public class BattleRoomSceneManager : BaseSceneManager {

        void Start() {
            Init();
            if (!Archive.CurrentArchive.CurrentSceneData.Entry) {
                Archive.CurrentArchive.CurrentSceneData.Entry = true;
            }
        }

    }

}
