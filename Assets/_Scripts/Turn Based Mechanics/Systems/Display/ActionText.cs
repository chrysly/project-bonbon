using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

namespace BattleUI {
    public class ActionText : ScreenSpaceElement {
        private BattleStateMachine stateMachine => BattleStateMachine.Instance;

        [SerializeField] private float textActiveDuration = 3f;
        [SerializeField] private Transform window;
        [SerializeField] private Transform pivot;
        private IEnumerator _activeDisplay;
        private Vector3 originalPosition;

        public override void Init(ScreenSpaceHandler handler) {
            base.Init(handler);
            stateMachine.CurrInput.SkillHandler.OnSkillTrigger += UpdateActionText;
            ///stateMachine.CurrInput.BonbonHandler.OnBonbonCreation += UpdateBonbonActionText;
            originalPosition = window.position;
        }

        private void UpdateActionText(ActiveSkillPrep skillPrep) {

            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.SetText((skillPrep.skill.Caster.Data is EnemyData ? "The " : "") + $"{skillPrep.skill.Caster.Data.DisplayName} used " +
                         $"{skillPrep.skill.SkillData.Name} on " + (skillPrep.targets[0].Data is EnemyData ? "the " : "") + $"{skillPrep.targets[0].Data.DisplayName}!");

            window.DOMove(pivot.position, 0.5f);
            ClearText();
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
}