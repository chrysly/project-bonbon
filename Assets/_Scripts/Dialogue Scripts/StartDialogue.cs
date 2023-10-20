using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    public TextAsset yarnFile;
    public void startDialogue() {
        DialogueManager.dialogueRequestEvent.Invoke(yarnFile.name);
    }
}
