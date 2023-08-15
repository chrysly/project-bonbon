using UnityEngine;
using UnityEditor;

/// <summary>
/// Dropdown window with additional mesh preview settings;
/// </summary>
public class ModelAssetLibraryExtraMeshPreview : EditorWindow {
    
    /// <summary>
    /// Show a dropdown window with additional settings and options for the active mesh preview;
    /// </summary>
    /// <param name="mp"> Active mesh preview; </param>
    /// <param name="rect"> Rect where the dropdown will be created; </param>
    public static void ShowPreviewSettings(MeshPreview mp, Rect rect) {
        var window = GetWindow<ModelAssetLibraryExtraMeshPreview>(true);
        window.ShowAsDropDown(rect, new Vector2(320, 40));
        meshPreview = mp;
    }

    /// <summary> Active mesh preview; </summary>
    private static MeshPreview meshPreview;

    void OnGUI() {
        GUIStyle previewStyle = new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter };
        using (new EditorGUILayout.HorizontalScope(previewStyle)) {
            meshPreview.OnPreviewSettings();
        }
    }

    void OnDisable() {
        meshPreview = null;
    }
}