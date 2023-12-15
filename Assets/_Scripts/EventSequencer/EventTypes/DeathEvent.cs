using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Death event (for game overs)
/// </summary>
[CreateAssetMenu(fileName = "New EventObject", menuName = "Event System/DeathEvent")]
public class DeathEvent : EventObject
{
    public override IEnumerator OnTrigger() {
        DeathLogic deathLogic = GameObject.FindObjectOfType<DeathLogic>();

        if (deathLogic == null) {
            Debug.Log("Death logic not found");
            yield break;
        }

        deathLogic.BackgroundFlash();

        yield return new WaitForSeconds(1f);    // because if not it goes to fast and glitches out
        DialogueManager.dialogueRequestEvent.Invoke(yarnFile.name);

        //wait while the dialogue event is still running
        while (DialogueManager.dialogueIsOccuring) {
            Debug.Log(yarnFile.name + " waiting...");
            yield return null;
        }

        // activate the reset/game over window
        deathLogic.gameObject.transform.GetChild(1).gameObject.SetActive(true); // LMAO
    }
}
