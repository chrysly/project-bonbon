using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIStateMachine {
    /// <summary>
    /// Limbo state for UI. Used for debugging and ensured stripping of functionality
    /// of UI during animate states.
    /// </summary>
    public class BattleUI_Limbo : BattleUIStateMachine.BattleUIState {
        public override void Enter(BattleUIStateInput i) {
            Debug.Log("In Limbo");
        }
        
        public override void Exit(BattleUIStateInput i) {
            Debug.Log("Exit Limbo");
        }
    }
}
