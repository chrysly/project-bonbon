using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterWindow : MonoBehaviour
{
    [Header("Linked Objects")]
    [SerializeField] private SelectManager selector;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private CanvasGroup healthbar;
    [SerializeField] private CharacterActor actor;

    [Header("Attributes")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float healthbarScale = 0.43f;

    private IEnumerator activeToggle = null;

    private void Start() {
        RegisterEvents();
    }

    private void RegisterEvents() {
        selector.OnSelect += Select;
        selector.OnDeselect += Deselect;
    }

    private void Select(CharacterActor selectedActor) {
        if (selectedActor.Equals(actor)) {   //ID Based Comparison
            ToggleCanvas(true);
        } else {
            Deselect();
        }
    }

    private void Deselect() {
        ToggleCanvas(false);
    }

    private void ToggleCanvas(bool setActive) {
        if (setActive) {
            EnableGroup();
        } else {
            DisableGroup();
        }
    }

    private void EnableGroup() {
        if (activeToggle != null) {
            StopCoroutine(activeToggle);
        }
        activeToggle = EnableCanvas();
        StartCoroutine(activeToggle);
    }

    private void DisableGroup() {
        if (activeToggle != null) {
            StopCoroutine(activeToggle);
        }
        activeToggle = DisableCanvas();
        StartCoroutine(activeToggle);
    }

    private IEnumerator EnableCanvas() {
        KillActiveTweens();
        canvas.gameObject.SetActive(true);
        canvas.DOFade(1f, fadeDuration);
        healthbar.transform.DOScaleX(0f, fadeDuration);

        yield return new WaitForSeconds(fadeDuration);
        healthbar.gameObject.SetActive(false);

        activeToggle = null;
        yield return null;
    }

    private IEnumerator DisableCanvas() {
        KillActiveTweens();
        canvas.DOFade(0f, fadeDuration);
        healthbar.gameObject.SetActive(true);
        healthbar.transform.DOScaleX(healthbarScale, fadeDuration);

        yield return new WaitForSeconds(fadeDuration);
        canvas.gameObject.SetActive(false);

        activeToggle = null;
        yield return null;
    }

    private void KillActiveTweens() {
        canvas.DOKill();
        healthbar.DOKill();
    }
}
