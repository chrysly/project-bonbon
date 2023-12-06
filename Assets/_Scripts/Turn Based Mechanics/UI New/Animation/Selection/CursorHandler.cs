using UnityEngine;

namespace BattleUI {
    public abstract class CursorHandler : MonoBehaviour {

        protected UIStateAnimator stateAnimator;
        protected UICursorAnimator cursorAnimator;

        void Awake() {
            stateAnimator = GetComponent<UIStateAnimator>();
            stateAnimator.OnSelectionUpdate += FocusEntity;
        }

        public virtual void FocusEntity(UIButtonAnimator target) {
            if (cursorAnimator == null) InitializeCursor(target);
            cursorAnimator.Refocus(target);
        }

        protected abstract void InitializeCursor(UIButtonAnimator target);
    }
}