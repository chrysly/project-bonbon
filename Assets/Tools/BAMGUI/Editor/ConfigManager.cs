using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

namespace BonbonAssetManager {
    public class ConfigManager : BonBaseTool {

        private string[] entryNames;
        private string[] assetPaths;
        private int toolLength;

        public override void Initialize() { }

        public string[] Init() {
            toolLength = MainGUI.tools.Length - 1;
            entryNames = new string[toolLength];
            for (int i = 0; i < toolLength; i++) {
                entryNames[i] = System.Enum.GetName(typeof(BAMGUI.ToolType), (BAMGUI.ToolType) i).CamelSpace() + " Path:";
            } assetPaths = LoadConfig();
            return assetPaths;
        }

        /// <summary> Path to the Configuration JSON File; </summary>
        private string ConfigPath {
            get {
                var assetGUID = AssetDatabase.FindAssets($"t:Script {nameof(ConfigManager)}");
                return AssetDatabase.GUIDToAssetPath(assetGUID[0]).RemovePathEnd("\\/") + "/BAMConfig.json";
            }
        }

        public override void ShowGUI() {
            MainGUI.DrawToolbar();
            bool invalidChanges = false;
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                for (int i = 0; i < assetPaths.Length; i++) {
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        assetPaths[i] = EditorGUILayout.TextField(entryNames[i], assetPaths[i]);
                        if (!AssetDatabase.IsValidFolder(assetPaths[i]) || assetPaths[i].EndsWith("/")) invalidChanges = true;
                        if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_Folder Icon")), GUILayout.MaxWidth(40), GUILayout.MaxHeight(18))) {
                            string res = GeneralUtils.OpenAndParseFolder();
                            if (res != null) {
                                assetPaths[i] = res;
                            } else Debug.LogWarning("The chosen path is invalid;");
                        }
                    }
                } GUI.enabled = !invalidChanges;
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    if (GUILayout.Button("Save Changes")) SaveConfig();
                } GUI.enabled = true;
            }
        }

        private struct SerializerPack {
            public string[] stringArr;
            public SerializerPack(string[] arr) {
                stringArr = arr;
            }
        }

        /// <summary>
        /// Save configuration data as a JSON string on this script's folder;
        /// </summary>
        public void SaveConfig() {
            System.Array.Copy(assetPaths, MainGUI.assetPaths, assetPaths.Length);
            string data = JsonUtility.ToJson(new SerializerPack(assetPaths));
            using StreamWriter writer = new StreamWriter(ConfigPath);
            writer.Write(data);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Load configuration data from a JSON string located in this script's folder;
        /// </summary>
        public string[] LoadConfig() {
            if (File.Exists(ConfigPath)) {
                using StreamReader reader = new StreamReader(ConfigPath);
                string data = reader.ReadToEnd();
                string[] paths = JsonUtility.FromJson<SerializerPack>(data).stringArr;
                for (int i = 0; i < paths.Length; i++) if (paths[i] == null) paths[i] = "";
                return paths;
            } else {
                string[] paths = new string[toolLength];
                for (int i = 0; i < paths.Length; i++) paths[i] = "";
                return paths;
            }
        }
    }
}