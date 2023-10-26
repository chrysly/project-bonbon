using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Class that holds all types of event condition checks
/// </summary>
public class EventConditions {

    /// <summary>
    /// Parent class of all event check conditions. Always returns true.
    /// </summary>
    [Serializable]
    public class Condition {
        /// <summary>
        /// A that must be passed for an event to start
        /// </summary>
        public virtual bool Check(AIActionValue package) {
            return true;
        }
    }

    /// <summary>
    /// Condition: a specific actor takes >= damageAmount
    /// </summary>
    [Serializable]
    public class DamageCondition : Condition {
        public CharacterData character;
        public int damage;
        public override bool Check(AIActionValue package) {
            if (package.target.name == character.name && package.immediateDamage >= damage) {
                return true;
            }
            return false;
        }

    }

    /// <summary>
    /// Condition: check if it's a specific turn
    /// </summary>
    [Serializable]
    public class TurnNumberCondition : Condition {
        public int turnNum;
        public override bool Check(AIActionValue package) {
            if (package.currentTurn == turnNum) {
                return true;
            }
            return false;
        }

    }
}



