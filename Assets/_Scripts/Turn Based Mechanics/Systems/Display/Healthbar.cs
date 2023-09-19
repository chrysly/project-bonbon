using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Healthbar : MonoBehaviour {
    private Slider slider;
    [SerializeField] private BattleStateMachine _stateMachine;
    [SerializeField] private Actor actor;
    //void Start() {
    //    slider = GetComponent<Slider>();
    //    _stateMachine.OnStateTransition += UpdateHealthBar;
    //}

    //private void UpdateHealthBar(BattleStateMachine.BattleState state, BattleStateInput input) {
    //    if (state is not BattleStateMachine.AnimateState) return;
    //    if (input.ActiveSkill().Target().UniqueID() == actor.UniqueID()) {
    //        float currHealth = input.ActiveSkill().Target().Hitpoints();
    //        float maxHealth = input.ActiveSkill().Target().data.MaxHitpoints();

    //        slider.value = currHealth / maxHealth;
    //    } else if (input.ActiveSkill().Caster().UniqueID() == actor.UniqueID()) {
    //        float currHealth = input.ActiveSkill().Caster().Hitpoints();
    //        float maxHealth = input.ActiveSkill().Caster().data.MaxHitpoints();

    //        slider.value = currHealth / maxHealth;
    //    }
    //}
}
