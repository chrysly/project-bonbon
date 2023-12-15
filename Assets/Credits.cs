using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Credits : MonoBehaviour {
    private CanvasGroup _canvasGroup;
    private bool toggled = false;
     private SpriteRenderer backdrop;
    
    // Start is called before the first frame update
    void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        backdrop = GetComponent<SpriteRenderer>();
        _canvasGroup.alpha = 0;
        backdrop.DOFade(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) {
            if (!toggled) {
                _canvasGroup.DOFade(1, 0.5f);
                backdrop.DOFade(1, 0.5f);
                toggled = true;
            }
            else {
                _canvasGroup.DOFade(0, 0.5f);
                backdrop.DOFade(0, 0.5f);
                toggled = false;
            }
        }
    }
}
