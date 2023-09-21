using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;

namespace ModelAssetDatabase {

    /// <summary>
    /// Simple window to show a window-sized object preview;
    /// </summary>
    public class PreviewExpanded : EditorWindow {

        /// <summary>
        /// Show a separate Window with a fully expanded preview of a given GameObject;
        /// <br></br> Note that this is a shared Object Preview from the Asset Library Reader;
        /// </summary>
        /// <param name="gameObject"> GameObject to preview; </param>
        public static void ShowPreviewWindow(GameObject gameObject) {
            var window = GetWindow<PreviewExpanded>("Expanded Preview");
            window.previewObject = gameObject;
        }

        /// <summary> GameObject to show in the preview; </summary>
        private GameObject previewObject;
        private GenericPreview preview;

        void OnGUI() {
            if (previewObject == null) {
                EditorUtils.DrawScopeCenteredText("Oh, Great Lady of Assembly Reloads...\nShow us your wisdom! And reload this page...");
            } else {
                if (preview == null) preview = GenericPreview.CreatePreview(previewObject);
                preview.DrawPreview(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
        }

        void OnDisable() {
            DestroyImmediate(preview);
        }
    }
}