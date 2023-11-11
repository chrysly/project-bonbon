using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour {

    [SerializeField] private BattleStateMachine battleStateMachine;
    [SerializeField] private SkillAnimationMap skillAnimationMap;
    public Dictionary<SkillObject, Dictionary<ActorData, SkillAnimation>> SkillAMap { get; private set; }

    public event System.Action OnSkillHit;

    void Awake() {
        SkillAMap = SKAEUtils.ProcessInternalDictionary(skillAnimationMap.animationMap);
    }

    void Start() {
        battleStateMachine.CurrInput.OnSkillAnimation += OnSkillTrigger;
    }

    public void OnSkillTrigger(SkillAction skillAction, BonbonObject bonbon = null) {
        try {
            SkillAnimation sa = SkillAMap[skillAction.SkillData][skillAction.Caster.Data];
            skillAction.Caster.GetComponentInChildren<Animator>(true).SetTrigger(sa.AnimationTrigger);
            battleStateMachine.StartBattle(sa.AnimationDuration);
            StartCoroutine(HitDelay(sa.HitDelay));
            if (bonbon != null) ; /// Do VFXs
        } catch (KeyNotFoundException) {
            Debug.LogWarning($"Animation Undefined for {skillAction.SkillData.Name} -> {skillAction.Caster.Data.DisplayName}");
        }
    }

    private IEnumerator HitDelay(float delay) {
        yield return new WaitForSeconds(delay);
        OnSkillHit?.Invoke();
    }
}
