using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseCanvasHelper : MonoBehaviour
{
    [SerializeField] private TextAsset defeatDialgoue;
    [SerializeField] private float dialogueDelay;

    private void OnEnable() {
        StartCoroutine(MessageCoroutine());
    }

    IEnumerator MessageCoroutine() {
        yield return new WaitForSeconds(dialogueDelay);
        DialogueManager.dialogueRequestEvent.Invoke(defeatDialgoue.name);
    }
}
