using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyResidentEvil {

    public class LoadingTextController : MonoBehaviour {

        public Text text;

        private float loading = 0;

	    void Update () {
            loading += Time.deltaTime;
            if (loading < 2.0f) {
                text.text = "Loading ..";
            } else if (loading > 2.0f && loading < 4.0f) {
                text.text = "Loading ....";
            } else if (loading > 4.0f && loading < 6.0f) {
                text.text = "Loading ...";
            } else {
                loading = 0;
            }
        }
    }

}
