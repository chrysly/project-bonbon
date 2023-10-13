using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class UIAnimationHandler : MonoBehaviour {

    //Everything should pass through activeUIAction! This is to prevent other animations overlapping
    public IEnumerator activeUIAction = null;
    [SerializeField] private BattleUIStateMachine _stateMachine;
    [SerializeField] private Canvas mainCanvas;

    public void Start() {
        DisableAll();
        cursor.gameObject.SetActive(false);
    }

    private void LateUpdate() {
        mainCanvas.transform.LookAt(Camera.main.transform);
    }

    public void DisableAll() {
        ToggleMainPanel(false, true);
    }

    public bool QueueIsEmpty() {
        return activeUIAction == null;
    }
    
    #region Main Window
    [Header("Main Panel")] 
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private float mainPanelToggleDuration = 0.3f;

    [SerializeField] private Transform cursor;

    [SerializeField] private List<BattleButton> mainPanelButtons;
    [SerializeField] private List<Transform> decorations;
    [SerializeField] private Vector3 mainPanelButtonScaleVector = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private float mainPanelButtonScaleDuration = 0.2f;
    [SerializeField] private float mainPanelButtonEmergeDuration = 1f;
    private int mainButtonIndex = -1;

    public void ToggleMainPanel(bool enable, bool force = false) {
        if (QueueIsEmpty()) {
            if (enable) mainPanel.gameObject.SetActive(true);
            activeUIAction = MainPanelAction(enable, force);
            StartCoroutine(activeUIAction);
        }
    }
    private IEnumerator MainPanelAction(bool enable, bool force) {
        //mainPanel.DOFade(enable ? 1 : 0, mainPanelToggleDuration); FADE
        if (!enable) CollapseCursor();
        foreach (BattleButton button in mainPanelButtons) {
            if (enable) {
                button.gameObject.SetActive(true);
                button.transform.DOScale(new Vector3(1, 1, 1), force ? 0 : mainPanelButtonEmergeDuration).SetEase(Ease.InOutBounce);
                if (!force) yield return new WaitForSeconds(mainPanelButtonEmergeDuration);
            }
            else {
                button.transform.DOScale(new Vector3(0, 0, 0), force ? 0 : mainPanelButtonEmergeDuration);
                if (!force) yield return new WaitForSeconds(mainPanelButtonEmergeDuration);
                button.gameObject.SetActive(false);
            }
        }

        foreach (Transform decoration in decorations) {
            if (enable) {
                //decoration.gameObject.SetActive(true);
                decoration.transform.DOScale(new Vector3(1, 1, 1), force ? 0 : mainPanelButtonEmergeDuration).SetEase(Ease.InOutBounce);
                if (!force) yield return new WaitForSeconds(mainPanelButtonEmergeDuration);
            }
            else {
                decoration.transform.DOScale(new Vector3(0, 0, 0), force ? 0 : mainPanelButtonEmergeDuration);
                if (!force) yield return new WaitForSeconds(mainPanelButtonEmergeDuration);
                //decoration.gameObject.SetActive(false);
            }
        }
        if (!enable) mainPanel.gameObject.SetActive(false);
        activeUIAction = null;
        yield return null;
    }

    public void SelectMainPanelButton(bool directionDown) {
        if (activeUIAction == null) {
            activeUIAction = SelectMainPanelButtonAction(directionDown);
            StartCoroutine(activeUIAction);
        }
    }
    private IEnumerator SelectMainPanelButtonAction(bool directionDown) {
        InitCursor();
        mainButtonIndex = mainButtonIndex == -1 ? 0 : mainButtonIndex;
        if (directionDown) mainButtonIndex = mainButtonIndex >= mainPanelButtons.Count - 1 ? 0 : mainButtonIndex + 1;
        else mainButtonIndex = mainButtonIndex <= 0 ? mainPanelButtons.Count - 1 : mainButtonIndex - 1;
        for (int i = 0; i < mainPanelButtons.Count; i++) {
            if (i == mainButtonIndex) {
                mainPanelButtons[mainButtonIndex].Scale(mainPanelButtonScaleVector, mainPanelButtonScaleDuration);
            }
            else {
                mainPanelButtons[i].Scale(new Vector3(1, 1, 1), mainPanelButtonScaleDuration);
            }
        }
        
        UpdateCursor(mainPanelButtons[mainButtonIndex]);

        yield return new WaitForSeconds(mainPanelButtonScaleDuration);
        activeUIAction = null;
        yield return null;
    }

    private void InitCursor() {
        if (mainButtonIndex == -1) {
            cursor.DOScale(new Vector3(1, 1, 1), mainPanelButtonScaleDuration).SetEase(Ease.Flash);
            cursor.gameObject.SetActive(true);
        }
    }

    private void CollapseCursor() {
        cursor.DOScale(new Vector3(0, 0, 0), mainPanelButtonScaleDuration);
    }

    private void UpdateCursor(BattleButton button) {
        cursor.DOMove(button.targetPoint.position, mainPanelButtonScaleDuration);
    }
    
    public void ActivateMainPanelButton() {
        mainPanelButtons[mainButtonIndex].Scale(new Vector3(1, 1, 1), mainPanelButtonScaleDuration);
        mainPanelButtons[mainButtonIndex].Activate(_stateMachine, mainPanelButtonScaleDuration);
    }
    #endregion Main Window
    
    #region Skill Window

    [SerializeField] private CanvasGroup skillPanel;
    [SerializeField] private float skillPanelToggleDuration;
    
    

    #endregion Skill Window

}
