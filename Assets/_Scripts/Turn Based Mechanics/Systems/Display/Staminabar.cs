using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Staminabar : MonoBehaviour {
    private Slider slider;
    [SerializeField] private BattleUIStateMachine _stateMachine;
    [SerializeField] private Actor actor;
    void Start() {
        slider = GetComponent<Slider>();
        _stateMachine.OnStateTransition += UpdateStaminaBar;
        BattleStateMachine.Instance.OnStateTransition += UpdateStaminaBar;
    }

    private void UpdateStaminaBar(BattleUIStateMachine.BattleUIState state, BattleUIStateInput input) {
        float currStamina = actor.Stamina;
        float maxStamina = actor.Data.MaxStamina;
        Debug.Log("Stamina: " + actor.GetStamina());

        slider.value = currStamina / maxStamina;
    }

    private void UpdateStaminaBar(BattleStateMachine.BattleState state, BattleStateInput input) {
        float currHealth = actor.Stamina;
        float maxHealth = actor.Data.MaxStamina;
        Debug.Log("Stamina: " + actor.GetStamina());

        slider.value = currHealth / maxHealth;
    }

    private void UpdateStaminaBarOnState() {
        float currHealth = actor.Stamina;
        float maxHealth = actor.Data.MaxStamina;
        Debug.Log("Stamina: " + actor.GetStamina());

        slider.value = currHealth / maxHealth;
    }
}
