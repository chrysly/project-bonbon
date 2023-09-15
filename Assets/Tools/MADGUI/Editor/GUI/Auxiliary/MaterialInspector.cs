using System.Reflection;
using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;

namespace ModelAssetDatabase {

    /// <summary>
    /// GUI mirroring the Material Editor from the Material Manager, with some addendums;
    /// </summary>
    public class MaterialInspector : EditorWindow {

        public static MaterialInspector ShowWindow(Material material, System.Action<bool> onChangeCallback) {
            var window = GetWindow<MaterialInspector>("Material Inspector", new System.Type[] { typeof(ModelAssetDatabaseGUI) });
            window.CleanEditor();
            window.materialInspector = MaterialEditorBundle.CreateBundle(material);
            window.onChangeCallback = onChangeCallback;
            window.isDefault = !AssetDatabase.GetAssetPath(material).StartsWith("Assets");
            return window;
        }

        /// <summary> A disposable Editor class embedded in the Editor Window to show an of an embedded inspector window; </summary>
        private MaterialEditorBundle materialInspector;

        private System.Action<bool> onChangeCallback;

        private bool isDefault;

        private static Vector2 scrollPosition;

        void OnGUI() {
            if (materialInspector is not null) {
                using (new EditorGUILayout.HorizontalScope()) {
                    using (new EditorGUILayout.VerticalScope()) {
                        GUIContent buttonContent = new GUIContent("Close Window", EditorUtils.FetchIcon("d_winbtn_win_close"));
                        if (GUILayout.Button(buttonContent, UIStyles.TextureButton)) {
                            Close();
                            GUIUtility.ExitGUI();
                        } if (isDefault) {
                            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                EditorGUILayout.HelpBox("This is a Default material. Editing is intentionally disabled. " +
                                                        "If you wish to edit default materials, enable the corresponding option in the Configuration Tab;",
                                                        MessageType.Info);
                            } GUI.enabled = false;
                        } EditorUtils.WindowBoxLabel("Material Inspector");
                        using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition)) {
                                scrollPosition = view.scrollPosition;
                                if (materialInspector.DrawEditor()) onChangeCallback.Invoke(false);
                                GUI.enabled = true;
                            } 
                        } /// Coming up with this window business...
                    }  /// Took some mental-ation...
                } /// ...
            } /// But I think it was worth it :D
        } /// Edit: Been a while since I wrote this, madre santa del amor hermoso >.>

        private void OnDisable() {
            CleanEditor();
            onChangeCallback.Invoke(true);
        }

        /// <summary>
        /// Dispose of the Material Editor;
        /// </summary>
        private void CleanEditor() => DestroyImmediate(materialInspector);
    }
}