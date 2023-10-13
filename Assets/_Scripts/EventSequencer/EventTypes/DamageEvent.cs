using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Event", menuName = "Event System/Damage Event")]
public class DamageEvent : EventObject {    // a specific character takes X amount of damage
    //public TextAsset yarnFile;
    public CharacterData character;
    public int damageAmount;

    public virtual bool CheckConitions(AIActionValue package)
    {
        if (package.target.name == character.name && package.immediateDamage >= damageAmount)
            return true;

        return false;
    }
}
