using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PolyTest : MonoBehaviour {
    

    [Serializable]
    public class Base {
        public int val;
    }

    [Serializable]
    public class DerivedA : Base {
        public string str;

        public void test() {

        }
    }

    [Serializable]
    public class DerivedB : Base {
        public float f;
    }

    [SerializeReference]
    public List<Base> bases;
}
