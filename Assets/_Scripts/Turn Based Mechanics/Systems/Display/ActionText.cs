using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class ActionText : MonoBehaviour
{
    private BattleStateMachine stateMachine => BattleStateMachine.Instance;
    [SerializeField] private BonbonHandler bonbonFactory;
    [SerializeField] private float textActiveDuration = 3f;
    [SerializeField] private Transform window;
    [SerializeField] private Transform pivot;
    private IEnumerator _activeDisplay;
    private Vector3 originalPosition;
    
    void Start() {
        stateMachine.OnStateTransition += UpdateActionText;
        bonbonFactory.OnBonbonCreation += UpdateBonbonActionText;
        originalPosition = window.position;
    }

    private void UpdateActionText(BattleStateMachine.BattleState state, BattleStateInput input) {
        if (state is BattleStateMachine.AnimateState) {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            if (input.SkillPrep.bonbon != null) {
                text.SetText(input.ActiveActor().Data.DisplayName + " used " + input.SkillPrep.bonbon.Name + " " + input.SkillPrep.skill.SkillData.name +
                             " on "
                             + input.SkillPrep.targets[0].Data.DisplayName +
                             "!");
            }
            else {
                text.SetText(input.ActiveActor().Data.DisplayName + " used " +
                                 input.SkillPrep.skill.SkillData.name +
                                 " on "
                                 + input.SkillPrep.targets[0].Data.DisplayName +
                                 "!"); //HARD CODED BC IM LAZY AND WE'RE GONNA CHANGE THIS LATER
            }

            window.DOMove(pivot.position, 0.5f);

            ClearText();
        } else if (state is BattleStateMachine.WinState) {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.SetText(@"You won!");
        } else if (state is BattleStateMachine.LoseState) {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.SetText(@"You were defeated!");
        }
    }
    
    private void UpdateBonbonActionText(BonbonObject bonbon) {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.SetText(stateMachine.CurrInput.ActiveActor().Data.DisplayName + " created " + bonbon.Name +
                     "!"); //HARD CODED BC IM LAZY AND WE'RE GONNA CHANGE THIS LATER
        ClearText();
    }

    private void ClearText() {
        if (_activeDisplay != null) {
            StopCoroutine(ClearTextAction(textActiveDuration));
        }
        _activeDisplay = ClearTextAction(textActiveDuration);
        StartCoroutine(ClearTextAction(textActiveDuration));
    }

    private IEnumerator ClearTextAction(float delay) {
        yield return new WaitForSeconds(delay);
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.SetText("");
        window.DOMove(originalPosition, 0.5f);
        yield return null;
    }
}
