using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using static ModelAssetDatabase.ModelAssetDatabase;
using static ModelAssetDatabase.ConfigurationCore;

namespace ModelAssetDatabase {

    public class ConfigurationGUI : EditorWindow {

        [MenuItem("Tools/Model Asset Library Config")]
        public static void ShowWindow() {
            ConfigGUI = GetWindow<ConfigurationGUI>("Configuration", typeof(ModelAssetDatabaseGUI));
            if (HasOpenInstances<ModelAssetDatabaseGUI>()) {
                ModelAssetDatabaseGUI.MainGUI.Close();
            }
        }
    
        /// <summary> Reference to the Configuration Window; </summary>
        public static ConfigurationGUI ConfigGUI { get; private set; }

        /// <summary> Temporary string displayed in the text field; </summary>
        public static string potentialPath;

        /// <summary> Temporary File Extension string displayed in the text field; </summary>
        public static string potentialExtensions;

        private static Vector2 scrollPosition;

        void OnEnable() {
            LoadConfig();
            potentialPath = Config.rootAssetPath;
            potentialExtensions = Config.modelFileExtensions;
        }

        /// <summary>
        /// Refresh the Window reference if null;
        /// </summary>
        private void OnFocus() {
            if (ConfigGUI == null && HasOpenInstances<ConfigurationGUI>()) {
                ConfigGUI = GetWindow<ConfigurationGUI>();
            }
        }

        void OnGUI() {
            using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                bool configIsInvalid = false;
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxHeight(100))) {
                    EditorUtils.DrawSeparatorLines("Library Settings", true);
                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.HorizontalScope()) {
                        potentialPath = EditorGUILayout.TextField("Root Asset Path", potentialPath);
                        if (AssetDatabase.IsValidFolder(potentialPath) && !potentialPath.EndsWith("/")) {
                            if (potentialPath != Config.rootAssetPath) {
                                UpdateRootAssetPath(potentialPath);
                            }
                        } else configIsInvalid = true;
                        if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_Folder Icon")), GUILayout.MaxWidth(40), GUILayout.MaxHeight(18 ))) {
                            string res = OpenAndParseFolder();
                            if (res != null) {
                                UpdateRootAssetPath(res);
                                potentialPath = res;
                            } else Debug.LogWarning("The chosen path is invalid;");
                        }
                    } GUILayout.FlexibleSpace();
                    potentialExtensions = EditorGUILayout.TextField("Model File Extension(s)", potentialExtensions);
                    if (!string.IsNullOrWhiteSpace(potentialExtensions)) {
                        UpdateModelExtension(potentialExtensions);
                    } else configIsInvalid = true;
                    GUILayout.FlexibleSpace();
                    if (configIsInvalid) GUI.enabled = false;
                    if (GUILayout.Button("Save Changes")) SaveConfig();
                    if (configIsInvalid) GUI.enabled = true;
                } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxHeight(100))) {
                    EditorUtils.DrawSeparatorLines("Data & Documentation", true);
                    GUILayout.FlexibleSpace();
                    if (configIsInvalid) GUI.enabled = false;
                    if (GUILayout.Button("Open Asset Library")) {
                        ModelAssetDatabaseGUI.ShowWindow(); 
                    } if (GUILayout.Button("Reload Asset Library")) {
                        Refresh();
                    } if (configIsInvalid) GUI.enabled = true;
                    if (GUILayout.Button("Open Documentation")) {
                        Debug.Log("There's no documentation to show here... YET! >:)");
                    }
                }
            } using (new EditorGUILayout.VerticalScope(new GUIStyle(GUI.skin.box) { padding = new RectOffset(20, 20, 0, 0)})) {
                EditorUtils.DrawSeparatorLines("Library Data", true);
                using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition)) {
                    scrollPosition = view.scrollPosition;
                    EditorUtils.DrawSeparatorLines("Model Data Dictionary");
                    BuildMDDictionary(ModelDataDict);

                    EditorUtils.DrawSeparatorLines("Prefab Data Dictionary");
                    BuildPDDictionary(PrefabDataDict);

                    EditorUtils.DrawSeparatorLines("Model - Prefab Association");
                    BuildM2PDictionary(ModelDataDict);
                }
            }
        }

        /// <summary>
        /// Displays data on the Model Data Dictionary;
        /// </summary>
        /// <param name="dict"> Model Data Dictionary; </param>
        private void BuildMDDictionary(Dictionary<string, ModelData> dict) {
            if (dict == null || dict.Keys.Count == 0) {
                EmptyLabel();
                return;
            } foreach (KeyValuePair<string, ModelData> kvp in dict) {
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.LabelField(AssetDatabase.GUIDToAssetPath(kvp.Key).IsolatePathEnd("\\/").RemovePathEnd("."), GUILayout.Width(160));
                    EditorGUILayout.LabelField(kvp.Value.path, GUILayout.MinWidth(EditorUtils.MeasureTextWidth(kvp.Value.path, GUI.skin.font) + 16));
                }
            }
        }

        /// <summary>
        /// Displays data on the Prefab Data Dictionary;
        /// </summary>
        /// <param name="dict"> Prefab Data Dictionary; </param>
        private void BuildPDDictionary(Dictionary<string, PrefabData> dict) {
            if (dict == null || dict.Keys.Count == 0) {
                EmptyLabel();
                return;
            } foreach (KeyValuePair<string, PrefabData> kvp in dict) {
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.LabelField(AssetDatabase.GUIDToAssetPath(kvp.Key).IsolatePathEnd("\\/").RemovePathEnd("."), GUILayout.Width(160));
                    EditorGUILayout.LabelField(kvp.Value.path, GUILayout.MinWidth(EditorUtils.MeasureTextWidth(kvp.Value.path, GUI.skin.font) + 16));
                }
            }
        }

        /// <summary>
        /// Displays the prefab correlation lists in the Model Data Dictionary;
        /// </summary>
        /// <param name="dict"> Model Data Dictionary; </param>
        private void BuildM2PDictionary(Dictionary<string, ModelData> dict) {
            if (dict == null || dict.Keys.Count == 0) {
                EmptyLabel();
                return;
            } foreach (KeyValuePair<string, ModelData> kvp in dict) {
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.LabelField(AssetDatabase.GUIDToAssetPath(kvp.Key).IsolatePathEnd("\\/").RemovePathEnd("."), GUILayout.Width(160));
                    var listString = "";
                    foreach (string str in kvp.Value.prefabIDList) {
                        listString += AssetDatabase.GUIDToAssetPath(str).IsolatePathEnd("\\/").RemovePathEnd(".") + " | ";
                    } if (string.IsNullOrWhiteSpace(listString)) listString = "-|";
                    EditorGUILayout.LabelField(listString.RemovePathEnd("|"));
                }
            }
        }

        private void EmptyLabel() => EditorGUILayout.LabelField(" - Empty - ", UIStyles.ItalicLabel);
    }
}