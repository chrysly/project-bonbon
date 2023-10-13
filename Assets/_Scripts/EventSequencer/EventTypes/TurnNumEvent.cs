using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Damage Event", menuName = "Event System/Turn Number Event")]
public class TurnNumEvent : EventObject {
    public int turnNum;
    public virtual bool CheckConditions(AIActionValue package) { 
        if (turnNum == package.currentTurn) { 
            return true;
        }
        return false;
    }

    // hard coded for M2 
    public virtual void OnEventEnd() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
