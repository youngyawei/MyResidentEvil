using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyResidentEvil {

    public class FadeoutMaskController : MonoBehaviour {

        public Image mask;

        public Color target;

        private float count = -1.0f;

        void Start() {
            count = 0;
        }

        void Update () {
            if (count < 0) {
                return;
            }
            mask.color = Color.Lerp(mask.color, target, 0.01f);
            count += 0.02f;
            if (count >= 2.0f) {
                Destroy(gameObject);
            }
	    }
    }

}
