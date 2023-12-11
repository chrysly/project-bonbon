using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads another event after this one finishes
/// </summary>
[CreateAssetMenu(fileName = "New EventObject", menuName = "Event System/LoadEventEvent")]
public class LoadEventEvent : EventObject {
    public EventObject eventToLoad;
    public EventObject thisEvent;

    public override IEnumerator OnTrigger() {
        thisEvent.OnTrigger();
        yield return null;
    }
}