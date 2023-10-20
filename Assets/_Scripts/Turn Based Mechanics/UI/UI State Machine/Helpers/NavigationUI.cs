using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class NavigationUI : MonoBehaviour {
    private bool _activeUIAction = false;
    private Queue<IEnumerator> animationQueue;
    
    [SerializeField] private List<BattleButton> buttonList;
    [SerializeField] private Transform cursor;
    [SerializeField] private float animationDuration;

    private int _activeIndex = 0;

    private void Start() {
        animationQueue = new Queue<IEnumerator>();
    }

    #region Main Panel Animaations
    public void ToggleMainPanel(bool enable, bool force) {
        animationQueue.Enqueue(MainPanelAction(enable, force));
        if (!_activeUIAction) {
            StartCoroutine(animationQueue.Dequeue());
            _activeUIAction = true;
        }
    }

    private IEnumerator MainPanelAction(bool enable, bool force) {
        if (!enable) CollapseCursor();
        foreach (BattleButton button in buttonList) {
            if (enable) {
                button.gameObject.SetActive(true);
                button.transform.DOScale(new Vector3(1, 1, 1), force ? 0 : animationDuration).SetEase(Ease.InOutBounce);
                if (!force) yield return new WaitForSeconds(animationDuration);
            }
            else {
                button.transform.DOScale(new Vector3(0, 0, 0), force ? 0 : animationDuration);
                if (!force) yield return new WaitForSeconds(animationDuration);
                button.gameObject.SetActive(false);
            }
        }
        
        if (animationQueue.Count != 0) {
            StartCoroutine(animationQueue.Dequeue());
        }
        else {
            _activeUIAction = false;
        }
        yield return null;
    }
    #endregion Main Panel Animations
    
    #region Cursor Animations

    private void InitCursor() {
        cursor.gameObject.SetActive(true);
        cursor.DOScale(1f, 0.2f).SetEase(Ease.Flash);
    }

    private void CollapseCursor() {
        cursor.DOScale(0f, 0.2f).SetEase(Ease.Flash);
        cursor.gameObject.SetActive(false);
    }

    private void UpdateCursor(BattleButton button) {
        cursor.DOMove(button.targetPoint.position, 0.2f);
    }
    #endregion Cursor Animations
}
