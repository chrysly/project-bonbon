using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonbonWindow : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;

    [SerializeField] private BakeWindow display;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private BattleStateMachine battleState;

    private List<BonbonBlueprint> _bonbonObjects;
    private List<GameObject> _activeButtons;

    private void Start() {
        _bonbonObjects = new List<BonbonBlueprint>();
        _activeButtons = new List<GameObject>();
    }

    public void ReloadActor(CharacterActor actor) {
        _bonbonObjects = actor.BonbonList;
        LoadButtons();
    }
    
    private void LoadButtons() {
        ClearPreviousButtons();
        foreach (BonbonBlueprint bonbonBlueprint in _bonbonObjects) {
            GameObject button = (GameObject) Instantiate(buttonPrefab, buttonContainer);
            BonbonButton bonbonButton = button.GetComponent<BonbonButton>();
            bonbonButton.AssignBonbon(bonbonBlueprint);
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(delegate { /*battleState.SwitchToTargetSelect(skill);*/ });
            btn.onClick.AddListener(delegate { display.Deactivate(); });
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
