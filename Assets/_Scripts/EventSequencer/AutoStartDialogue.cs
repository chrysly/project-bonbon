using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStartDialogue : MonoBehaviour {
    public float waitToStart = 0.0f;

    public List<EventObject> campEvents;

    public EventSequencer evSeq;

    void Start() {
        StartCoroutine(Wait());
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(waitToStart);

        foreach (EventObject ev in campEvents) {
            evSeq.AddEvent(ev);
        }
        evSeq.RunNextEvent();
    }
}
