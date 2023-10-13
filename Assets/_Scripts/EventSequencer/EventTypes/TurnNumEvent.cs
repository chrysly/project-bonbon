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

    public virtual void onTrigger() {
        Debug.Log("ok");
        GameManager.Instance.TransitionToLevel(GameManager.CurrLevel + 1);
    }

    // hard coded for M2 
    //public virtual void OnEventEnd() {
    //    Debug.Log("ev end");
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}

}
