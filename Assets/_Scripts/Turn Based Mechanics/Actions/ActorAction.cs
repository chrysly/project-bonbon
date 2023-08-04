using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ActorAction {
    public int GetCost();
    public void RunAction(Transform actor, float duration);
}
