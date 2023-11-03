using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static EventConditions;   // i hope this is ok?

[CreateAssetMenu(fileName = "New EventObject", menuName ="Event System/EventObject" )]

public class EventObject : ScriptableObject {
    [SerializeReference]
    public List<Condition> eventConditions = new List<Condition>(); //check

    public TextAsset yarnFile;

    /// <summary>
    /// checks if an events conditions are met. if yes, add it to the event queue
    /// </summary>
    /// <param name="package"></param>
    public virtual bool CheckConitions(AIActionValue package) {
        foreach (Condition condition in eventConditions) {
            if (!condition.Check(package)) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// what happens when an event is called
    /// </summary>
    public virtual void OnTrigger() {
        DialogueManager.dialogueRequestEvent.Invoke(yarnFile.name);
    }

    public virtual void OnEventEnd() { }

    // NOTE
    //You'll need to implement a custom editor or context menus
    //to add derived class instances to your polymorphic list,
    //because Unity's inspector doesn't natively support that.

    // for now we use context menus for simplicity --> later we can do custom unity Generengine stuff
    [ContextMenu("Add Generic Condition")]
    void AddGenericCondition() {
        eventConditions.Add(new Condition());
    }

    [ContextMenu("Add Damage Condition")]
    void AddDamageCondition() {
        eventConditions.Add(new DamageCondition());
    }

    [ContextMenu("Add Turn Number Condition")]
    void AddTurnNumCondition() {
        eventConditions.Add(new TurnNumberCondition());
    }
}
