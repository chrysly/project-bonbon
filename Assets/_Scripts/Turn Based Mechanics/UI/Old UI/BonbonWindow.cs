using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonbonWindow : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;

    [SerializeField] private BakeWindow display;
    [SerializeField] private BonbonRadialWindow _radialWindow;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private BattleStateMachine battleState;
    
    private List<GameObject> _activeButtons;

    private void Start() {
        transform.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        _activeButtons = new List<GameObject>();
    }

    public void ReloadActor(CharacterActor actor, int index) {
        LoadButtons(actor, index);
    }
    
    private void LoadButtons(CharacterActor actor, int index) {
        ClearPreviousButtons();
        foreach (BonbonBlueprint bonbonBlueprint in actor.BonbonList) {
            GameObject button = (GameObject) Instantiate(buttonPrefab, buttonContainer);
            BonbonButton bonbonButton = button.GetComponent<BonbonButton>();
            bonbonButton.AssignBonbon(bonbonBlueprint);
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(delegate { display.Deactivate(); });
            btn.onClick.AddListener(delegate { _radialWindow.CloseAll(); });
            btn.onClick.AddListener(delegate { battleState.SwitchToBonbonState(bonbonBlueprint, index, new bool[4]); });
            Debug.Log("Slot Index:" + index);
            _activeButtons.Add(button);
        }
    }

    private void ClearPreviousButtons() {
        foreach (GameObject button in _activeButtons) {
            Destroy(button);
        }
        _activeButtons.Clear();
    }
}
