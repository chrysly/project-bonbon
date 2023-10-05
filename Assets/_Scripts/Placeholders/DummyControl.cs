using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyControl : MonoBehaviour {

    public void SetCore(int coreIndex) {
        GameManager.Instance.TransitionToCore((GameManager.CoreScene) coreIndex);
    }

    public void SetLevel(int levelIndex) {
        GameManager.Instance.TransitionToLevel(levelIndex);
    }
}
