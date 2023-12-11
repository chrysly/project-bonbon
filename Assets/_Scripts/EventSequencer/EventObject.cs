using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static EventConditions;   // i hope this is ok?

[CreateAssetMenu(fileName = "New EventObject", menuName ="Event System/EventObject" )]

public class EventObject : ScriptableObject {
    [SerializeReference]
    public List<Condition> eventConditions = new List<Condition>();

    [SerializeField] private TextAsset yarnFile;
    [SerializeField] private bool playOnStart = false;
    public bool getPlayOnStart() { return playOnStart; }

    #region Events
    public event System.Action EventObjectTerminate;
    #endregion

    /// <summary>
    /// return true if event conditions are met
    /// </summary>
    public virtual bool CheckConitions(AIActionValue package) {

        foreach (Condition condition in eventConditions) {
            if (condition != null) {
                if (!condition.Check(package)) {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// what happens when an event is called
    /// </summary>
    public virtual IEnumerator OnTrigger() {

        yield return new WaitForSeconds(1f);    // because if not it goes to fast and glitches out
        DialogueManager.dialogueRequestEvent.Invoke(yarnFile.name);

        //wait while the dialogue event is still running
        while (DialogueManager.dialogueIsOccuring) {
            Debug.Log(yarnFile.name + " waiting...");
            yield return null;
        }

        OnEventEnd();
    }
    //public virtual void OnTrigger() {
    //    DialogueManager.dialogueRequestEvent.Invoke(yarnFile.name);

    //    DialogueManager.OnDialogueStart += () => OnEventEnd();
    //}

    public virtual void OnEventEnd() {
        EventObjectTerminate.Invoke();
    }

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
