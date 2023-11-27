using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GetVectorBetween))]
public class VectorBetweenEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GetVectorBetween handler = (GetVectorBetween) target;
        if (GUILayout.Button("Calculate Difference")) handler.CalculateVector();
    }
}
