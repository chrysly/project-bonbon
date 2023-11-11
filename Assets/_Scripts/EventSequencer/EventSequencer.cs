using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;

/// <summary>
/// Event runner --> holds events for each scene, and runs them when conditions are met
/// </summary>
public class EventSequencer : MonoBehaviour {

    [SerializeField] private BattleStateMachine bsm;

    public List<EventObject> eventSequence;
    public EventObject onStartEvent;
    public EventObject onWinEvent;
    Queue<EventObject> events = new Queue<EventObject>();

    #region Events
    public delegate void EventTerminate();
    public event EventTerminate OnEventTerminate;
    #endregion Events

    void Awake() {
        if (bsm == null)
            return;

        bsm.OnStateTransition += RunEvent;
    }

    /// <summary>
    /// Every time the bsm.OnStateTransition event is invoked this method is called
    /// </summary>
    private void RunEvent(BattleStateMachine.BattleState state, BattleStateInput input) {

        if (bsm == null)
            return;

        if (state.GetType() == typeof(BattleStateMachine.WinState)) {
            events.Enqueue(onWinEvent);
            RunNextEvent();
        } else if (state.GetType() == typeof(BattleStateMachine.LoseState)) {
            DialogueManager.dialogueRequestEvent.Invoke("death_dialogue.yarn");
            bsm.ToggleMachine(true);
            bsm.uiStateMachine.ToggleMachine(true);
        } else if (bsm.PrevState.GetType() == typeof(BattleStateMachine.BattleStart)) {
            onStartEvent.OnTrigger();
            bsm.ToggleMachine(true);
            bsm.uiStateMachine.ToggleMachine(true);
        }
    }

    //// hard coded because fml
    //public void StartEvent() {  // CHANGE TO EVENTS DUMMY
    //    onStartEvent.OnTrigger();
    //}

    public void CheckForEvents(AIActionValue package) {
        // add any events that meet activate conditions to a queue
        foreach (EventObject ev in eventSequence) {
            if (ev.CheckConitions(package)) {
                events.Enqueue(ev);
            }
        }
    }

    public bool RunNextEvent() {
        // run the next event in queue
        Debug.Log("Queue count: " + events.Count);
        if (events.Count > 0)
        {
            Debug.Log("event");
            events.Peek().OnTrigger();
            return true;
        }
        return false;
    }

    public void CheckForEventEnd() {
        Debug.Log("noice");

        if (events.Count != 0) {
            Debug.Log("actually nice");
            events.Peek().OnEventEnd();
            eventSequence.Remove(events.Peek());
            events.Dequeue();
        }
        OnEventTerminate?.Invoke();  //Invoke C# event whenever the battle event is terminated ᕙ(`▽´)ᕗ
    }
}
