using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionText : MonoBehaviour
{
    [SerializeField] private BattleStateMachine stateMachine;
    [SerializeField] private float textActiveDuration = 3f;
    private IEnumerator _activeDisplay;
    void Start() {
        stateMachine.OnStateTransition += UpdateActionText;
    }

    private void UpdateActionText(BattleStateMachine.BattleState state, BattleStateInput input) {
        if (state is not BattleStateMachine.AnimateState) return;
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.SetText(input.ActiveActor().data.DisplayName() + " used " + input.SkillPrep.skill.ToString() + " on "
                     + input.SkillPrep.targets[0].data.DisplayName() + "!");      //HARD CODED BC IM LAZY AND WE'RE GONNA CHANGE THIS LATER
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
        yield return null;
    }
}
