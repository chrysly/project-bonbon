using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New EventObject", menuName ="Event System/EventObject" )]

public class EventObject : ScriptableObject {
    public TextAsset yarnFile;

    public virtual bool CheckConitions(AIActionValue package) {
        return true;
    }

    // override for specific behavior
    public virtual void OnTrigger() {
        DialogueManager.dialogueRequestEvent.Invoke(yarnFile.name);
    }
}
