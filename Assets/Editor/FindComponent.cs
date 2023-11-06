using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindComponent : MonoBehaviour {

    public void DeFaqAreYou() {
        var uah = FindObjectOfType(typeof(UIAnimationHandler)) as UIAnimationHandler;
        EditorGUIUtility.PingObject(uah.gameObject);
    }
}

[CustomEditor(typeof(FindComponent))]
public class FindComponentEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        FindComponent handler = (FindComponent) target;
        if (GUILayout.Button("DE FAQ ARE YOU?!")) handler.DeFaqAreYou();
    }
}
