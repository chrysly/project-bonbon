using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnIndicator : MonoBehaviour {
    [SerializeField] private BattleStateMachine stateMachine;
    void Start() {
        stateMachine.OnStartTurn += UpdateTurnIndicator;
    }

    private void UpdateTurnIndicator(BattleStateInput input) {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.SetText("[" + input.CurrTurn() + "] " + input.ActiveActor().data.DisplayName());
    }
}
