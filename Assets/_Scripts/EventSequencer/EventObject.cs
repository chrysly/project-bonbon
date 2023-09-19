using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EventObject", menuName ="Event System/EventObject" )]

public class EventObject : ScriptableObject
{
    public int executionOrder = 0;
    public GameObject prefabToLoad;

    public virtual void OnTrigger() // override for specific behavior
    {

    }
}
