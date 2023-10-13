using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    // fcking buttons AAAAAAAAAAA
    public KeyCode _key;
    private Button _button;

    void Awake() {
        _button = GetComponent<Button>();
    }

    void Update() { 
        if (Input.GetKeyDown(_key)) { 
            _button.onClick.Invoke();
        }
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Hide() {
        gameObject.SetActive(false);
    }
}
