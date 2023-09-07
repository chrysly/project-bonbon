using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, IComparable<Actor>
{
    #region Data Attributes
    [SerializeField] public ActorData data;
    [SerializeField] private string uniqueID;
    #endregion Data Attributes
    
    #region Accessors
    public ActorData Data() { return data; }

    public string UniqueID() { return uniqueID; }
    #endregion Accessors
    
    #region Variable Attributes
    [SerializeField] private float _hitpoints;
    private bool _defeated;
    private int _stamina;
    #endregion Variable Attributes
    
    protected virtual void Start()
    {
        InitializeAttributes();
    }

    protected virtual void InitializeAttributes() {
        _hitpoints = data.MaxHitpoints();
        _stamina = data.MaxStamina();
        _defeated = false;
    }

    //Returns true if Actor has no remaining health.
    public bool DepleteHitpoints(float damage) {
        if (_hitpoints - damage <= 0) {
            _hitpoints = 0;
            _defeated = true;
            Debug.Log($"{this.data.DisplayName()} has fallen!");
            return true;
        }
        _hitpoints -= damage;
        return false;
    }
    
    //Returns true if over maximum hitpoints.
    //Does not heal if Actor is defeated.
    public bool RestoreHitpoints(float heal) {
        if (_hitpoints + heal > data.MaxHitpoints()) {
            _hitpoints = data.MaxHitpoints();
            return true;
        }
        if (!_defeated) {
            _hitpoints += heal;
        }
        return false;
    }

    public float Hitpoints() {
        return _hitpoints;
    }

    public bool Defeated() {
        return _defeated;
    }
    
    public bool HasRemainingStamina() {
        return _stamina > 0;
    }

    public bool HasRemainingStamina(int cost) {
        return _stamina - cost > 0;
    }
    
    public void RefundStamina(int cost) {
        if (_stamina + cost > data.MaxStamina()) {
            _stamina = data.MaxStamina();
        } else {
            _stamina += cost;
        }
    }
    
    #region Comparators
    public int CompareTo(Actor actor) {
        return data.BaseSpeed() - actor.data.BaseSpeed();
    }
    
    public override bool Equals(object obj) {
        var item = obj as Actor;

        if (item == null) {
            return false;
        }

        return item.Data().ID() == data.ID();
    }
    #endregion Comparators
}
