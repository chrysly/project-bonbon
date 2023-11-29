using UnityEngine;

namespace BattleUI {
    public class UINavigationManager : MonoBehaviour {
        
        public UIButtonAnimator SelectedButton { get; private set; }
        private UIAnimationBrain brain;
        private UIBrain LogicBrain => brain.LogicBrain;

        private CursorHandler cursorHandler;
        private ActorCursorHandler actorCursorHandler;
        private TargetCursorHandler targetCursorHandler;
        
        void Start() {
            brain = GetComponent<UIAnimationBrain>();
            brain.LogicBrain.OnUIRefresh += LoadHandlers;
            brain.OnSelectionChange += UIAnimationBrain_UpdateCursor;
        }

        private void LoadHandlers(Actor actor) {
            CursorHandler[] handlers = actor.GetComponentsInChildren<CursorHandler>();
            foreach (CursorHandler handler in handlers) {
                if (handler is ActorCursorHandler) actorCursorHandler = handler as ActorCursorHandler;
                if (handler is TargetCursorHandler) targetCursorHandler = handler as TargetCursorHandler;
            }
        }

        private void UIAnimationBrain_UpdateCursor(UIButtonAnimator buttonAnim) {
            bool isDifferent = SelectedButton != null && SelectedButton.Button == buttonAnim.Button;
            if (SelectedButton != null) SelectedButton.OverrideSelect(false);
            SelectedButton = buttonAnim;
            SelectedButton.OverrideSelect(true);
            if (isDifferent) UpdateCursor(buttonAnim);
        }

        private void UpdateCursor(UIButtonAnimator buttonAnimator) {
            CursorHandler nextHandler = actorCursorHandler;
            if (LogicBrain.CurrHandler is TargetSelectHandler) nextHandler = targetCursorHandler;
            else if (LogicBrain.CurrHandler is SkillSelectHandler
                     || LogicBrain.CurrHandler is BonbonCraftHandler) nextHandler = null;
            if (cursorHandler != nextHandler && cursorHandler != null) cursorHandler.Deactivate();
            cursorHandler = nextHandler;
            nextHandler.FocusEntity(buttonAnimator.CursorTarget);
        }
    }
}