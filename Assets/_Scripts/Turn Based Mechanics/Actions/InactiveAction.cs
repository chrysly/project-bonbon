using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveAction : ActorAction
{
    [SerializeField] private int cost = 1;

    public void RunAction(Transform actor, float duration) {
        //Action when actor is inactive
    }

    public void DisplayPreview() {

    }

    public int GetCost() {
        return cost;
    }
}
