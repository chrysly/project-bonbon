using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeButton : BattleButton
{
    public override void Activate(BattleUIStateMachine stateMachine, float delay) {
        base.Activate(stateMachine, delay);
        
        stateMachine.DelayedTransition<BattleUIStateMachine.InitUIState>(0.5f, true);
    }
}
