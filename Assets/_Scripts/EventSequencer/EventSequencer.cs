using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;

/// <summary>
/// Event runner --> holds events for each scene, and runs them when conditions are met
/// When the battle starts, you run every event that's in the 'eventsToRun' queue
/// </summary>
public class EventSequencer : MonoBehaviour {

    private BattleStateMachine bsm => BattleStateMachine.Instance;

    [SerializeField] private List<EventObject> eventSequence;
    [SerializeField] private EventObject onWinEvent;
    private Queue<EventObject> eventsToRun = new Queue<EventObject>();

    #region Events
    public delegate void EventTerminate();
    public event EventTerminate OnEventTerminate;
    #endregion Events

    void Awake() {
        if (bsm == null)
            return;

        bsm.OnStateTransition += RunStateEvent;

        // add events that will play on start
        foreach (EventObject e in eventSequence) {
            if (e.getPlayOnStart()) { AddEvent(e); }
        }
    }

    void Start() {
        if (bsm == null)
            return;

        bsm.CurrInput.SkillHandler.OnSkillTrigger += CheckSkillEvent;
    }

    /// <summary>
    /// Every time the bsm.OnStateTransition event is invoked this method is called
    /// </summary>
    private void RunStateEvent(BattleStateMachine.BattleState state, BattleStateInput input) {

        if (bsm == null)
            return;

        if (state.GetType() == typeof(BattleStateMachine.WinState)) {
            AddEvent(onWinEvent);
            RunNextEvent();
        } 
        else if (state.GetType() == typeof(BattleStateMachine.LoseState)) {
            DialogueManager.dialogueRequestEvent.Invoke("death_dialogue.yarn");
        } 
        else if (bsm.PrevState.GetType() == typeof(BattleStateMachine.BattleStart)) {
            RunNextEvent();
        }
    }

    /// <summary>
    /// Should be called every time a skill is used, and checks if events should run based on that
    /// </summary>
    private void CheckSkillEvent(ActiveSkillPrep activeSkillPrep) {

        for (int i = 0; i < activeSkillPrep.targets. Length; ++i) {
            AIActionValue package = activeSkillPrep.skill.ComputeSkillActionValues(activeSkillPrep.targets[i]);
            package.currentTurn = bsm.CurrInput.CurrTurn();

            // add any events that meet activate conditions to a queue
            foreach (EventObject ev in eventSequence) {

                if (ev != null) {
                    if (ev.CheckConitions(package)) {
                        AddEvent(ev);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Run the next event in the event queue
    /// </summary>
    public void RunNextEvent() {
        if (eventsToRun.Count <= 0) {
            Debug.Log("There are no Events to Run!");
            return;
        }

        if (bsm != null) { bsm.ToggleMachine(true); }

        StartCoroutine(eventsToRun.Peek().OnTrigger());

        // subscribe to an event that triggers event end logic later
        // temporary function that takes in no arguments and has the code on the right as its statement
        eventsToRun.Peek().EventObjectTerminate += () => EventEnd();
    }

    /// <summary>
    /// Checks the event queue. If there are more events to run, run those events
    /// </summary>
    public void EventEnd() {
        if (eventsToRun.Count > 0) {
            RunNextEvent();
        } else {
            bsm.ToggleMachine(false);
            eventSequence.Remove(eventsToRun.Peek());
            eventsToRun.Dequeue();
            OnEventTerminate?.Invoke();  //Invoke C# event whenever the battle event is terminated ᕙ(`▽´)ᕗ
        }      
    }

    public void AddEvent(EventObject eventToAdd) {
        eventsToRun.Enqueue(eventToAdd);
    }
}
