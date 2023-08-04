using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectAlpha : MonoBehaviour
{
    private CharacterActor parentActor;
    [SerializeField] private Transform renderObject;
    [SerializeField] private SelectManager selector;
    [SerializeField] private float fadeDuration = 0.1f;
    private Color color;

    private IEnumerator activeToggle = null;
    // Start is called before the first frame update
    void Start()
    {
        parentActor = transform.parent.GetComponent<CharacterActor>();
        //color = renderObject.GetComponent<Renderer>().material.color;
        selector.OnSelect += Toggle;
        selector.OnDeselect += Deselect;
    }

    private void Toggle(CharacterActor actor) {
        Fade(actor.Equals(parentActor));
    }

    private void Deselect() {
        Fade(false);
    }

    private void Fade(bool isEnabled) {
        if (activeToggle != null) {
            StopCoroutine(activeToggle);
        }
        
        if (isEnabled) {
            activeToggle = FadeInAction();
        } else {
            activeToggle = FadeOutAction();
        }
        StartCoroutine(activeToggle);
    }

    private IEnumerator FadeInAction() {
        KillActiveTweens();
        renderObject.gameObject.SetActive(true);
        

        yield return new WaitForSeconds(fadeDuration);

        activeToggle = null;
        yield return null;
    }

    private IEnumerator FadeOutAction() {
        KillActiveTweens();
        //material.DOFade(0f, fadeDuration);

        yield return new WaitForSeconds(fadeDuration);

        renderObject.gameObject.SetActive(false);
        activeToggle = null;
        yield return null;
    }

    private void KillActiveTweens() {
        //material.DOKill();
    }
}
