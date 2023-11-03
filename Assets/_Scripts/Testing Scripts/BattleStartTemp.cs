using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStartTemp : MonoBehaviour
{
    bool hasStarted = false;
    [SerializeField] private BattleStateMachine _stateMachine;

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                _stateMachine.StartBattle();
                hasStarted = true;
            }
        }
    }
}
