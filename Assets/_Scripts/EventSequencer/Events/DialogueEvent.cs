using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Event", menuName = "Event System/Dialogue Event")]
public class DialogueEvent : EventObject
{
    public GameObject dialoguePrefab;   // is this how we're doing dialogue??
}
