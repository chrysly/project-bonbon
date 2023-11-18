using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationHandler : StateMachineHandler {

    [SerializeField] private SkillAnimationMap skillAnimationMap;
    private BattleStateMachine battleStateMachine => BattleStateMachine.Instance;

    #region Events
    public event Action<int, Actor> DamageEvent;
    public event Action<int, Actor> HealEvent;
    public event Action<EffectBlueprint, Actor> EffectEvent;
    #endregion Events
    public Dictionary<SkillObject, Dictionary<ActorData, SkillAnimation>> SkillAMap { get; private set; }

    void Awake() {
        SkillAMap = SKAEUtils.ProcessInternalDictionary(skillAnimationMap.animationMap);
    }

    public override void Initialize(BattleStateInput input) {
        base.Initialize(input);
        input.SkillHandler.OnSkillTrigger += OnSkillTrigger;
    }

    public void OnSkillTrigger(ActiveSkillPrep skillPrep) {
        SkillAction skillAction = skillPrep.skill;
        BonbonObject bonbon = skillPrep.bonbon;
        AIActionValue[] avs = skillPrep.targets.Select(target => skillAction.ComputeSkillActionValues(target)).ToArray();
        try {
            SkillAnimation sa = SkillAMap[skillAction.SkillData][skillAction.Caster.Data];
            CameraAnimationPackage cap = sa.CameraAnimationPackage;
            if (cap != null) input.CameraHandler.PlayAnimation(cap);
            Animator casterAnimator = skillAction.Caster.GetComponentInChildren<Animator>(true);
            casterAnimator.SetTrigger(sa.AnimationTrigger);
            battleStateMachine.StartBattle(sa.AnimationDuration);
            if (bonbon != null) ; /// Do VFXs

            foreach (DelaySkillAnimation delaySkillAnimation in sa.DelaySkills) {
                foreach (IEnumerator ie in delaySkillAnimation.GetCoroutines(this, avs, skillPrep.targets)) {
                    StartCoroutine(ie);
                }
            }
        } catch (KeyNotFoundException) {
            Debug.LogWarning($"Animation Undefined for {skillAction.SkillData.Name} -> {skillAction.Caster.Data.DisplayName}");
        }
    }

    #region Events
    public void TriggerDamage(int damage, Actor actor) {
        DamageEvent?.Invoke(damage, actor);
    }

    public void TriggerHeal(int heal, Actor actor) {
        HealEvent?.Invoke(heal, actor);
    }

    public void TriggerEffect(EffectBlueprint effect, Actor actor) {
        EffectEvent?.Invoke(effect, actor);
    }
    #endregion Events
}
