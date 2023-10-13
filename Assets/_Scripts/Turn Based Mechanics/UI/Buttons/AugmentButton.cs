using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentButton : BattleButton
{
    public override void Activate(BattleUIStateMachine stateMachine, float delay) {
        base.Activate(stateMachine, delay);
    }

    public void Augment(BattleUIStateMachine stateMachine, float delay, int slot) {
        stateMachine.DelayedTransition<BattleUIStateMachine.BattleUI_SkillSelect>(delay, false);
    }
}
