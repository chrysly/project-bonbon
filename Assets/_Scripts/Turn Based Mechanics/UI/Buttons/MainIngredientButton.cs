using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainIngredientButton : BattleButton {
    public bool merge;
    public override void Activate(BattleUIStateMachine stateMachine, float delay) {
        base.Activate(stateMachine, delay);
        stateMachine.Transition<BattleUIStateMachine.BattleUI_IngredientSelect>();
    }
}
