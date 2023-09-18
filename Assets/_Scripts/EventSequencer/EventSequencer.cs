using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSequencer
{
    public List<EventObject> eventSequence;

    private int currentEventIndex = 0;

    // where we need to execute events --> eventSequencer.StartEventSequence();
    public void StartEventSequence()
    {
        currentEventIndex = 0;
        ExecuteNextEvent();
    }

    private void ExecuteNextEvent()
    {
        if (currentEventIndex < eventSequence.Count)
        {
            EventObject currentEvent = eventSequence[currentEventIndex];
            currentEvent.OnTrigger();
            currentEventIndex++;
        }
        else
        {
            // it's done with all events
        }
    }
}
