using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    public abstract class EnemySensorController : MonoBehaviour {

        public abstract Transform GetNearByPlayer();

    }

}

