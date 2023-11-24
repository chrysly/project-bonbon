using System.Collections.Generic;
using UnityEngine;
using PseudoDataStructures;

namespace BattleUI {

    public enum UIStateType { Main, Skill, Bonbon }

    public class UIBrain : MonoBehaviour {

        BattleStateMachine battleStateMachine => BattleStateMachine.Instance;
        public UIInput CurrInput { get; private set; }

        private Dictionary<UIStateType, UIStateHandler> stateMap;

        private UIStateHandler currHandler;
        public UIStateType CurrState => currHandler.type;

        void Awake() {
            CurrInput = new UIInput();
        }

        protected void Refresh(BattleStateMachine.BattleState state, BattleStateInput input) {
            if (state is BattleStateMachine.TurnState && input.ActiveActor() is CharacterActor) {
                CurrInput.AnimationHandler = input.ActiveActor().transform.GetComponent<UIAnimationHandler>();
                CurrInput.Actor = (CharacterActor) input.ActiveActor();
                //Transition<InitUIState>();
            }
        }

        public void Transition(UIStateType newState) {
            
        }
    }
}