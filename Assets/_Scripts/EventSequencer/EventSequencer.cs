using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Event runner --> holds events for each scene, and runs them when conditions are met
/// </summary>
public class EventSequencer : MonoBehaviour
{
    public List<EventObject> eventSequence;
    public EventObject onStartEvent;
    Queue<EventObject> events = new Queue<EventObject>();
    
    #region Events
    public delegate void EventTerminate();
    public event EventTerminate OnEventTerminate;
    #endregion Events

    // hard coded because fml
    public void StartEvent() {
        Debug.Log("start event");
        onStartEvent.OnTrigger();
    }

    public void CheckForEvents(AIActionValue package) { 
        // add any events that meet activate conditions to a queue
        foreach (EventObject ev in eventSequence) { 
            if (ev.CheckConitions(package)) {
                events.Enqueue(ev);
            }
        }

        // jank bc fck it's 5am
        foreach (EventObject q in events) {
            eventSequence.Remove(q);
        }
    }

    public bool RunNextEvent() {
        // run the next event in queue
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
            events.Peek().OnEventEnd();
            events.Dequeue();
        }
        OnEventTerminate?.Invoke();  //Invoke C# event whenever the battle event is terminated ᕙ(`▽´)ᕗ
    }
}
