using CJUtils;
using UnityEditor;
using UnityEngine;

namespace ModelAssetDatabase {

    /// <summary> Editor Window to confirm Prefab Variant Deletion; </summary>
    public class ModalPrefabDeletion : EditorWindow {

        /// <summary>
        /// Confirms whether a Prefab Deletion Action should be performed;
        /// </summary>
        /// <param name="prefabName"> Name to display in the confirmation window; </param>
        /// <returns> True if the asset should be deleted, false otherwise; </returns>
        public static bool ConfirmPrefabDeletion(string prefabName) {
            fileName = prefabName;
            var window = GetWindow<ModalPrefabDeletion>("Prefab Variant Deletion");
            window.maxSize = new Vector2(350, 105);
            window.minSize = window.maxSize;
            window.ShowModal();
            return result;
        }

        /// <summary> Name to display in the confirmation window; </summary>
        private static string fileName;
        /// <summary> Result to return from the modal window; </summary>
        private static bool result;

        void OnGUI() {
            using (new EditorGUILayout.VerticalScope(UIStyles.MorePaddingScrollView)) {
                GUILayout.Label("Are you sure you want to delete the following asset?", UIStyles.CenteredLabel);
                EditorGUILayout.Separator();
                GUI.color = UIColors.Red;
                GUILayout.Label(fileName, UIStyles.CenteredLabel);
                EditorGUILayout.Separator();
                using (new EditorGUILayout.HorizontalScope()) {
                    if (GUILayout.Button("Delete")) {
                        result = true;
                        Close();
                    } GUI.color = Color.white;
                    if (GUILayout.Button("Cancel")) {
                        result = false;
                        Close();
                    }
                }
            }
        }
    }

    /// <summary> Editor Window to confirm Material Slot Changes; </summary>
    public class ModalMaterialChanges : EditorWindow {

        /// <summary>
        /// Confirms whether the Material Changes should be applied or discarded;
        /// </summary>
        /// <returns> True if the changes should be applied, false otherwise; </returns>
        public static bool ConfirmMaterialChanges() {
            var window = GetWindow<ModalMaterialChanges>("Unapplied Material Changes");
            window.maxSize = new Vector2(350, 85);
            window.minSize = window.maxSize;
            window.ShowModal();
            return result;
        }
    
        /// <summary> Result to return from the modal window; </summary>
        private static bool result;

        void OnGUI() {
            using (new EditorGUILayout.VerticalScope(UIStyles.MorePaddingScrollView)) {
                GUILayout.Label("You have unapplied material changes.\n" +
                    "What would you like to do with them?", UIStyles.CenteredLabel);
                EditorGUILayout.Separator();
                using (new EditorGUILayout.HorizontalScope()) {
                    GUI.color = UIColors.Green;
                    if (GUILayout.Button("Apply Changes")) {
                        result = true;
                        Close();
                    }
                    GUI.color = UIColors.Red;
                    if (GUILayout.Button("Dicard Changes")) {
                        result = false;
                        Close();
                    }
                }
            }
        }
    }
}