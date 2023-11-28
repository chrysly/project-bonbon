using System.Collections;
using System.Collections.Generic;
using BattleUI;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BattleUI {
    public class SkillButtonAnimator : UIButtonAnimator {
        [SerializeField] private float selectDuration = 0.3f;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI staminaText;
        private CanvasGroup canvasGroup;

        protected override void Awake() {
            base.Awake();
            canvasGroup = GetComponentInChildren<CanvasGroup>(true);
        }

        public override void Init(UIStateAnimator stateAnimator) {
            base.Init(stateAnimator);
            this.stateAnimator.StateHandler.OnHandlerRevert += PseudoDestroy;
        }

        public void PseudoDestroy() {
            stateAnimator.StateHandler.OnHandlerRevert -= PseudoDestroy;
            Destroy(gameObject);
        }

        public override void Toggle(bool toggle) {
            base.Toggle(toggle);
            SkillSelectButton skillButton = Button as SkillSelectButton;
            nameText.text = skillButton.Skill.SkillData.GetSkillName();
            staminaText.text = "STA " + skillButton.Skill.SkillData.staminaCost.ToString();
        }

        protected override IEnumerator Idle() {
            if (selected) { 
                canvasGroup.DOFade(1f, selectDuration);
                transform.DOScale(1.1f, selectDuration);
                yield return new WaitForSeconds(selectDuration);
            } else {
                canvasGroup.DOFade(0f, selectDuration);
                transform.DOScale(targetScale, selectDuration);
                yield return new WaitForSeconds(selectDuration);
            }
        }

        protected override IEnumerator Unload() {
            transform.DOScale(Vector2.zero, animationDuration);
            yield return new WaitForSeconds(animationDuration);
        }
    }
}
