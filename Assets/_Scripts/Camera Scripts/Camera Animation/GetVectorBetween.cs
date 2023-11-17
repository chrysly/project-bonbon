using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class GetVectorBetween : MonoBehaviour {
    [SerializeField] private Transform lookTarget;

    [SerializeField] private Transform followTarget;

    public Vector3 difference;

    public void CalculateVector() {
        if (lookTarget != null && followTarget != null) {
            difference = lookTarget.position - followTarget.position;
        }
    }
}

[CustomEditor(typeof(GetVectorBetween))]
public class VectorBetweenEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GetVectorBetween handler = (GetVectorBetween) target;
        if (GUILayout.Button("Calculate Difference")) handler.CalculateVector();
    }
}
