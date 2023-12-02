using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BattleUI {
    public class UIButtonAnimator : UIAnimator {

        [SerializeField] public Transform cursorTarget;
        public Transform CursorTarget => cursorTarget;
        public UIButton Button { get; protected set; }

        protected MaskableGraphic[] graphics;

        protected float selectedScale = 1.2f;
        protected float targetScale;
        protected bool selected;

        protected override void Awake() {
            base.Awake();
            LoadLogicButton();
            graphics = GetComponentsInChildren<MaskableGraphic>(true);
        }

        protected virtual void LoadLogicButton() {
            Button = GetComponent<UIButton>();
            Button.OnSelect += UIButton_OnSelect;
            Button.OnActivate += UIButton_OnActivate;
        }

        public virtual void OverrideSelect(bool select) {
            //stateAnimator.SendCursor(this);
            selected = select;
        }

        protected void UIButton_OnSelect() {
            stateAnimator.Brain.UpdateSelection(this);
        }

        protected override IEnumerator Idle() {
            if (selected) {
                transform.DOScale(Vector2.one * selectedScale, animationDuration);
                yield return new WaitForSeconds(animationDuration / 2);
            } else {
                transform.DOScale(Vector2.one * targetScale, animationDuration);
                yield return new WaitForSeconds(animationDuration / 2);
            }
        }

        public override void Toggle(bool toggle) {
            base.Toggle(toggle);
            ProcessAvailability();
            targetScale = toggle ? 1 : 0;
            if (!toggle) selected = false;
        }

        /// <summary>
        /// Visual feedback for when the button is selected;
        /// </summary>
        protected virtual void UIButton_OnActivate() {
            transform.DOShakeRotation(0.1f).SetEase(Ease.OutElastic);
        }

        /// <summary>
        /// Updates the visual appearance of the button based on availability;
        /// </summary>
        protected virtual void ProcessAvailability() {
            float targetAlpha = Button.Available ? 1 : 0.25f;
            foreach (MaskableGraphic graphic in graphics) graphic.DOFade(targetAlpha, 0);
        }
    }
}