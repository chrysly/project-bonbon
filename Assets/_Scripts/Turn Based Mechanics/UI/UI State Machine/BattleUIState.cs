using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIStateMachine {
    public class BattleUIState : State<BattleUIStateMachine, BattleUIState, BattleUIStateInput> {
        protected virtual void RunPreAnimation() {}
        protected virtual void RunPostAnimation() {}
    }
}
