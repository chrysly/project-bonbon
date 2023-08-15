using UnityEngine;
using UnityEditor;
using CJUtils;

/// <summary>
/// Simple window to show a window-sized object preview;
/// </summary>
public class ModelAssetLibraryPreviewExpanded : EditorWindow {

    /// <summary>
    /// Show a separate Window with a fully expanded preview of a given GameObject;
    /// <br></br> Note that this is a shared Object Preview from the Asset Library Reader;
    /// </summary>
    /// <param name="gameObject"> GameObject to preview; </param>
    public static void ShowPreviewWindow(GameObject gameObject) {
        previewObject = gameObject;
        GetWindow<ModelAssetLibraryPreviewExpanded>("Expanded Preview");
    }

    /// <summary> GameObject to show in the preview; </summary>
    private static GameObject previewObject;

    void OnGUI() {
        if (previewObject == null) {
            EditorUtils.DrawScopeCenteredText("Oh, Great Lady of Assembly Reloads...\nShow us your wisdom! And reload this page...");
            return;
        } ModelAssetLibraryModelReader.DrawObjectPreviewEditor(previewObject, position.width, position.height);
    }

    void OnDisable() {
        ModelAssetLibraryModelReader.CleanObjectPreview();
    }
}