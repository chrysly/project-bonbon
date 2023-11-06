using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleBonbonWindow : MonoBehaviour {
    public event System.Action<IEnumerator, bool> OnAnimationEndpoint;
    public IEnumerator activeUIAction = null;

    [SerializeField] private Transform icon;
    [SerializeField] private Transform tray1;
    [SerializeField] private Transform tray2;
    [SerializeField] private Transform ingredientsButton;
    [SerializeField] private Transform augmentButton;
    [SerializeField] private Transform consumeButton;
    [SerializeField] private Transform shareButton;
    [SerializeField] private Transform bonbonSelectLocation;

    [SerializeField] private Transform cursor;

    [SerializeField] private float animationDuration = 0.4f;
    [SerializeField] private float animationDelay = 0.05f;

    private BonbonObject[] bonbons;
    [SerializeField] private List<BonbonIcon> bonbonSlots;
    [SerializeField] private List<BattleButton> bonbonOperationButtons;

    public int mainButtonIndex = 0;
    public int bonbonOperationsIndex = 0;

    public bool bonbonOperationEnabled = false;

    // Start is called before the first frame update
    void Start() {
        bonbons = new BonbonObject[4];
        QuickDisable();
    }

    public void QuickDisable() {
        icon.DOScale(0f, 0);
        tray1.DOScale(0f, 0);
        tray2.DOScale(0f, 0);
        ingredientsButton.DOScaleX(0f, 0);
        augmentButton.DOScaleX(0f, 0);
        consumeButton.DOScaleX(0f, 0);
        shareButton.DOScaleX(0f, 0);
        cursor.DOScale(0f, 0);
        ClearButtons();
        mainButtonIndex = -1;
    }

    private void ClearButtons() {
        foreach (BonbonIcon bonbonSlot in bonbonSlots) {
            bonbonSlot.Disable();
        }

        ingredientsButton.GetComponent<MainIngredientButton>().merge = false;
        bonbons = new BonbonObject[4];
        bonbonOperationEnabled = false;
    }

    public void Initialize(BonbonObject[] bonbonArray) {
        for (int i = 0; i < bonbonSlots.Count; i++) {
            bonbons[i] = bonbonArray[i];
        }

        ToggleMainDisplay(true);
    }

    public void ToggleMainDisplay(bool enable) {
        if (activeUIAction == null) {
            if (!enable) CollapseCursor();
            activeUIAction = enable ? EnableAnimation() : DisableAnimation();
            OnAnimationEndpoint?.Invoke(activeUIAction, true);
            StartCoroutine(activeUIAction);
        }
    }

    private IEnumerator EnableAnimation() {
        icon.DOScale(1f, animationDuration);
        yield return new WaitForSeconds(animationDelay);
        tray1.DOScale(1f, animationDuration);
        yield return new WaitForSeconds(animationDelay);
        tray2.DOScale(1f, animationDuration);
        yield return new WaitForSeconds(animationDelay);

        //Bonbon Icons
        for (int i = 0; i < bonbons.Length; i++) {
            if (bonbons[i] != null) {
                bonbonSlots[i].Initialize(bonbons[i]);
                yield return new WaitForSeconds(animationDelay);
            }
        }

        var action = activeUIAction;
        activeUIAction = null;
        BonbonSelect(true);

        OnAnimationEndpoint?.Invoke(action, false);
    }

    private IEnumerator DisableAnimation() {
        cursor.DOScale(0f, 0);
        shareButton.DOScaleX(0f, animationDuration);
        yield return new WaitForSeconds(animationDelay);
        consumeButton.DOScaleX(0f, animationDuration);
        yield return new WaitForSeconds(animationDelay);
        augmentButton.DOScaleX(0f, animationDuration);
        yield return new WaitForSeconds(animationDelay);
        ingredientsButton.DOScaleX(0f, animationDuration);
        yield return new WaitForSeconds(animationDelay);
        icon.DOScale(0f, animationDuration);
        yield return new WaitForSeconds(animationDelay);
        ClearButtons();
        tray1.DOScale(0f, animationDuration);
        yield return new WaitForSeconds(animationDelay);
        tray2.DOScale(0f, animationDuration);
        mainButtonIndex = -1;

        var action = activeUIAction;
        activeUIAction = null;

        OnAnimationEndpoint?.Invoke(action, false);
    }

    public void BonbonSelect(bool directionDown) {
        if (activeUIAction == null) {
            if (!bonbonOperationEnabled) {
                activeUIAction = SelectMainPanelButtonAction(directionDown);
            }
            else {
                activeUIAction = OperationSelect(directionDown);
            }

            OnAnimationEndpoint?.Invoke(activeUIAction, true);
            StartCoroutine(activeUIAction);
        }
    }

    private IEnumerator SelectMainPanelButtonAction(bool directionDown) {
        InitCursor();
        if (mainButtonIndex == -1) mainButtonIndex = 0;
        else if (directionDown) mainButtonIndex = mainButtonIndex >= bonbons.Length - 1 ? 0 : mainButtonIndex + 1;
        else mainButtonIndex = mainButtonIndex <= 0 ? bonbons.Length - 1 : mainButtonIndex - 1;
        Debug.Log(mainButtonIndex + " is cursor index");
        for (int i = 0; i < bonbons.Length; i++) {
            if (i == mainButtonIndex) {
                bonbonSlots[mainButtonIndex].Select();
            }
            else {
                bonbonSlots[mainButtonIndex].Deselect();
            }
        }

        if (bonbons[mainButtonIndex] == null) {
            ToggleEmptySlot();
        }
        else {
            ToggleOccupiedSlot();
        }

        UpdateCursor(bonbonSlots[mainButtonIndex]);

        yield return new WaitForSeconds(animationDelay);

        var action = activeUIAction;
        activeUIAction = null;

        OnAnimationEndpoint?.Invoke(action, false);
    }

    private void ToggleEmptySlot() {
        ingredientsButton.DOScaleX(1f, 0.1f);
        ingredientsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ingredients";
        ingredientsButton.GetComponent<MainIngredientButton>().merge = false;
        augmentButton.DOScaleX(0f, 0.1f);
        consumeButton.DOScaleX(0f, 0.1f);
        shareButton.DOScale(0f, 0.1f);
    }

    private void ToggleOccupiedSlot() {
        ingredientsButton.DOScaleX(1f, 0.1f);
        ingredientsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Bake";
        ingredientsButton.GetComponent<MainIngredientButton>().merge = true;
        augmentButton.DOScaleX(1f, 0.1f);
        consumeButton.DOScaleX(1f, 0.1f);
        shareButton.DOScale(1f, 0.1f);
    }

    private void InitCursor() {
        if (mainButtonIndex == -1) {
            cursor.DOScale(new Vector3(1, 1, 1), 0.07f).SetEase(Ease.Flash);
            cursor.gameObject.SetActive(true);
        }
    }

    private void CollapseCursor() {
        cursor.DOScale(new Vector3(0, 0, 0), 0.07f);
        mainButtonIndex = -1;
    }

    private void UpdateCursor(BattleButton button) {
        Debug.Log("moved cursor");
        cursor.DOMove(button.targetPoint.position, 0.06f);
    }

    public void ToggleBonbonOperations(bool enable) {
        bonbonOperationEnabled = enable;
        if (enable) UpdateCursor(bonbonOperationButtons[bonbonOperationsIndex]);
        else UpdateCursor(bonbonSlots[bonbonOperationsIndex]);
    }

    //BONBON OPERATION METHODS
    private IEnumerator OperationSelect(bool directionDown) {
        if (bonbons[mainButtonIndex] == null) {
            UpdateCursor(bonbonOperationButtons[bonbonOperationsIndex]);
        }
        else {
            if (bonbonOperationsIndex == -1) bonbonOperationsIndex = 0;
            else if (directionDown) bonbonOperationsIndex = bonbonOperationsIndex >= bonbonOperationButtons.Count - 1 ? 0 : bonbonOperationsIndex + 1;
            else bonbonOperationsIndex = bonbonOperationsIndex <= 0 ? bonbonOperationButtons.Count - 1 : bonbonOperationsIndex - 1;
            UpdateCursor(bonbonOperationButtons[bonbonOperationsIndex]);
        }
        yield return new WaitForSeconds(animationDelay);

        var action = activeUIAction;
        activeUIAction = null;

        OnAnimationEndpoint?.Invoke(action, false);
    }

    public BattleButton ConfirmButton() {
        return bonbonOperationButtons[bonbonOperationsIndex];
    }
}
