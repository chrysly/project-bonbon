using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class fadeInOut : MonoBehaviour
{
    [SerializeField] private float transitionRate=3f;
    [SerializeField] public CanvasGroup myCanvasGroup;
    private bool fadeIn=false;
    private bool fadeOut=false;

    // Start is called before the first frame update
    void Start(){
        FadeOut();
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeIn==true){
            if(myCanvasGroup.alpha<1){
                myCanvasGroup.alpha = Mathf.MoveTowards(myCanvasGroup.alpha,1,transitionRate*Time.deltaTime);
                if(myCanvasGroup.alpha>=1){
                    fadeIn = false;
                }
            }
        }

        if(fadeOut==true){
            if(myCanvasGroup.alpha>=0){
                myCanvasGroup.alpha = Mathf.MoveTowards(myCanvasGroup.alpha,0,transitionRate*Time.deltaTime);
                if(myCanvasGroup.alpha<=0){
                    fadeOut = false;
                }
            }
        }
    }

    public void FadeIn(){
        fadeIn=true;
        //StartCoroutine(ffadeIn());
    }

    public void FadeOut(){
        fadeOut=true;
        //Debug.Log("yuh");
        //StartCoroutine(ffadeOut());
    }
    // IEnumerator ffadeIn(){
    //     while(fadeIn==true){
    //         if(myCanvasGroup.alpha<1){
    //             myCanvasGroup.alpha = Mathf.MoveTowards(myCanvasGroup.alpha,1,transitionRate*Time.deltaTime);
    //             if(myCanvasGroup.alpha>=1){
    //                 fadeIn = false;
    //             }
    //         }
    //         yield return null;
    //     }
    // }

    // IEnumerator ffadeOut(){
    //     while(fadeOut==true){
    //         if(myCanvasGroup.alpha>=0){
    //             myCanvasGroup.alpha = Mathf.MoveTowards(myCanvasGroup.alpha,0,transitionRate*Time.deltaTime);
    //             if(myCanvasGroup.alpha<=0){
    //                 fadeOut = false;
    //             }
    //         }
    //         yield return null;
    //     }
    // }


}

