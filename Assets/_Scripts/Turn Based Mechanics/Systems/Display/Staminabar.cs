using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Staminabar : MonoBehaviour {
    private Slider slider;
    private TextMeshProUGUI _text;
    [SerializeField] private Actor actor;
    void Start() {
        slider = GetComponent<Slider>();
        //_stateMachine.OnStateTransition += UpdateStaminaBar;
        BattleStateMachine.Instance.OnStateTransition += UpdateStaminaBar; 
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void UpdateStaminaBar(BattleStateMachine.BattleState state, BattleStateInput input) {
        float currStamina = actor.Stamina;
        float maxStamina = actor.Data.MaxStamina;
        Debug.Log("Stamina: " + actor.Stamina);
        
        float staminaRatio = currStamina / maxStamina;
        DOTween.To(() => slider.value, x => slider.value = x, staminaRatio, 0.5f);
        _text.text = currStamina + " / " + maxStamina;
        
        slider.value = currStamina / maxStamina;
    }

    private void UpdateStaminaBar(BattleStateInput input) {
        float currStamina = actor.Stamina;
        float maxStamina = actor.Data.MaxStamina;
        Debug.Log("Stamina: " + actor.Stamina);

        slider.value = currStamina / maxStamina;

        _text.text = currStamina + "/" + maxStamina;
    }

    private void UpdateStaminaBarOnState() {
        float currHealth = actor.Stamina;
        float maxHealth = actor.Data.MaxStamina;
        Debug.Log("Stamina: " + actor.Stamina);

        slider.value = currHealth / maxHealth;
    }
}
