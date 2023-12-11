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
        public event System.Action<bool> OnGlobalSoftToggle;
        public event System.Action OnAnimationEvent;

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
                OnUIRefresh?.Invoke(CurrActor);
                Transition<MainStateHandler>();
            }
        }

        private void BattleStateMachine_OnBattleLock(bool disabled) {
            if (UILocked == disabled) return;
            
            if (disabled) InputReader.Disable();
            UILocked = disabled;
            OnGlobalSoftToggle?.Invoke(!disabled);
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

        public void SoftResetHandler<T>() => handlerMap[typeof(T)].SoftEnable();

        private void CycleButton(InTraversal traversal) => CurrInput.ProcessTraversal(traversal);
        private void ProcessAction(InAction action) {
            switch (action) {
                case InAction.Confirm:
                    CurrInput.SelectedButton.TryActivate();
                    break;
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

        public void ExitUI(bool autonomousBattleTransition) {
            transitionManager.RevertAll();
            DisableUnrelatedHandlers();
            InputReader.Disable();
            if (autonomousBattleTransition) BattleStateMachine.StartBattle(0.5f);
        }
    }
}