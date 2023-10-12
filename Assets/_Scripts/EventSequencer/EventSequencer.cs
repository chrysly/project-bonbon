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
    Queue<EventObject> events = new Queue<EventObject>();
    
    #region Events
    public delegate void EventTerminate();
    public event EventTerminate OnEventTerminate;
    #endregion Events

    public void CheckForEvents(AIActionValue package) { 
        // add any events that meet activate conditions to a queue
        foreach (EventObject ev in eventSequence) { 
            if (ev.CheckConitions(package)) {
                events.Enqueue(ev);
            }
        }

        // run the next event in queue
        if (events.Count > 0) {
            EventObject next = events.Dequeue();
            next.OnTrigger();
        }
    }

    public void CheckForEventEnd() {
        OnEventTerminate?.Invoke();  //Invoke C# event whenever the battle event is terminated ᕙ(`▽´)ᕗ
    }
}
