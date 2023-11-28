using System.Collections.Generic;
using UnityEngine;
using PseudoDataStructures;

namespace BattleUI {

    public enum UIStateType { Main, Bonbon, Skill, TargetSelect }

    public class UIBrain : MonoBehaviour {
        public BattleStateMachine BattleStateMachine => BattleStateMachine.Instance;

        private Dictionary<System.Type, UIStateHandler> handlerMap = new ();
        UITransitionManager transitionManager = new();

        public UIStateHandler CurrHandler => transitionManager.CurrHandler;
        private UIInputPack CurrInput => transitionManager.CurrInput;

        public Actor CurrActor { get; private set; }
        public UIInput InputReader { get; private set; }

        public event System.Action<Actor> OnUIRefresh;

        public bool UILocked;

        void Start() {
            InputReader = new UIInput();
            InputReader.OnInputTraversal += CycleButton;
            InputReader.OnInputAction += ProcessAction;

            handlerMap.LoadHandlers(this, gameObject);
            BattleStateMachine.OnStateTransition += Refresh;
            BattleStateMachine.OnBattleLock += BattleStateMachine_OnBattleLock;
        }

        private void Refresh(BattleStateMachine.BattleState state, BattleStateInput input) {
            if (state is BattleStateMachine.TurnState && input.ActiveActor() is CharacterActor) {
                CurrActor = (CharacterActor) input.ActiveActor();
                handlerMap.LoadHandlers(this, CurrActor.gameObject);
                Transition<MainStateHandler>();
                OnUIRefresh?.Invoke(CurrActor);
            }
        }

        private void BattleStateMachine_OnBattleLock(bool disabled) {
            if (disabled) InputReader.Disable();
            UILocked = disabled;
        }

        public void Transition<T>(BaseTransitionInfo info = null) where T : UIStateHandler {
            if (UILocked) return;
            UIStateHandler handler = handlerMap[typeof(T)];
            UIInputPack input = handler.Enable(info);
            DisableUnrelatedHandlers(handler);
            transitionManager.Record(CurrHandler, CurrInput);
            transitionManager.Transition(handler, input);
            InputReader.Enable();
        }

        public void ReturnTo<T>() where T : UIStateHandler => transitionManager.RevertTo<T>();

        private void CycleButton(InTraversal traversal) => CurrInput.ProcessTraversal(traversal);
        private void ProcessAction(InAction action) {
            switch (action) {
                case InAction.Confirm:
                    if (CurrInput.SelectedButton.Available) {
                        CurrInput.SelectedButton.Activate();
                    } break;
                case InAction.Back:
                    if (transitionManager.Reversible) {
                        transitionManager.Revert();
                    } break;
            }
        }

        public void DisableUnrelatedHandlers(UIStateHandler handler = null) {
            foreach (KeyValuePair<System.Type, UIStateHandler> kvp in handlerMap) {
                if (handler == null || kvp.Value.Type != handler.Type) kvp.Value.Disable();
            }
        }

        public void ExitUI() {
            transitionManager.RevertAll();
            DisableUnrelatedHandlers();
            InputReader.Disable();
        }
    }
}