using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStartDialogue : MonoBehaviour {
    public float waitToStart = 0.0f;

    public EventObject startEvent;

    public EventSequencer eventSequencer;
    // Start is called before the first frame update
    void Start() {
        Debug.Log("huh");
        StartCoroutine(Wait());
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(waitToStart);
        eventSequencer.addEvent(startEvent);
        eventSequencer.RunNextEvent();
        Debug.Log("triggered");
    }
}
