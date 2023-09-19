using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override String ToString() {
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

    public void ActivateSkill() 
    {
        foreach(Actor target in _targets)
        {
            target.DepleteHitpoints(_data.damageAmount);
            target.RestoreHitpoints(_data.healAmount);
        }
        
        int selfInflictAmount = _data.selfInflictAmount;
        if (selfInflictAmount > 0f) {
            _caster.DepleteHitpoints(selfInflictAmount);
        } else {
            _caster.RestoreHitpoints(-selfInflictAmount);
        }
    }

    public void Clear() {
        _data = null;
        _targets = null;
        _caster = null;
    }
}
