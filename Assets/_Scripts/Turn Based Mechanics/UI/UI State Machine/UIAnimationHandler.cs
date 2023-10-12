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

    public void Start() {
        DisableAll();
    }

    public void DisableAll() {
        ToggleMainPanel(false);
    }

    public bool QueueIsEmpty() {
        return activeUIAction == null;
    }
    
    #region Main Window
    [Header("Main Panel")] 
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private float mainPanelToggleDuration = 0.3f;

    [SerializeField] private List<BattleButton> mainPanelButtons;
    [SerializeField] private Vector3 mainPanelButtonScaleVector = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private float mainPanelButtonScaleDuration = 0.2f;
    [SerializeField] private float mainPanelButtonEmergeDuration = 1f;
    private int mainButtonIndex = -1;

    public void ToggleMainPanel(bool enable) {
        if (QueueIsEmpty()) {
            if (enable) mainPanel.gameObject.SetActive(true);
            activeUIAction = MainPanelAction(enable);
            StartCoroutine(activeUIAction);
        }
    }
    private IEnumerator MainPanelAction(bool enable) {
        //mainPanel.DOFade(enable ? 1 : 0, mainPanelToggleDuration); FADE
        foreach (BattleButton button in mainPanelButtons) {
            if (enable) {
                button.gameObject.SetActive(true);
                button.transform.DOScale(new Vector3(1, 1, 1), mainPanelButtonEmergeDuration);
                yield return new WaitForSeconds(mainPanelButtonEmergeDuration);
            }
            else {
                button.transform.DOScale(new Vector3(0, 0, 0), mainPanelButtonEmergeDuration);
                yield return new WaitForSeconds(mainPanelButtonEmergeDuration);
                button.gameObject.SetActive(false);
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

        yield return new WaitForSeconds(mainPanelButtonScaleDuration);
        activeUIAction = null;
        yield return null;
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