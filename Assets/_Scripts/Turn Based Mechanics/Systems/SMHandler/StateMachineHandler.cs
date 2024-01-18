using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineHandler : MonoBehaviour {

    protected BattleStateInput input;
    public BattleStateInput Input => input;

    public virtual void Initialize(BattleStateInput input) => this.input = input;
}
