using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class UIAnimationHandler : MonoBehaviour {

    //Everything should pass through activeUIAction! This is to prevent other animations overlapping
    public IEnumerator activeUIAction;
    
    [SerializeField] private BattleUIStateMachine _stateMachine;
    [SerializeField] private Canvas mainCanvas;

    private Dictionary<IEnumerator, Coroutine> coroutineDict;

    void Awake() {
        coroutineDict = new Dictionary<IEnumerator, Coroutine>();
    }

    public void Start() {
        DisableAll();
        cursor.gameObject.SetActive(false);
        skillWindow.OnAnimationEndpoint += ProcessAnimation;
        targetWindow.OnAnimationEndpoint += ProcessAnimation;
        bonbonWindow.OnAnimationEndpoint += ProcessAnimation;
        ingredientWindow.OnAnimationEndpoint += ProcessAnimation;
    }

    public void ProcessAnimation(IEnumerator animation, bool start) {
        if (start) {
            coroutineDict[animation] = StartCoroutine(LockUI());
        } else {
            StopCoroutine(coroutineDict[animation]);
            coroutineDict.Remove(animation);
            if (coroutineDict.Count == 0) _stateMachine.UnlockUI();
        }
    }

    private IEnumerator LockUI() {
        while (true) {
            _stateMachine.LockUI();
            yield return null;
        }
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
    [SerializeField] private float mainPanelToggleDuration = 0.06f;

    [SerializeField] private Transform cursor;

    [SerializeField] private List<BattleButton> mainPanelButtons;
    [SerializeField] private List<Transform> decorations;
    [SerializeField] private Vector3 mainPanelButtonScaleVector = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private float mainPanelButtonScaleDuration = 0.04f;
    [SerializeField] private float mainPanelButtonEmergeDuration = .2f;
    private int mainButtonIndex = -1;

    public void ToggleMainPanel(bool enable, bool force = false) {
        if (QueueIsEmpty()) {
            if (enable) mainPanel.gameObject.SetActive(true);
            activeUIAction = MainPanelAction(enable, force);
            ProcessAnimation(activeUIAction, true);
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

        var action = activeUIAction;
        activeUIAction = null;

        if (!enable) mainPanel.gameObject.SetActive(false);
        else SelectMainPanelButton(true);

        ProcessAnimation(action, false);
    }

    public void SelectMainPanelButton(bool directionDown) {
        if (activeUIAction == null) {
            activeUIAction = SelectMainPanelButtonAction(directionDown);
            ProcessAnimation(activeUIAction, true);
            StartCoroutine(activeUIAction);
        }
    }
    private IEnumerator SelectMainPanelButtonAction(bool directionDown) {
        InitCursor();
        if (mainButtonIndex == -1) mainButtonIndex = 0;
        else if (directionDown) mainButtonIndex = mainButtonIndex >= mainPanelButtons.Count - 1 ? 0 : mainButtonIndex + 1;
        else mainButtonIndex = mainButtonIndex <= 0 ? mainPanelButtons.Count - 1 : mainButtonIndex - 1;
        Debug.Log(mainButtonIndex);
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

        var action = activeUIAction;
        activeUIAction = null;

        ProcessAnimation(action, false);
    }

    private void InitCursor() {
        if (mainButtonIndex == -1) {
            cursor.gameObject.SetActive(true);
            cursor.DOScale(new Vector3(1, 1, 1), mainPanelButtonScaleDuration).SetEase(Ease.Flash);
        }
    }

    private void CollapseCursor() {
        cursor.DOScale(new Vector3(0, 0, 0), mainPanelButtonScaleDuration);
        mainButtonIndex = -1;
    }

    private void UpdateCursor(BattleButton button) {
        cursor.DOMove(button.targetPoint.position, mainPanelButtonScaleDuration);
    }
    
    public void ActivateMainPanelButton() {
        if (mainButtonIndex == -1) return;
        mainPanelButtons[mainButtonIndex].Scale(new Vector3(1, 1, 1), mainPanelButtonScaleDuration);
        mainPanelButtons[mainButtonIndex].Activate(_stateMachine, mainPanelButtonScaleDuration);
    }
    #endregion Main Window
    
    #region Skill Window

    public BattleSkillWindow skillWindow;
    
    #endregion Skill Window

    #region TargetSelect

    public BattleTargetWindow targetWindow;

    #endregion
    
    #region Bonbon Window

    public BattleBonbonWindow bonbonWindow;
    public IngredientSelectWindow ingredientWindow;

    #endregion Bonbon Window
}
