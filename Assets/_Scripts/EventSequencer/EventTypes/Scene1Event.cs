using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Event", menuName = "Event System/Character Add Event")]
public class Scene1Event : EventObject
{
    public Actor character;

    public virtual void OnTrigger() { 
        
    }

    public virtual void OnEventEnd() {
        // fuck
        character.GetComponent<Renderer>().enabled = true;
        character.ApplyState(Actor.ActorState.Normal);
    }
}
