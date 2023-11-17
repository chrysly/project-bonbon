using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class GlobalCameraManager : MonoBehaviour {
    public CinemachineVirtualCamera staticCamera;
    public CinemachineVirtualCamera dynamicCamera;
    
    //FOR TESTING
    [SerializeField] private CameraAnimationPackage cameraPackage;
    
    private BattleStateInput _data;
    private Transform _followTarget;
    private Transform _lookTarget;

    private IEnumerator _action;

    public void Start() {
        _followTarget = dynamicCamera.m_Follow;
        _lookTarget = dynamicCamera.m_LookAt;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            PlayAnimation();
        }
    }

    public void PlayAnimation() {
        if (_action == null) {
            Debug.Log("Camera activated");
            _action = AnimationAction();
            StartCoroutine(_action);
        }
    }
    
    public IEnumerator AnimationAction() {
        ToggleDynamicCamera(true);
        int runs = 0;
        foreach (CameraAnimation camAnim in cameraPackage.animationList) {
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
            dynamicCamera.m_Priority = 10;
            staticCamera.m_Priority = 0;
        } else {
            staticCamera.m_Priority = 10;
            dynamicCamera.m_Priority = 0;
        }
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
        
    }

    private void OffsetOperation(Vector4 operation) {
        Debug.Log("Moved");
        _followTarget.DOMove(_lookTarget.position - (Vector3) operation, operation.w);
    }
    
    private void RotationOperation(Vector4 operation) {
    }

    private IEnumerator RotationAction(Vector4 operation) {
        yield return null;
    }
    
    private void FOVOperation(Vector2 operation) {
        
    }

    private IEnumerator FOVAction(Vector2 operation) {
        yield return null;
    }
    
    private void DutchOperation(Vector2 operation) {
        
    }

    private IEnumerator DutchAction(Vector2 operation) {
        yield return null;
    }
    
    private void ScreenShakeOperation(Vector3 vector, Vector3 timing) {
        
    }

    private IEnumerator ScreenShakeAction(Vector3 vector, Vector3 timing) {
        yield return null;
    }
    
    #endregion Camera Operations
    
}
