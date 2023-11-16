using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActorHandler))]
public class ActorHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        ActorHandler handler = target as ActorHandler;

        if (handler.PrefabMap != null) {
            if (GUILayout.Button("Launch Actor Space Editor")) LaunchActorSpaceEditor(handler);
        } else {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Assign a Prefab Map to Continue;", MessageType.Warning);
            EditorGUILayout.Space();
        }
    }

    private void LaunchActorSpaceEditor(ActorHandler handler) => ActorSpaceEditor.Launch(handler);
}