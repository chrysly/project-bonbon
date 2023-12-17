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

        public event System.Action<bool> OnToggle;
        public event System.Action<bool> OnActivate;

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
            selected = select;
        }

        protected void UIButton_OnSelect() {
            stateAnimator.UpdateSelection(this);
        }

        protected override IEnumerator Idle() {
            if (selected) {
                transform.DOScale(selectedScale, animationDuration);
                yield return new WaitForSeconds(animationDuration / 2);
            } else {
                transform.DOScale(new Vector3(targetScale, targetScale, 1f), animationDuration);
                yield return new WaitForSeconds(animationDuration / 2);
            }
        }

        public override void Toggle(bool toggle) {
            base.Toggle(toggle);
            if (toggle) ProcessAvailability();
            targetScale = toggle ? 1 : 0;
            if (!toggle) selected = false;
            OnToggle?.Invoke(toggle);
        }

        /// <summary>
        /// Visual feedback for when the button is selected;
        /// </summary>
        protected virtual void UIButton_OnActivate() {
            if (!Button.Available) {
                transform.DOShakeRotation(0.5f, new Vector3(0, 0, 15 * Mathf.Sign(Mathf.Sin(Time.time))),
                                          10, 45, true, ShakeRandomnessMode.Harmonic).SetEase(Ease.OutQuint);
            } else transform.DOScale(1.5f, animationDuration).SetEase(Ease.OutBack);
            OnActivate?.Invoke(Button.Available);
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