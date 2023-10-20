using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentButton : BattleButton
{
    public override void Activate(BattleUIStateMachine stateMachine, float delay) {
        base.Activate(stateMachine, delay);
        stateMachine.CurrInput.slot = stateMachine.CurrInput.AnimationHandler.bonbonWindow.mainButtonIndex;
        stateMachine.DelayedTransition<BattleUIStateMachine.BattleUI_AugmentSkill>(0.2f, false);
    }
}
