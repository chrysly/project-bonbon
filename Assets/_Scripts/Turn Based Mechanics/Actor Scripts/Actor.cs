using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, IComparable<Actor> {

    #region Data Attributes
    [SerializeField] private ActorData data;
    public StatIteration ActiveData { get; private set; }
    [SerializeField] private string uniqueID;
    #endregion Data Attributes

    #region Accessors
    public ActorData Data => data;

    public string UniqueID => uniqueID;
    #endregion Accessors

    #region Variable Attributes

    [SerializeField] private int _hitpoints;
    private int _stamina;

    public int Hitpoints => _hitpoints;
    public bool Defeated => State == ActorState.Fainted;
    public bool Available => (int) State < 3;
    public int Stamina => _stamina;

    #endregion Variable Attributes

    #region Level Skills, Bonbons & Modifiers

    public enum ActorState {
        Normal = 0,
        Confused = 1,
        Paralyzed = 2,
        Fainted = 3,
        Benched = 4,
    } public ActorState State { get; private set; }

    public List<SkillAction> SkillList { get; protected set; }

    public List<BonbonBlueprint> BonbonList { get; protected set; }

    public List<Effect> EffectList { get; protected set; }

    #endregion

    #region Bonbon Inventory

    public BonbonObject[] BonbonInventory { get; protected set; }

    #endregion

    protected virtual void Start() {
        InitializeAttributes();
        InitializeLevelObjects();
    }

    protected virtual void InitializeAttributes() {
        ActiveData = new StatIteration(this, data);
        _hitpoints = data.MaxHitpoints;
        _stamina = data.MaxStamina;
        State = ActorState.Normal;
    }

    protected virtual void InitializeLevelObjects() {
        SkillList = new List<SkillAction>();
        BonbonList = new List<BonbonBlueprint>();
        BonbonInventory = new BonbonObject[4];
        EffectList = new List<Effect>();
        ComputeStats();
        
        for (int i = 0; i < GameManager.Instance.CurrLevel; i++) {
            /// Load Skills
            foreach (SkillObject skill in data.skillMap[i]) {
                CreateSkillAction(skill);
            }

            /// Load Bonbons
            foreach (BonbonBlueprint bonbon in data.bonbonMap[i]) {
                BonbonList.Add(bonbon);
            }
        }
    }

    public void TurnStart() {
        if (Defeated) return;
        State = 0;
        List<int> spentEffects = new List<int>();
        for (int i = 0; i < EffectList.Count; i++) {
            EffectList[i].PerformActions(this);
            if (EffectList[i].IsSpent()) spentEffects.Add(i);
        } RemoveEffects(spentEffects);
        ComputeStats();
        RefundStamina(ActiveData.StaminaRegen);
    }

    private void ComputeStats() {
        ActiveData.Reset();
        List<PassiveModifier> modifiers = new List<PassiveModifier>();
        foreach (Effect effect in EffectList) {
            modifiers.Add(effect.modifiers);
        } ActiveData.ComputeModifiers(modifiers);
    }

    public void ApplyState(ActorState actorState) {
        if ((int) actorState > (int) this.State) this.State = actorState;
    }

    public void ApplyEffects(List<Effect> effects) {
        EffectList.AddRange(effects);
        ComputeStats();
    }

    public void RemoveEffects(List<int> effectIndices) {
        foreach (int effectIndex in effectIndices) EffectList.RemoveAt(effectIndex);
        ComputeStats();
    }

    //Returns true if Actor has no remaining health.
    public bool DepleteHitpoints(int damage) {
        damage = ActiveData.ComputeDefense(damage);

        if (_hitpoints - damage <= 0) {
            Faint();
            return true;
        }
        _hitpoints -= damage;
        return false;
    }

    private void Faint() {
        _hitpoints = 0;
        ApplyState(ActorState.Fainted);
        Debug.Log($"{data.DisplayName} has fallen!");
        gameObject.SetActive(false);
    }

    //Returns true if over maximum hitpoints.
    //Does not heal if Actor is defeated.
    public bool RestoreHitpoints(int heal) {
        if (_hitpoints + heal > data.MaxHitpoints) {
            _hitpoints = data.MaxHitpoints;
            return true;
        }
        if (!Defeated) {
            _hitpoints += heal;
        }
        return false;
    }

    public void ConsumeStamina(int amount) {
        _stamina -= amount;
    }

    public void InsertBonbon(int slot, BonbonObject bonbon) {
        if (BonbonInventory[slot] == null) {
            BonbonInventory[slot] = bonbon;
        } else Debug.LogError("Inventory slot was not available;");
    }

    protected void CreateSkillAction(SkillObject skillData) {
        SkillList.Add(new SkillAction(skillData, this, SkillList.Count));
    }


    public bool HasRemainingStamina() {
        return _stamina > 0;
    }

    public bool HasRemainingStamina(int cost) {
        return _stamina - cost > 0;
    }

    public int GetStamina() {
        return _stamina;
    }

    /// <summary> parameter is based on %health </summary>
    public void RefundStamina(int percent) {
        // ensure the % is in valid range
        if (percent < 0)
            percent = 0;
        else if (percent > 100)
            percent = 100;

        // calculate
        int maxStamina = data.MaxStamina;
        int refillAmount = (int) (maxStamina * percent / 100);

        if (_stamina + refillAmount > maxStamina)
            _stamina = maxStamina;
        else
            _stamina = refillAmount;
    }

    #region Comparators
    public int CompareTo(Actor actor) {
        return data.BaseSpeed - actor.data.BaseSpeed;
    }

    public override bool Equals(object obj) {
        var item = obj as Actor;

        if (item == null) {
            return false;
        }

        return item.Data.ID == data.ID;
    }

    public override int GetHashCode() {
        return HashCode.Combine(base.GetHashCode(), name, gameObject, uniqueID);
    }

    #endregion Comparators
}
