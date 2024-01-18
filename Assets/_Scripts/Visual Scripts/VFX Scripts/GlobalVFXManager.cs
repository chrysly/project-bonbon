using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using GameObject = UnityEngine.GameObject;

public class GlobalVFXManager : StateMachineHandler {

    [SerializeField] private VFXMap vfxMap;
    // [SerializeField] private VFXAnimationPackage package;
    // [SerializeField] private Animator animator;
    // [SerializeField] private Actor actor;
    public VFXMap VFXMap => vfxMap;
    
    private IEnumerator _action;
    private Queue<List<GameObject>> _activeVFXQueue = new Queue<List<GameObject>>();

    // private void Update() {
    //     if (UnityEngine.Input.GetKeyDown(KeyCode.O)) {
    //         animator.Play("_Skill1");
    //         Debug.Log("oof");
    //         PlayAnimation(package, actor.transform);
    //     }
    // }

    public void Connect(AnimationHandler animationHandler) {
        animationHandler.HealEvent += AnimationHandler_HealEvent;
        animationHandler.StaminaEvent += AnimationHandler_StaminaEvent;
        animationHandler.DamageEvent += AnimationHandler_DamageEvent;
    }

    public void PlayAnimation(VFXAnimationPackage package, Transform actor) {
        _action = AnimationAction(package, actor);
        StartCoroutine(_action);
    }
    
    public IEnumerator AnimationAction(VFXAnimationPackage package, Transform actor) {
        foreach (VFXAnimation vfxAnim in package.animationList) {
            CycleOperations(vfxAnim, actor);
            yield return new WaitForSeconds(vfxAnim.delay);
        }

        yield return new WaitForSeconds(1f);
        _action = null;
        yield return null;
    }
    
    private void CycleOperations(VFXAnimation vfxAnim, Transform actor) {
        if (vfxAnim.visualEffects.Count > 0) SpawnOperation(vfxAnim, actor);
        if (vfxAnim.material != null && vfxAnim.doMaterialSwap) ApplyShaderOperation(vfxAnim, actor);
        if (vfxAnim.material != null && vfxAnim.doMaterialReplace) ReplaceMaterialOperation(vfxAnim, actor);
        if (vfxAnim.material != null && vfxAnim.doMaterialLevel) ShaderValueOperation(vfxAnim);
        if (vfxAnim.createAfterImage && vfxAnim.afterImageMaterial != null) AfterImageOperation(vfxAnim, actor);
    }
    
    #region VFX Operations

    private void AfterImageOperation(VFXAnimation vfxAnim, Transform actor) {
        Transform meshTransform = SetTarget(vfxAnim.spawnAt, actor);
        SkinnedMeshRenderer[] skins = meshTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        Material[] materials = { vfxAnim.afterImageMaterial };
        foreach (SkinnedMeshRenderer skin in skins) {
            materials[0].SetFloat("_Level", 2.5f);
            GameObject image = Instantiate(skin.gameObject, skin.transform.position, skin.transform.rotation, skin.transform.parent);
            image.GetComponent<SkinnedMeshRenderer>().materials = materials;
            materials[0].DOFloat(20, "_Level", vfxAnim.afterImageDuration);
            Destroy(image, vfxAnim.afterImageDuration + 0.1f);
        }
    }

    private void SpawnOperation(VFXAnimation vfxAnim, Transform actor) {
        Transform spawnLocation = SetTarget(vfxAnim.spawnAt, actor);
        if (vfxAnim.visualEffects != null) {
            List<GameObject> visualEffects = new List<GameObject>();
            foreach (GameObject vfx in vfxAnim.visualEffects) {
                GameObject effectObject;
                if (vfxAnim.parentVFX) {
                    effectObject = Instantiate(vfx, spawnLocation.position, Quaternion.identity, actor);
                    effectObject.transform.rotation = spawnLocation.rotation;
                }
                else {
                    effectObject = Instantiate(vfx, spawnLocation.position, Quaternion.identity);
                }

                visualEffects.Add(effectObject);
            }
            _activeVFXQueue.Enqueue(visualEffects);
            StartCoroutine(VFXDequeue(vfxAnim.duration));
        }
    }

    private void ReplaceMaterialOperation(VFXAnimation vfxAnim, Transform actor) {
        Transform meshTransform = SetTarget(vfxAnim.spawnAt, actor);
        SkinnedMeshRenderer[] skins = meshTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        Material[] materials = { vfxAnim.material };
        foreach (SkinnedMeshRenderer skin in skins) {
            skin.materials = materials;
        }
    }

    private IEnumerator VFXDequeue(float duration) {
        yield return new WaitForSeconds(duration);
        List<GameObject> activeVFXList = _activeVFXQueue.Dequeue();
        foreach (GameObject vfx in activeVFXList) {
            Destroy(vfx);
        }
    }

    private void ApplyShaderOperation(VFXAnimation vfxAnim, Transform actor) {
        Transform meshTransform = SetTarget(vfxAnim.spawnAt, actor);
        SkinnedMeshRenderer[] skins = meshTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer skin in skins) {
            if (skin.materials[skin.materials.Length - 1] == null) {
                Material[] materials = new Material[skin.materials.Length];
                for (int i = 0; i < materials.Length; i++) {
                    materials[i] = skin.materials[i];
                }
                skin.materials[skin.materials.Length - 1] = vfxAnim.material;
                skin.materials = materials;
            }
            else {
                Material[] materials = new Material[skin.materials.Length + 1];
                for (int i = 0; i < skin.materials.Length; i++) {
                    materials[i] = skin.materials[i];
                }

                materials[materials.Length - 1] = vfxAnim.material;
                skin.materials = materials;
            }
        }
        StartCoroutine(ClearShaderOperation(vfxAnim.materialDuration, skins));
    }

    private IEnumerator ClearShaderOperation(float duration, SkinnedMeshRenderer[] skins) {
        yield return new WaitForSeconds(duration);
        foreach (SkinnedMeshRenderer skin in skins) {
            Material[] materials = new Material[skin.materials.Length - 1];
            for (int i = 0; i < materials.Length; i++) {
                materials[i] = skin.materials[i];
            }
            skin.materials = materials;
        }
        
    }

    private void ShaderValueOperation(VFXAnimation vfxAnim) {
        Material material = vfxAnim.material;
        material.DOFloat(vfxAnim.materialLevel.x, "_Level", vfxAnim.materialLevel.y);
    }
    
    
    //CHANGE TO BSM
    private Transform SetTarget(VFXAnimation.SpawnAt spawnAt, Transform actor) {
        if (spawnAt == VFXAnimation.SpawnAt.User) {
            return actor;
        }

        if (spawnAt == VFXAnimation.SpawnAt.Target) {
            return actor;
        }

        if (spawnAt == VFXAnimation.SpawnAt.Field) {
            Debug.Log("Field spawn");
        }

        return actor;
    }
    #endregion VFX Operations

    #region Events

    private void AnimationHandler_HealEvent(float heal, Actor actor) {
        PlayAnimation(VFXMap.GenericVFXDict[GenericVFXType.Heal], actor.transform);
    }

    private void AnimationHandler_StaminaEvent(int value, Actor actor) {
        if (value < 0) return;
        PlayAnimation(VFXMap.GenericVFXDict[GenericVFXType.StaminaRegen], actor.transform);
    }

    private void AnimationHandler_DamageEvent(float damage, Actor actor, bool hasBonbon) {
        PlayAnimation(VFXMap.GenericVFXDict[GenericVFXType.Damage], actor.transform);
    }

    #endregion
}
