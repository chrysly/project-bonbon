using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VectorMathVisualizer))]
public class VectorMathEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        VectorMathVisualizer handler = (VectorMathVisualizer) target;
        if (GUILayout.Button("Get Forward")) handler.ReturnForward();
    }
}
