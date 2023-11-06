using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareButton : BattleButton
{
    public override void Activate(BattleUIStateMachine stateMachine, float delay) {
        base.Activate(stateMachine, delay);
        
        stateMachine.DelayedTransition<BattleUIStateMachine.BattleUI_Limbo>(delay, false);
        stateMachine._battleStateMachine.StartBattle(0.5f);
    }
}
