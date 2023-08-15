using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ModelAssetLibraryConfigurationCore {

    /// <summary> Path to the Configuration JSON File; </summary>
    public static string ConfigPath {
        get {
            var assetGUID = AssetDatabase.FindAssets($"t:Script {nameof(ModelAssetLibraryConfigurationCore)}");
            return AssetDatabase.GUIDToAssetPath(assetGUID[0]).RemovePathEnd("\\/") + "/Config.json";
        }
    }

    /// <summary> Collection of assets used by the tool GUI; </summary>
    public static ModelAssetLibraryAssets ToolAssets {
        get {
            var assetGUID = AssetDatabase.FindAssets($"t:ModelAssetLibraryAssets {nameof(ModelAssetLibraryAssets)}");
            return AssetDatabase.LoadAssetAtPath<ModelAssetLibraryAssets>(AssetDatabase.GUIDToAssetPath(assetGUID[0]));
        }
    }

    /// <summary> Configuration struct to save and load through JSON; </summary>
    public struct Configuration {
        public string rootAssetPath;
        public string dictionaryDataPath;
        public string modelFileExtensions;
    } public static Configuration Config = new Configuration();

    /// <summary> Path to the root of the folder hierarchy where the library will search for assets; </summary>
    public static string RootAssetPath { get { return Config.rootAssetPath; } }

    /// <summary>
    /// File extension of the assets to look for (without the dot);
    /// </summary>
    public static string ModelFileExtensions { get { return Config.modelFileExtensions; } }

    public static string OpenAndParseFolder() {
        string res = EditorUtility.OpenFolderPanel("Set Root Path", "Assets", "");
        if (res != null && res.StartsWith(Application.dataPath)) {
            res = "Assets" + res.Substring(Application.dataPath.Length);
            return res;
        } else return null;
    }

    /// <summary>
    /// Replace the Root Asset Path statically. The path still needs to be saved;
    /// </summary>
    /// <param name="newAssetPath"> Asset path to use as a root for the Model Asset Library; </param>
    public static void UpdateRootAssetPath(string newAssetPath) {
        Config.rootAssetPath = newAssetPath.Trim('/').Replace("\\", "/");
        ModelAssetLibrary.Refresh();
    }

    public static void UpdateModelExtension(string newExtensions) {
        Config.modelFileExtensions = newExtensions;
        ModelAssetLibrary.Refresh();
    }

    /// <summary>
    /// Save configuration data as a JSON string on this script's folder;
    /// </summary>
    public static void SaveConfig() {
        string data = JsonUtility.ToJson(Config);
        using StreamWriter writer = new StreamWriter(ConfigPath);
        writer.Write(data);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Load configuration data from a JSON string located in this script's folder;
    /// </summary>
    public static void LoadConfig() {
        if (File.Exists(ConfigPath)) {
            using StreamReader reader = new StreamReader(ConfigPath);
            string data = reader.ReadToEnd();
            Config = JsonUtility.FromJson<Configuration>(data);
        } else {
            Config = new Configuration();
        }
    }
}