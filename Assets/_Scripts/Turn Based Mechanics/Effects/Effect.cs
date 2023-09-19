using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ScriptableObject {

    public int duration;
    public List<EffectModifier> modifiers;
    public List<EffectAction> actions;
}

public abstract class EffectModifier : ScriptableObject {

    public float attackModifier;
    public float healModifier;
    public float resistanceModifier;
    public float evasionModifier;
    public float speedModifier;
}

public abstract class EffectAction : ScriptableObject {

    protected int amount;

    public delegate System.Action<Actor, int> Damage(Actor target, int amount);

    public delegate System.Action<Actor, int> Heal(Actor target, int amount);

    public delegate System.Action<int> Skip(int amount);
    public abstract void Use(Actor target = null);

    public EffectAction(int amount) {
        this.amount = amount;
    }
}

public class DamageAction : EffectAction {

    public DamageAction(int amount) : base(amount) { }

    public override void Use(Actor target = null) {
        target.DepleteHitpoints(amount);
    }
}

public class HealAction : EffectAction {

    public HealAction(int amount) : base(amount) { }

    public override void Use(Actor target = null) {
        target.RestoreHitpoints(amount);
    }
}

public class SkipTurnAction : EffectAction {

    public SkipTurnAction(int amount) : base(amount) { }

    public override void Use(Actor target = null) {
        // Skip Turn;
    }
}