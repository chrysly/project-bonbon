using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStartTemp : MonoBehaviour
{
    bool hasStarted = false;
    private BattleStateMachine stateMachine => BattleStateMachine.Instance;

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                stateMachine.StartBattle();
                hasStarted = true;
            }
        }
    }
}
