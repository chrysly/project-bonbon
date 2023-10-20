using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneGoBrr : MonoBehaviour {
    void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}