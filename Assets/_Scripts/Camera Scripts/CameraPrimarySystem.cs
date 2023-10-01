using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraPrimarySystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera orbitalCam;
    [SerializeField] private CinemachineVirtualCamera aerialCam;
    [SerializeField] private CinemachineVirtualCamera charCam;
    [SerializeField] private BattleStateMachine stateMachine;
    //[SerializeField] private Transform testTarget;
    // Start is called before the first frame update

    private CinemachineVirtualCamera activeCam;
    private Transform oldLookAt;
    
    void Start() {
        orbitalCam.Priority = 5;
        activeCam = orbitalCam;
        oldLookAt = charCam.m_LookAt;
        stateMachine.OnStateTransition += UpdateCamera;
    }

    private void UpdateCamera(BattleStateMachine.BattleState state, BattleStateInput input) {
        Debug.Log("call");
        if (state is BattleStateMachine.TurnState) { FocusActor(input); }
        else if (state is BattleStateMachine.AnimateState) { ViewAnimate(input); }
    }

    private void FocusActor(BattleStateInput input) {
        charCam.m_LookAt = oldLookAt;
        if (input.ActiveActor() is not CharacterActor) {
            Debug.Log("to enemy");
            ReturnToBattleView(input);
            return;
        }

        Debug.Log("to chara");
        Transform target = input.ActiveActor().transform.GetChild(0);
        charCam.m_Follow = target;
        SetActiveCam(charCam);
    }

    private void ReturnToBattleView(BattleStateInput input) {
        if (input.ActiveActor() is not EnemyActor) return;
        SetActiveCam(orbitalCam);
    }

    private void ViewAnimate(BattleStateInput input) {

        Transform target = input.SkillPrep.targets[0].transform.GetChild(0);   // hard coded bc pain
        Transform user = input.ActiveActor().transform.GetChild(0);

        SetActiveCam(charCam);
        if (input.ActiveActor() is CharacterActor)
        {
            activeCam.m_LookAt = target;
            //activeCam.m_Follow = testTarget;
        }

        //if (input.ActiveActor() is EnemyActor) {
        //    charCam.m_LookAt = user;
        //    charCam.m_Follow = target;
        //}
        //else {
        //    charCam.m_LookAt = target;
        //    charCam.m_Follow = user;
        //}

        //SetActiveCam(charCam);
    }

    private void SetActiveCam(CinemachineVirtualCamera cam) {
        if (cam == activeCam) return;
        activeCam.m_Priority = 0;
        activeCam = cam;
        activeCam.m_Priority = 5;
    }
}
