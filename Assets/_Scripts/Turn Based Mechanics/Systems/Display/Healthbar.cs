using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Healthbar : MonoBehaviour {
    private Slider slider;
    private TextMeshProUGUI _text;
    private BattleStateMachine stateMachine => BattleStateMachine.Instance;
    [SerializeField] private ActorData actorIdentifier;
    [SerializeField] private Actor actor;
    void Start() {
        slider = GetComponent<Slider>();
        stateMachine.OnStateTransition += UpdateHealthBar;
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void UpdateHealthBar(BattleStateMachine.BattleState state, BattleStateInput input) {
        float currHealth = actor.Hitpoints;
        float maxHealth = actor.Data.MaxHitpoints;

        float healthRatio = currHealth / maxHealth;
        DOTween.To(() => slider.value, x => slider.value = x, healthRatio, 0.5f);
        _text.text = currHealth + " / " + maxHealth;
    }

    #if UNITY_EDITOR

    public ActorData ActorIdentifier => actorIdentifier;
    public Actor Actor { get => actor; set => actor = value; }

    #endif
}
