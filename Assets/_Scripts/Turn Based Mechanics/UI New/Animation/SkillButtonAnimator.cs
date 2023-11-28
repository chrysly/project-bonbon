using System.Collections;
using System.Collections.Generic;
using BattleUI;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BattleUI {
    public class SkillButtonAnimator : UIButtonAnimator {
        [SerializeField] private float selectDuration = 0.3f;
        protected override void Awake() {
            base.Awake();
        }

        public override void Toggle(bool toggle) {
            base.Toggle(toggle);
            if (!toggle) return;
            SkillSelectButton skillButton = Button as SkillSelectButton;
            GetComponent<SkillNameIdentifier>().GetComponent<TextMeshProUGUI>().text =
                skillButton.Skill.SkillData.GetSkillName();
            GetComponent<StaminaCostIdentifier>().GetComponent<TextMeshProUGUI>().text =
                skillButton.Skill.SkillData.staminaCost.ToString(); 
        }

        protected override IEnumerator Idle() {
            if (selected) { 
                GetComponentInChildren<CanvasGroup>().DOFade(1f, selectDuration);
                transform.DOScale(1.1f, selectDuration);
                yield return new WaitForSeconds(selectDuration);
            } else {
                GetComponentInChildren<CanvasGroup>().DOFade(0f, selectDuration);
                transform.DOScale(1f, selectDuration);
            }
        }

        protected override IEnumerator Unload() {
            transform.DOScale(Vector2.zero, animationDuration);
            yield return new WaitForSeconds(animationDuration);
            Destroy(gameObject);
        }
    }
}
