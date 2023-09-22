using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine.UI;


public class BonbonRadialWindow : MonoBehaviour {
    [SerializeField] private float rotateDuration = 0.5f;
    [SerializeField] private float rotateAngle = 60f;
    [SerializeField] private Actor actor;
    [SerializeField] private BattleStateMachine stateMachine;

    [SerializeField] private CanvasGroup mainCanvas;
    [SerializeField] private List<BonbonDisplay> slots;
    [SerializeField] private Transform mainSelectLocation;
    [SerializeField] private BakeWindow bakeWindow;

    [SerializeField] private Texture defaultSprite;
    [SerializeField] private RawImage output;
    [SerializeField] private LineRenderer line;

    private CanvasGroup _canvasGroup;
    private int _index = 0;
    private int _combineIndex;
    
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
        line.positionCount = 3;
        Hide();
        HideOutput();
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
        UpdateSlots();
        _selectState = BonbonSelectState.Display;
    }

    public void UpdateSlots() {
        BonbonObject[] inventory = actor.BonbonInventory;
        for (int i = 0; i < slots.Count; i++) {
            if (inventory[i] == null) {
                slots[i].UpdateSprite(defaultSprite);
            } else {
                slots[i].UpdateSprite(inventory[i].Texture);
            }
        }
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
            
            case BonbonSelectState.Options:
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    _selectState = BonbonSelectState.Display;
                    bakeWindow.Deactivate();
                    ResetSlots(false);
                } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    if (HasAvailableIngredients()) {
                        _selectState = BonbonSelectState.Combine;
                        ShowOutput();
                        Debug.Log("bruh");
                    }
                }

                break;
            case BonbonSelectState.Combine:
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    _selectState = BonbonSelectState.Display;
                    HideLine();
                    HideOutput();
                } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    if (_combineIndex < slots.Count - 1) {
                        _combineIndex++;
                        if (_index == _combineIndex) _combineIndex++;
                        if (_combineIndex > slots.Count - 1) _combineIndex = 0;
                    }
                    else {
                        _combineIndex = 0;
                        if (_index == 0) {
                            _combineIndex++;
                        }
                    }
                    Debug.Log("Showing" + _combineIndex);
                    CombineSelect();
                } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    if (_combineIndex > 0) {
                        _combineIndex--;
                        if (_index == _combineIndex) _combineIndex--;
                        if (_combineIndex < 0) _combineIndex = slots.Count - 1;
                    }
                    else {
                        _combineIndex = slots.Count - 1;
                        if (_index == slots.Count - 1) {
                            _combineIndex--;
                        }
                    }
                    Debug.Log("Showing" + _combineIndex);
                    CombineSelect();
                } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    if (actor.BonbonInventory[_combineIndex] != null) {
                        BonbonFactory factory = stateMachine.CurrInput.BonbonFactory;
                        BonbonBlueprint[] blueprints =
                            { actor.BonbonInventory[_combineIndex].Data, actor.BonbonInventory[_index].Data, null, null };
                        List<BonbonBlueprint> blueprint = factory.FindRecipes(blueprints);
                        if (blueprint != null) {
                            actor.BonbonInventory[_index] = null;
                            actor.BonbonInventory[_combineIndex] = null;
                            stateMachine.SwitchToBonbonState(blueprint[0], _combineIndex, new bool[4]);
                            HideLine();
                            HideOutput();
                            CloseAll();
                        }
                    }
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

    #region Combine

    private bool HasAvailableIngredients() {
        int i = 0;
        foreach (BonbonObject bonbon in actor.BonbonInventory) {
            if (bonbon != null && i != _index) {
                _combineIndex = i;
                return true;
            }
            i++;
        }
        return false;
    }
    
    private BonbonBlueprint CombineSelect() {
        Vector3 outputPos = output.transform.position;
        for (int i = 0; i < slots.Count; i++) {
            if (i != _combineIndex && i != _index) {
                slots[i].Hide();
            }
            else {
                slots[i].Show();
            }
        }
        
        if (actor.BonbonInventory[_combineIndex] != null) {
            BonbonFactory factory = stateMachine.CurrInput.BonbonFactory;
            BonbonBlueprint[] blueprints =
                { actor.BonbonInventory[_combineIndex].Data, actor.BonbonInventory[_index].Data, null, null };
            List<BonbonBlueprint> blueprint = factory.FindRecipes(blueprints);
            if (blueprint != null) {
                Debug.Log("showing thing");
                output.texture = blueprint[0].texture;
                DrawLine(slots[_combineIndex].transform.position, slots[_index].transform.position,
                    output.transform.position);
                return blueprint[0];
            }
            else {
                Debug.Log("not valid recipe");
            }
        }
        else {
            HideLine();
            output.texture = defaultSprite;
            return null;
        }

        return null;
    }

    private void ShowOutput() {
        output.transform.DOScale(new Vector3(3, 1.5f, 1), rotateDuration);
    }

    private void HideOutput() {
        output.texture = defaultSprite;
        output.transform.DOScale(0, rotateDuration);
    }

    private void DrawLine(Vector3 pos1, Vector3 pos2, Vector3 output) {
        
        line.SetPosition(0, pos1);
        line.SetPosition(1, pos2);
        line.SetPosition(2, output);
        line.gameObject.SetActive(true);
    }

    private void HideLine() {
        line.gameObject.SetActive(false);
        for (int i = 0; i < 3; i++) {
            line.SetPosition(i, new Vector3(0, 0, 0));
        }
    }
    
    

    private void CombineIncrement() {
        while (slots[_combineIndex].bonbon == null) {
            if (_combineIndex < slots.Count - 1) {
                _combineIndex++;
            }
            else {
                _combineIndex = 0;
            }
        }
    }
    
    private void CombineDecrement() {
        while (slots[_combineIndex].bonbon == null) {
            if (_combineIndex > 0) {
                _combineIndex--;
            }
            else {
                _combineIndex = slots.Count - 1;
            }
        }
    }
    
    #endregion Combine
    public void SelectedSlot() {
        if (actor.BonbonInventory[_index] == null) {
            _selectState = BonbonSelectState.Craft;
            bakeWindow.Activate((CharacterActor) actor, _index);
            MainSelect();
        } else {
            _selectState = BonbonSelectState.Options;
            MainSelect();
        }
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

    public void CloseAll() {
        slots[_index].InstantReset();
        StartCoroutine(CloseAction());
    }

    public IEnumerator CloseAction() {
        foreach (BonbonDisplay display in slots) {
            display.Show();
        }
        yield return new WaitForSeconds(0.3f);
        _canvasGroup.DOFade(0, rotateDuration);
        transform.DOScale(0f, rotateDuration);
    }
}
