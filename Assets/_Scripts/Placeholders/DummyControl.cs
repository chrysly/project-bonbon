using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyControl : MonoBehaviour {

    public void SetLevel(int levelIndex) {
        GameManager.Instance.TransitionToLevel(levelIndex);
    }
}
