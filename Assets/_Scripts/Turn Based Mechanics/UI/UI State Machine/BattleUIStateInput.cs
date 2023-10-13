using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIStateInput : StateInput {
    public UIAnimationHandler AnimationHandler;
    public CharacterActor actor;

    public int slot;
    
    //Freezes all UI response
    public bool Locked;
}
