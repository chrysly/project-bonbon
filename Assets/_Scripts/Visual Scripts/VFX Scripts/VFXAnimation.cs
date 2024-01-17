using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class VFXAnimation
{
    public enum SpawnAt {
        User,
        Target,
        Field
    }

    [Header("VFX Spawn Properties")] 
    [Tooltip("Target location of VFX")]
    public SpawnAt spawnAt = SpawnAt.User;
    
    [Tooltip("Duration before next VFX animation")]
    [Range(0, 10)] public float delay;
    
    [Tooltip("Duration of current VFX animation")]
    [Range(0, 10)] public float duration;
    
    [Tooltip("List of visual effect prefabs to spawn")]
    public List<GameObject> visualEffects;

    [Tooltip("Offset of VFX")] public Vector3 vfxOffset;

    [Tooltip("Toggle material swap. Keep material inside material to change its level.")]
    public bool doMaterialSwap;
    
    [Tooltip("Completely replaces existing material with new.")]
    public bool doMaterialReplace;
    
    [Tooltip("Material/shaders to apply on SkinnedMeshRenderer")]
    public Material material;

    [Tooltip("Duration of material")]
    public float materialDuration;

    [Tooltip("Change level value of shader")]
    public bool doMaterialLevel;

    [Tooltip("Level value of shader. Effect varies depending on first index float value of shader. y = duration")]
    public Vector2 materialLevel;
}
