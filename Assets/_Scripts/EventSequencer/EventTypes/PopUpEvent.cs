using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads another event after this one finishes
/// </summary>
[CreateAssetMenu(fileName = "New EventObject", menuName = "Event System/PopUpEvent")]
public class PopUpEvent : EventObject {
    public List<Sprite> popUps;

    public override void OnTrigger() {
        PopUpLogic popUpLogic = GameObject.FindObjectOfType<PopUpLogic>();

        popUpLogic.startPopUp(popUps);
    }
}
