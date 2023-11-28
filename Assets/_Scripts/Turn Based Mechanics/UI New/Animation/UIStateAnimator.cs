using UnityEngine;

namespace BattleUI {
    public class UIStateAnimator : MonoBehaviour {

        [SerializeField] protected UIAnimator[] animators;

        public UIAnimationBrain Brain { get; private set; }
        protected UIStateHandler stateHandler;

        protected virtual void Awake() {
            stateHandler = GetComponent<UIStateHandler>();
            foreach (UIAnimator animator in animators) animator.Init(this);
            stateHandler.OnHandlerToggle += UIStateHandler_OnHandlerToggle;
        }

        public virtual void Init(UIAnimationBrain brain) => Brain = brain;

        protected virtual void UIStateHandler_OnHandlerToggle(bool toggle) {
            foreach (UIAnimator animator in animators) animator.Toggle(toggle);
        }

        #if UNITY_EDITOR
        public UIAnimator[] EditorAnimators { get => animators; set => animators = value; }
        #endif
    }
}