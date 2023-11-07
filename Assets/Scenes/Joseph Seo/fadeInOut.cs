using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeInOut : MonoBehaviour
{
    [SerializeField] private float transitionRate=0.03f;
    [SerializeField] public CanvasGroup myCanvasGroup;
    [SerializeField] private GameObject loadingCanvas;
    private float fadeValues;


    public void Fade(float goTo){
        fadeValues = goTo;
        StartCoroutine(_Fade(fadeValues));
    }

    IEnumerator _Fade(float goTo){
        while(myCanvasGroup.alpha!=goTo){
            myCanvasGroup.alpha = Mathf.MoveTowards(myCanvasGroup.alpha,goTo,transitionRate*Time.deltaTime);
            if(myCanvasGroup.alpha==goTo){
                if(myCanvasGroup.alpha<0.5f){
                    loadingCanvas.SetActive(false);
                }
            }
            yield return null;
        }
    }


}

