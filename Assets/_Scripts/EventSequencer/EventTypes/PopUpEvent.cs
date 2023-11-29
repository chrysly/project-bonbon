using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loads another event after this one finishes
/// </summary>
[CreateAssetMenu(fileName = "New EventObject", menuName = "Event System/PopUpEvent")]
public class PopUpEvent : EventObject {
    public List<Sprite> popUps;

    public override void OnTrigger() {
        PopUpLogic popUpLogic = GameObject.FindObjectOfType<PopUpLogic>();

        if (popUpLogic == null) {
            Debug.Log("Pop Up not found");
            return;
        }

        popUpLogic.GetComponent<Image>().enabled = true;
        popUpLogic.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        popUpLogic.startPopUp(popUps);
    }
}
