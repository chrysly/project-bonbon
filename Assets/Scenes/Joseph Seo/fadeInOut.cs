using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeInOut : MonoBehaviour
{
    [SerializeField] private float transitionRate=0.03f;
    [SerializeField] public CanvasGroup myCanvasGroup;
    [SerializeField] private GameObject transitionCanvas;
    [SerializeField] private GameObject loadingComponents;
    private float fadeValues;
    private Color _color;


    public void Fade(float goTo,bool load,Color? color=null){
        _color = color ?? Color.black;
        fadeValues = goTo;
        StartCoroutine(_Fade(fadeValues,load,_color));
    }

    public void Fade(float goTo, bool load) => Fade(goTo,load,Color.black);

    IEnumerator _Fade(float goTo,bool load,Color color){
        transitionCanvas.GetComponent<Image>().color=color;
        loadingComponents.SetActive(load);
        while(myCanvasGroup.alpha!=goTo){
            myCanvasGroup.alpha = Mathf.MoveTowards(myCanvasGroup.alpha,goTo,transitionRate*Time.deltaTime);
            if(myCanvasGroup.alpha==goTo){
                if(myCanvasGroup.alpha<0.5f){
                    transitionCanvas.SetActive(false);
                }
            }
            yield return null;
        }
    }


}

