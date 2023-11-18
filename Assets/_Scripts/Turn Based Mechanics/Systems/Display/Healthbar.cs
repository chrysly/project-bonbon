using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Healthbar : MonoBehaviour {
    private Slider slider;
    private BattleStateMachine stateMachine => BattleStateMachine.Instance;
    [SerializeField] private ActorData actorIdentifier;
    [SerializeField] private Actor actor;
    void Start() {
        slider = GetComponent<Slider>();
        stateMachine.OnDisplayUpdate += UpdateHealthBar;
    }

    private void UpdateHealthBar(BattleStateInput input) {
        float currHealth = actor.Hitpoints;
        float maxHealth = actor.Data.MaxHitpoints;

        slider.value = currHealth / maxHealth;
    }

    #if UNITY_EDITOR

    public ActorData ActorIdentifier => actorIdentifier;
    public Actor Actor { get => actor; set => actor = value; }

    #endif
}
