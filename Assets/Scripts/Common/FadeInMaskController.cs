using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MyResidentEvil {

    public class FadeInMaskController : MonoBehaviour {

        public Image mask;

        public Color target;

        void Start() {
            Instantiate(Resources.Load<GameObject>("UI/LoadingTextCanvas"));
        }

        void Update () {
            mask.color = Color.Lerp(mask.color, target, Time.unscaledDeltaTime);
	    }
    }

}
