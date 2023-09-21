using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class BonbonRadialWindow : MonoBehaviour {
    [SerializeField] private float rotateDuration = 0.5f;
    [SerializeField] private float rotateAngle = 60f;
    [SerializeField] private Actor actor;
    [SerializeField] private BattleStateMachine stateMachine;

    [SerializeField] private CanvasGroup mainCanvas;
    [SerializeField] private List<BonbonDisplay> slots;
    [SerializeField] private Transform mainSelectLocation;
    [SerializeField] private BakeWindow bakeWindow;

    private CanvasGroup _canvasGroup;
    private int _index = 0;
    public enum BonbonSelectState {
        Display,
        Select,
        Options,
        Use,
        Craft,
        Combine,
        Pass
    }

    private BonbonSelectState _selectState = BonbonSelectState.Display;
    
    private void Start() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.DOFade(0, 0);
        Hide();
    }

    #region RadialWindow
    public void Hide() {
        _canvasGroup.DOFade(0, rotateDuration);
        transform.DOScale(0f, rotateDuration);
        mainCanvas.DOFade(1, rotateDuration);
    }

    public void Display() {
        _canvasGroup.DOFade(1, rotateDuration);
        transform.DOScale(1, rotateDuration);
        mainCanvas.DOFade(0, rotateDuration / 2);
        _selectState = BonbonSelectState.Display;
    }
    
    #endregion RadialWindow
    public void Update() {

        switch (_selectState) {
            case BonbonSelectState.Display:
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
                    SwitchToSelect();
                } else if (Input.GetKeyDown(KeyCode.Escape)) {
                    ResetSlots(true);
                }
                break;
            case BonbonSelectState.Select:
                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    if (_index < slots.Count - 1) {
                        _index++;
                    }
                    else {
                        _index = 0;
                    }
                    Select();
                } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    if (_index > 0) {
                        _index--;
                    }
                    else {
                        _index = slots.Count - 1;
                    }
                    Select();
                } else if (Input.GetKeyDown(KeyCode.Escape)) {
                    ResetSlots(true);
                } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    SelectedSlot();
                }
                break;
            case BonbonSelectState.Craft:
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    _selectState = BonbonSelectState.Display;
                    bakeWindow.Deactivate();
                    ResetSlots(false);
                }

                break;
        }
    }

    #region Select
    public void SwitchToSelect() {
        slots[_index].Select();
        _selectState = BonbonSelectState.Select;
    }

    public void Select() {
        for (int i = 0; i < slots.Count; i++) {
            if (i != _index) {
                slots[i].Deselect();
            }
            else {
                slots[i].Select();
            }
        }
    }
    #endregion Select

    public void SelectedSlot() {
        if (slots[_index].bonbon == null) {
            _selectState = BonbonSelectState.Craft;
            bakeWindow.Activate();
        }
        MainSelect();
    }

    public void MainSelect() {
        for (int i = 0; i < slots.Count; i++) {
            if (i == _index) {
                slots[_index].MainSelect(mainSelectLocation.position);
            }
            else {
                slots[i].Hide();
            }
        }
    }

    public void ResetSlots(bool hide) {
        slots[_index].QuickReset();
        StartCoroutine(ResetAction(hide));
    }

    public IEnumerator ResetAction(bool hide) {
        foreach (BonbonDisplay display in slots) {
            display.Show();
        }
        yield return new WaitForSeconds(0.2f);
        if (hide) Hide();
        _index = 0;
        yield return null;
    }
}
