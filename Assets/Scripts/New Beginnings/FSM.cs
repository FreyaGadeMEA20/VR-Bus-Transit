using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement{
    public class FSM : MonoBehaviour
    {

        protected virtual void Initialize() {}
        protected virtual void FSMUpdate() {}
        protected virtual void FSMFixedUpdate() {}
        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            FSMUpdate();
        }

        void FixedUpdate(){
            FSMFixedUpdate();
        }
    }
}
