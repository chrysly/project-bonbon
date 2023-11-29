using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;

/// <summary>
/// Event runner --> holds events for each scene, and runs them when conditions are met
/// </summary>
public class EventSequencer : MonoBehaviour {

    private BattleStateMachine bsm => BattleStateMachine.Instance;

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

    void Start() {
        if (bsm == null)
            return;

        bsm.CurrInput.SkillHandler.OnSkillTrigger += CheckSkillEvent;
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
        } else if (bsm.PrevState.GetType() == typeof(BattleStateMachine.BattleStart)) {
            events.Enqueue(onStartEvent);
            RunNextEvent();
            bsm.ToggleMachine(true);
        }
    }

    /// <summary>
    /// Should be called every time a skill is used, and checks if events should run based on that
    /// </summary>
    private void CheckSkillEvent(ActiveSkillPrep activeSkillPrep) {
        Debug.Log("triggered skill event");

        for (int i = 0; i < activeSkillPrep.targets. Length; ++i) {
            AIActionValue package = activeSkillPrep.skill.ComputeSkillActionValues(activeSkillPrep.targets[i]);
            package.currentTurn = bsm.CurrInput.CurrTurn();

            // add any events that meet activate conditions to a queue
            foreach (EventObject ev in eventSequence) {
                if (ev.CheckConitions(package)) {
                    Debug.Log("Event added to queue");
                    events.Enqueue(ev);
                }
            }
        }

        if (RunNextEvent()) {
            bsm.ToggleMachine(true);
        }
    }

    //public void CheckForEvents(AIActionValue package) {
    //    // add any events that meet activate conditions to a queue
    //    foreach (EventObject ev in eventSequence) {
    //        if (ev.CheckConitions(package)) {
    //            events.Enqueue(ev);
    //        }
    //    }
    //}

    public bool RunNextEvent() {
        // run the next event in queue
        if (events.Count > 0)
        {
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
    
    public void addEvent(EventObject eventToAdd) {
        events.Enqueue(eventToAdd);
    }
}
