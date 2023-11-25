using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeHandler : TransitionHandler {
    [SerializeField] private float transitionRate;
    [SerializeField] private CanvasGroup loadingComponents;

    private CanvasGroup canvasGroup;
    private Image image;
    private bool lastLoadSetting;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
    }

    public void Fade(float goTo) => Fade(goTo, lastLoadSetting);

    public void Fade(float goTo, bool load, Color? color = null){
        Color colorRes = color ?? Color.black;
        StopAllCoroutines();
        StartCoroutine(_Fade(goTo, load, colorRes));
    }

    public void Fade(float goTo, bool load) => Fade(goTo, load, Color.black);

    IEnumerator _Fade(float goTo, bool load, Color color){
        image.color = color;
        loadingComponents.gameObject.SetActive(load);
        while (canvasGroup.alpha != goTo){
            if (load) loadingComponents.alpha = Mathf.MoveTowards(loadingComponents.alpha, goTo, transitionRate * Mathf.Min(0.1f, Time.unscaledDeltaTime) * 4f);
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, goTo, transitionRate * Mathf.Min(0.1f, Time.unscaledDeltaTime));
            yield return null;
        } if (canvasGroup.alpha == 0) loadingComponents.gameObject.SetActive(false);
    }
}

