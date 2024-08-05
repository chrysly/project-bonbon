using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class GlobalCameraManager : StateMachineHandler {
    public CinemachineVirtualCamera staticCamera;
    public CinemachineVirtualCamera dynamicCamera;
    public MMF_Player impulse;
    //[SerializeField] private Transform fieldTarget;
    
    
    private BattleStateInput _data;
    private Transform _followTarget;
    private Transform _lookTarget;

    private IEnumerator _action;

    public CameraAnimationPackage cameraPackage;
    public Animator animator;

    public void Start() {
        _followTarget = new GameObject("Follow Target").transform;
        _lookTarget = new GameObject("Look Target").transform;
        dynamicCamera.m_LookAt = _lookTarget;
        dynamicCamera.m_Follow = _followTarget;
        _followTarget.position = staticCamera.transform.position;
    }

    public void Update() {
        if (UnityEngine.Input.GetKeyDown(KeyCode.C)) {
            PlayAnimation(cameraPackage);
            animator.Play("_Skill1");
        }
    }

    public void PlayAnimation(CameraAnimationPackage package) {
        if (_action == null) {
            Debug.Log("Camera activated");
            _action = AnimationAction(package);
            StartCoroutine(_action);
        }
    }
    
    public IEnumerator AnimationAction(CameraAnimationPackage package) {
        ToggleDynamicCamera(true);
        foreach (CameraAnimation camAnim in package.animationList) {
            CycleOperations(camAnim);
            yield return new WaitForSeconds(camAnim.delay);
        }

        yield return new WaitForSeconds(1f);
        ToggleDynamicCamera(false);
        _action = null;
        yield return null;
    }

    #region Camera Operations
    private void ToggleDynamicCamera(bool isEnabled) {
        if (isEnabled) {
            ResetDynamicCamera();
            //_lookTarget.position = animator.transform.GetChild(0).position; //Potato
            dynamicCamera.m_Priority = 10;
            staticCamera.m_Priority = 0;
        } else {
            staticCamera.m_Priority = 10;
            dynamicCamera.m_Priority = 0;
        }
    }

    private void ResetDynamicCamera() {
        _followTarget.position = staticCamera.transform.position;
        dynamicCamera.m_Follow.rotation = new Quaternion(0, 0, 0, 0);
        dynamicCamera.m_Lens.FieldOfView = staticCamera.m_Lens.FieldOfView;
        dynamicCamera.m_Lens.Dutch = staticCamera.m_Lens.Dutch;
    }

    private void CycleOperations(CameraAnimation camAnim) {
        if (camAnim.lookAt != CameraAnimation.LookAt.NoChange) LookAtOperation(camAnim.lookAt);
        if (camAnim.doOffset) OffsetOperation(camAnim.offset);
        if (camAnim.doRotation) RotationOperation(camAnim.rotation);
        if (camAnim.doFOV) FOVOperation(camAnim.fov);
        if (camAnim.doDutch) DutchOperation(camAnim.dutch);
        if (camAnim.doScreenShake) ScreenShakeOperation(camAnim.shakeVelocity, camAnim.shakeTimeCurve);
    }

    private void LookAtOperation(CameraAnimation.LookAt lookAt) {
        switch (lookAt) {
            case CameraAnimation.LookAt.Field:
                //_lookTarget.position = fieldTarget.position;
                break;
            case CameraAnimation.LookAt.Target:
                //_lookTarget.position = BattleStateMachine.Instance.CurrInput.SkillPrep.targets[0].transform.GetChild(0).transform.position;
                break;
            case CameraAnimation.LookAt.User:
                _lookTarget.position = BattleStateMachine.Instance.CurrInput.ActiveActor().transform.GetChild(0).transform.position;
                break;
        }
    }

    private void OffsetOperation(Vector4 operation) {
        _followTarget.DOMove((_lookTarget.position - (Vector3) operation), operation.w);
    }

    private void RotationOperation(Vector4 operation) {
        _followTarget.DORotate(operation, operation.z);
    }

    private void FOVOperation(Vector2 operation) {
        DOTween.To(() => dynamicCamera.m_Lens.FieldOfView, x => dynamicCamera.m_Lens.FieldOfView = x, operation.x,
            operation.y);
    }
    
    private void DutchOperation(Vector2 operation) {
        DOTween.To(() => dynamicCamera.m_Lens.Dutch, x => dynamicCamera.m_Lens.Dutch = x, operation.x,
            operation.y);
    }
    
    private void ScreenShakeOperation(Vector3 vector, Vector3 timing) {
        impulse.PlayFeedbacks();
    }

    private IEnumerator ScreenShakeAction(Vector3 vector, Vector3 timing) {
        yield return null;
    }
    
    #endregion Camera Operations
    
}
