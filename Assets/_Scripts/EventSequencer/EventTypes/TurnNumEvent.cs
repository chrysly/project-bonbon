using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Event", menuName = "Event System/Turn Number Event")]
public class TurnNumEvent : EventObject {
    public int turnNum;
    public virtual bool CheckConditions(int turn) { 
        if (turnNum == turn) { 
            return true;
        }
        return false;
    }
}
