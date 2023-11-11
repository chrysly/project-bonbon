using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterPanel : MonoBehaviour {
    [SerializeField] private BattleStateMachine stateMachine;
    private Vector3 resetPos;
    private Vector3 newPos;
    private bool active = false;

    private void Start() {
        resetPos = transform.position;
        newPos = new Vector3(resetPos.x, resetPos.y - 300, resetPos.z);
        transform.DOMove(newPos, 0f);
        stateMachine.OnStateTransition += Toggle;
    }

    private void Toggle(BattleStateMachine.BattleState state, BattleStateInput input) {
        if (state is BattleStateMachine.BattleStart || state is BattleStateMachine.WinState ||
            state is BattleStateMachine.LoseState) {
            if (active) {
                transform.DOMove(newPos, 0.5f);
            }
        }
        else {
            if (!active) {
                transform.DOMove(resetPos, 0.5f);
            }
        }
    }
}
