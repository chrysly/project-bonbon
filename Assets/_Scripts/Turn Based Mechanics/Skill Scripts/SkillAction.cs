using System.Collections.Generic;

public class SkillAction {
    private SkillObject _data;
    private Actor _caster;
    private List<Actor> _targets;

    public SkillAction(SkillObject data) {
        _data = data;
    }
    
    public SkillAction(SkillObject data, Actor caster, List<Actor> target) {
        _data = data;
        _caster = caster;
        _targets = target;
    }

    public void SetTarget(List<Actor> target) {
        _targets = target;
    }

    public void SetCaster(Actor caster) {
        _caster = caster;
    }

    public void SetSkill(SkillObject data) {
        _data = data;
    }

    public override string ToString() {
        return _data.GetSkillName();
    }

    public List<Actor> Targets() {
        return _targets;
    }

    public Actor Caster() {
        return _caster;
    }

    public SkillObject Data() {
        return _data;
    }

    public AIActionValue ComputeSkillActionValues(Actor actor) {
        AIActionValue actionValue = new AIActionValue();
        _data.ComputeActionValues(ref actionValue);
        /// Throw values against actor stats;
        return actionValue;
    }

    public void ActivateSkill() 
    {
        foreach(Actor target in _targets)
        {
            //target.DepleteHitpoints(_data.damageAmount);
            //target.RestoreHitpoints(_data.healAmount);
        }
    }

    public void Clear() {
        _data = null;
        _targets = null;
        _caster = null;
    }
}

public class StatIteration {

    public readonly Actor Actor;
    private readonly ActorData baseData;

    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int StaminaRegen { get; private set; }

    public StatIteration(Actor actor, ActorData data) {
        Actor = actor;
        baseData = data;
        Reset();
    }

    public void Reset() {
        Attack = baseData.BaseAttack();
        Defense = baseData.BaseDefense();
        StaminaRegen = baseData.StaminaRegenRate();
    }

    public void ComputeModifiers(List<EffectModifier> mods) {
        foreach (EffectModifier mod in mods) {
            Attack = (int) (mod.attackModifier * Attack);
            Defense = (int) (mod.defenseModifier * Defense);
            StaminaRegen = (int) (mod.staminaRegenModifier * StaminaRegen);
        }
    }
}