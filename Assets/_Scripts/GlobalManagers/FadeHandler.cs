using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeHandler : TransitionHandler {
    [SerializeField] private float transitionRate;
    [SerializeField] private GameObject loadingComponents;

    private CanvasGroup canvasGroup;
    private Image image;

    void Awake() {
        loadingComponents.SetActive(true);
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
    }

    public void Fade(float goTo, bool load, Color? color = null){
        Color colorRes = color ?? Color.black;
        StopAllCoroutines();
        StartCoroutine(_Fade(goTo, load, colorRes));
    }

    public void Fade(float goTo, bool load) => Fade(goTo, load, Color.black);

    IEnumerator _Fade(float goTo, bool load, Color color){
        image.color = color;
        loadingComponents.SetActive(load);
        while (canvasGroup.alpha != goTo){
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, goTo, transitionRate * Time.unscaledDeltaTime);
            yield return null;
        } if (canvasGroup.alpha == 0) loadingComponents.SetActive(false);
    }
}

