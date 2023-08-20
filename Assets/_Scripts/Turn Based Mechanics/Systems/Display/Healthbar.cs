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
    void Start() {
        slider = GetComponent<Slider>();
        _stateMachine.OnConfirmTurn += UpdateHealthBar;
    }

    private void UpdateHealthBar(BattleStateInput input) {
        if (input.ActiveSkill().Target().data.ID() == actor.data.ID()) {
            float currHealth = input.ActiveSkill().Target().Hitpoints();
            float maxHealth = input.ActiveSkill().Target().data.MaxHitpoints();

            slider.value = currHealth / maxHealth;
        }
    }
}
