using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour
{
    [SerializeField] private float expandDuration = 0.5f;
    [SerializeField] private float contractDuration = 0.5f;
    [SerializeField] private float buttonLoadDuration;
    [SerializeField] private CharacterActor actor;
    [SerializeField] private Transform buttonContainer;

    [SerializeField] private BattleStateMachine battleState;

    [SerializeField] private GameObject buttonPrefab;

    //TODO: Remove after refactoring selection script
    [SerializeField] private CanvasGroup panel;

    private bool _initialized = false;

    private List<SkillAction> skills;

    private void Start() {
        transform.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        battleState.OnStateTransition += Redisplay;
    }

    public void Display() {
        if (!_initialized) {
            skills = new List<SkillAction>(actor.SkillList);
            LoadButtons();
            _initialized = true;
        }

        gameObject.SetActive(true);
        transform.DOScaleY(40, expandDuration);
    }

    public void DisplayPanel() {
        panel.DOFade(1, contractDuration);
    }

    public void Redisplay(BattleStateMachine.BattleState state, BattleStateInput input) {
        if (state is BattleStateMachine.TurnState) {
            if (input.ActiveActor().Equals(actor)) {
                DisplayPanel();
            }
        }
    }

    private void DisplayOnConfirm(bool canceled) {
        panel.alpha = 1;
    }

    public void Hide() {
        StartCoroutine(HideAction());
    }

    private IEnumerator HideAction() {
        transform.DOScaleY(0, contractDuration);
        panel.DOFade(0, contractDuration);

        yield return new WaitForSeconds(contractDuration);

        gameObject.SetActive(false);
    }

    private void LoadButtons() {
        foreach (SkillAction skill in skills) {
            GameObject button = (GameObject) Instantiate(buttonPrefab, buttonContainer);
            SkillButton skillButton = button.GetComponent<SkillButton>();
            skillButton.AssignSkill(skill);
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(delegate { battleState.SwitchToTargetSelect(skill); });
            btn.onClick.AddListener(delegate { Hide(); });
        }
    }
}
