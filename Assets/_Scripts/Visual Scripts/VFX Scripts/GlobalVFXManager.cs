using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using GameObject = UnityEngine.GameObject;

public class GlobalVFXManager : MonoBehaviour {
    //[SerializeField] private VFXAnimationPackage package;
    public Transform target;
    public Transform user;
    
    // Update is called once per frame
    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) {
            PlayAnimation(package);
        }
    }*/
    
    private IEnumerator _action;
    private Queue<List<GameObject>> _activeVFXQueue = new Queue<List<GameObject>>();
    
    public void PlayAnimation(VFXAnimationPackage package) {
        if (_action == null) {
            _action = AnimationAction(package);
            StartCoroutine(_action);
        }
    }
    
    public IEnumerator AnimationAction(VFXAnimationPackage package) {
        foreach (VFXAnimation vfxAnim in package.animationList) {
            CycleOperations(vfxAnim);
            yield return new WaitForSeconds(vfxAnim.delay);
        }

        yield return new WaitForSeconds(1f);
        _action = null;
        yield return null;
    }
    
    private void CycleOperations(VFXAnimation vfxAnim) {
        if (vfxAnim.visualEffects.Count > 0) SpawnOperation(vfxAnim);
        if (vfxAnim.material != null && vfxAnim.doMaterialSwap) ApplyShaderOperation(vfxAnim);
        if (vfxAnim.material != null && vfxAnim.doMaterialReplace) ReplaceMaterialOperation(vfxAnim);
        if (vfxAnim.material != null && vfxAnim.doMaterialLevel) ShaderValueOperation(vfxAnim);
    }
    
    #region VFX Operations

    private void SpawnOperation(VFXAnimation vfxAnim) {
        Transform spawnLocation = SetTarget(vfxAnim.spawnAt);
        if (vfxAnim.visualEffects != null) {
            List<GameObject> visualEffects = new List<GameObject>();
            foreach (GameObject vfx in vfxAnim.visualEffects) {
                GameObject effectObject = Instantiate(vfx, spawnLocation.position, Quaternion.identity);
                visualEffects.Add(effectObject);
            }
            _activeVFXQueue.Enqueue(visualEffects);
            StartCoroutine(VFXDequeue(vfxAnim.duration));
        }
    }

    private void ReplaceMaterialOperation(VFXAnimation vfxAnim) {
        Transform meshTransform = SetTarget(vfxAnim.spawnAt);
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

    private void ApplyShaderOperation(VFXAnimation vfxAnim) {
        Transform meshTransform = SetTarget(vfxAnim.spawnAt);
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
    private Transform SetTarget(VFXAnimation.SpawnAt spawnAt) {
        if (spawnAt == VFXAnimation.SpawnAt.User) {
            return user;
        }

        if (spawnAt == VFXAnimation.SpawnAt.Target) {
            return target;
        }

        if (spawnAt == VFXAnimation.SpawnAt.Field) {
            Debug.Log("Field spawn");
        }

        return target;
    }
    #endregion VFX Operations
}
