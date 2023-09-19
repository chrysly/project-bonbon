using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSequencer : MonoBehaviour // not sure if should be monobehabior
{
    public List<EventObject> eventSequence;
    public Transform spawnPoint;    // temporary

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

            // check if you need to load prefab
            if (currentEvent.prefabToLoad != null)
            {
                Instantiate(currentEvent.prefabToLoad, spawnPoint.position, Quaternion.identity);
            }

            currentEvent.OnTrigger();
            currentEventIndex++;
        }
        else
        {
            // it's done with all events
            Debug.Log("No more sequence");
        }
    }
}
