using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScaleHelper : MonoBehaviour
{
    void OnValidate() {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material material = meshRenderer.sharedMaterial;
        material.SetVector("minBounds", meshRenderer.bounds.min);
        material.SetVector("maxBounds", meshRenderer.bounds.max);
    }
}
