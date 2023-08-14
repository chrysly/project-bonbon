using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

public class ModelAssetLibraryMaterialInspector : EditorWindow {

    public static ModelAssetLibraryMaterialInspector ShowWindow(Material material) {
        CleanMaterialInspector();
        var window = GetWindow<ModelAssetLibraryMaterialInspector>("Material Inspector", new System.Type[] { typeof(ModelAssetLibraryGUI) });
        materialInspector = (MaterialEditor) Editor.CreateEditor(material);
        isDefault = !AssetDatabase.GetAssetPath(material).StartsWith("Assets");
        return window;
    }

    /// <summary> A disposable Editor class embedded in the Editor Window to show an of an embedded inspector window; </summary>
    private static MaterialEditor materialInspector;

    private static bool isDefault;

    private static Vector2 scrollPosition;

    void OnGUI() {
        if (materialInspector != null) {
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope()) {
                    string buttonText = "Close Window" + (isDefault ? ". And, by the way, this is a default material. I made it read-only ex proffesso u.u" : "");
                    GUIContent buttonContent = new GUIContent(buttonText, EditorUtils.FetchIcon("d_winbtn_win_close"));
                    if (GUILayout.Button(buttonContent, UIStyles.TextureButton)) {
                        Close();
                        GUIUtility.ExitGUI();
                    } if (isDefault) GUI.enabled = false;
                    using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition)) {
                        scrollPosition = view.scrollPosition;
                        using (new EditorGUILayout.VerticalScope()) {
                            materialInspector.DrawHeader();
                            materialInspector.OnInspectorGUI();
                        } if (isDefault) GUI.enabled = true;
                    } /// Coming up with this window business...
                }  /// Took some mental-ation...
            } /// ...
        } /// But I think it was worth it :D
    }

    private void OnDisable() {
        CleanMaterialInspector();
        ModelAssetLibraryModelReader.SetSelectedMaterial(null);
    }

    /// <summary>
    /// Dispose of the Material Editor;
    /// </summary>
    private static void CleanMaterialInspector() {
        try {
            if (materialInspector != null) Editor.DestroyImmediate(materialInspector);
        } catch (System.NullReferenceException) {
            Debug.LogWarning("Nice Assembly Reload! Please disregard this message...");
        }
    }
}