using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GlobalCameraManager : MonoBehaviour {
    public CinemachineVirtualCamera staticCamera;
    public CinemachineVirtualCamera dynamicCamera;
    private CinemachineVirtualCamera _activeCamera;
    
    //FOR TESTING
    [SerializeField] private CameraAnimationPackage cameraPackage;
    
    private BattleStateInput _data;

    public void PlayAnimation() {
        
    }

    private void ToggleDynamicCamera(bool enabled) {
        if (enabled) {
            dynamicCamera.m_Priority = 10;
            staticCamera.m_Priority = 0;
        } else {
            staticCamera.m_Priority = 10;
            
        }
    }
    
    
}
