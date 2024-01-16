using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

namespace BattleUI {
    public class ActionText : ScreenSpaceElement {

        [SerializeField] private float textActiveDuration = 3f;
        [SerializeField] private Transform window;
        [SerializeField] private Transform pivot;
        private IEnumerator _activeDisplay;
        private Vector3 originalPosition;

        public override void Init(ScreenSpaceHandler handler) {
            base.Init(handler);
            handler.Input.SkillHandler.OnSkillTrigger += UpdateActionText;
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