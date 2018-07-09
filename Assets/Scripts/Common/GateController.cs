using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyResidentEvil {

    public class GateController : MonoBehaviour {

        public float rotateSpeed = 15.0f;

        private Transform left;

        private Transform right;

        private bool open;

        private bool openGate = false;

        private float t = 0;

        public bool Open {
            get { return open; }
        }

        public void OpenGate() {
            openGate = true;
        }

	    void Start () {
            left = transform.Find("Left");
            right = transform.Find("Right");
        }
	
	    void Update () {
            if (openGate) {
                t += rotateSpeed * Time.deltaTime;
                if (t < 90) {
                    left.localEulerAngles = new Vector3(0, -t, 0);
                    right.localEulerAngles = new Vector3(0, t, 0);
                }
                if (t >= 90) {
                    open = true;
                }
            }
	    }

    }

}
