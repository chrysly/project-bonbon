using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BonbonWindow : MonoBehaviour
{
    [SerializeField] private CharacterActor actor;
    [SerializeField] private Transform buttonContainer;

    [SerializeField] private BonbonRadialWindow radialWindow;

    [SerializeField] private GameObject buttonPrefab;

    //TODO: Remove after refactoring selection script
    [SerializeField] private CanvasGroup panel;

    private List<BonbonObject> _bonbonObjects;

    private void LoadButtons() {
        foreach (BonbonObject bonbonObject in _bonbonObjects) {
            GameObject button = (GameObject) Instantiate(buttonPrefab, buttonContainer);
            SkillButton skillButton = button.GetComponent<SkillButton>();
            skillButton.AssignSkill(skill);
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(delegate { battleState.SwitchToTargetSelect(skill); });
            btn.onClick.AddListener(delegate { Hide(); });
        }
    }
}
