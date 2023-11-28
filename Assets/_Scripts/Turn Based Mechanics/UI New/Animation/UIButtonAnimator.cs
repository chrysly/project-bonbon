using UnityEngine;

namespace BattleUI {
    public class UIButtonAnimator : UIAnimator {

        [SerializeField] private Transform cursorTarget;
        public Transform CursorTarget => cursorTarget;

        public UIButton Button { get; private set; }

        protected override void Awake() {
            base.Awake();
            Button = GetComponent<UIButton>();
            Button.OnSelect += UIButton_OnSelect;
            Button.OnActivate += UIButton_OnActivate;
        }

        protected void UIButton_OnSelect() {
            stateAnimator.Brain.UpdateSelection(this);
        }

        protected void UIButton_OnActivate() {

        }
    }
}