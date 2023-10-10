using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCaller : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.T)) DialogueManager.dialogueRequestEvent.Invoke("TestingThisCoolThingie") ;
    }
}
