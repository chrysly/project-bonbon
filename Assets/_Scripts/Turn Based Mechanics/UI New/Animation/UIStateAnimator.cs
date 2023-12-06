using UnityEngine;

namespace BattleUI {
    public class UIStateAnimator : MonoBehaviour {

        [SerializeField] protected UIAnimator[] animators;

        public UIAnimationBrain Brain { get; protected set; }
        public UIStateHandler StateHandler { get; protected set; }

        protected UIButtonAnimator selectedButton;

        public event System.Action<UIButtonAnimator> OnSelectionUpdate;

        protected virtual void Awake() {
            StateHandler = GetComponent<UIStateHandler>();
            StateHandler.OnHandlerToggle += UIStateHandler_OnHandlerToggle;
        }

        public virtual void Init(UIAnimationBrain brain) {
            Brain = brain;
            foreach (UIAnimator animator in animators) animator.Init(this);
        }

        public void UpdateSelection(UIButtonAnimator buttonAnim) {
            if (selectedButton != null) selectedButton.OverrideSelect(false);
            selectedButton = buttonAnim;
            selectedButton.OverrideSelect(true);
            OnSelectionUpdate?.Invoke(selectedButton);
        }

        protected virtual void UIStateHandler_OnHandlerToggle(bool toggle) {
            foreach (UIAnimator animator in animators) animator.Toggle(toggle);
        }

        #if UNITY_EDITOR
        public UIAnimator[] EditorAnimators { get => animators; set => animators = value; }
        #endif
    }

    public class UICursorHub {

        public enum UICursorType { Standard, Bonbon, TargetSelect }
        
    }
}