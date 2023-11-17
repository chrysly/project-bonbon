using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraAnimation {
    [Header("Camera Focus Properties")]

    [Tooltip("Switches LookAt target to capture entire battlefield")]
    [Range(0, 10)] public float delay;

    public enum LookAt {
        User,
        Target,
        Field,
        NoChange
    }

    [Tooltip("Switches LookAt target of camera.")]
    public LookAt lookAt = LookAt.NoChange;

    [Header("Camera Transform Properties")]
    public bool doOffset;

    [Tooltip("Offset of camera, z = duration")] public Vector4 offset;
    
    public bool doRotation;
    [Tooltip("Rotation of camera, z = duration")] public Vector4 rotation;
    
    [Header("Camera View Properties")]
    public bool doFOV;
    [Tooltip("FOV of camera, y = duration")] public Vector2 fov;

    public bool doDutch;
    [Tooltip("Dutch angle of camera, y = duration")] public Vector2 dutch;

    [Header("Camera Effects")] 
    public bool doScreenShake;

    [Tooltip("Shake velocity")] public Vector3 shakeVelocity;
    [Tooltip("x = Attack, y = Sustain, z = Decay")]
    public Vector3 shakeTimeCurve;
}
