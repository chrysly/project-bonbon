using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using static ModelAssetLibraryGUI;
using ModelReader = ModelAssetLibraryModelReader;
using PrefabOrganizer = ModelAssetLibraryPrefabOrganizer;

/// <summary> Core class of the Model Asset Library;
/// <br></br> Reads the folder directory and generates interactable Hierarchy Previews; </summary>
public class ModelAssetLibraryHierarchyBuilder {

    /// <summary> Subfolder and File Paths + Foldout Scope of a folder in the model hierarchy; </summary>
    public class FolderData {
        public string name;
        public List<string> subfolders;
        public List<string> files;
        public bool foldout = true;
    } /// <summary> Dictionary that maps each folder path in the hierarchy to some useful information; </summary>
    public static Dictionary<string, FolderData> FolderMap { get; private set; }

    /// <summary> Sorted list of all identified models for the search function; </summary>
    private static List<string> modelList;

    /// <summary> Sorted list of all identified model-containing folders for the search function; </summary>
    private static List<string> folderList;

    /// <summary> Currently selected model path in the hierarchy; </summary>
    private static string SelectedModelPath { get { return ModelReader.Model != null ? ModelReader.Model.assetPath : null; } }

    /// <summary> Currently selected category path in the hierarchy; </summary>
    private static string SelectedCategoryPath { get { return PrefabOrganizer.SelectedCategory; } }

    /// GUI variables;
    private static string searchString;

    /// <summary>
    /// Loads the data required to generate Hierarchy Previews;
    /// </summary>
    public static void InitializeHierarchyData() {
        modelList = new List<string>();
        folderList = new List<string>();
        FolderMap = BuildFolderMap(ModelAssetLibrary.RootAssetPath, false);
        modelList.Sort((name1, name2) => name1.IsolatePathEnd("\\/").CompareTo(name2.IsolatePathEnd("\\/")));
        folderList.Sort((name1, name2) => name1.IsolatePathEnd("\\/").CompareTo(name2.IsolatePathEnd("\\/")));
    }

    /// <summary>
    /// Unloads all static data;
    /// </summary>
    public static void FlushHierarchyData() {
        modelList = null;
        folderList = null;
        FolderMap = null;
    }

    /// <summary>
    /// Iterates through the directories in the target path to build a dictionary tree;
    /// <br></br> This method is recursive and will traverse the full depth of the target folder hierarchy;
    /// </summary>
    /// <param name="path"> The path to the root folder where the search should begin; </param>
    /// <param name="externalCall"> Whether the function is called outside of the Hierarchy Builder; </param>
    /// <param name="newFolderMap"> Recursive variable; </param>
    public static Dictionary<string, FolderData> BuildFolderMap(string path, bool externalCall = true, Dictionary<string, FolderData> newFolderMap = null) {
        path = path.Replace('\\', '/');
        if (newFolderMap == null) newFolderMap = new Dictionary<string, FolderData>();
        newFolderMap[path] = new FolderData();
        List<string> subfolders = new List<string>(AssetDatabase.GetSubFolders(path));
        List<string> files = new List<string>(ModelAssetLibrary.FindAssets(path, ModelAssetLibrary.ModelFileExtensions));
        for (int i = 0; i < files.Count; i++) files[i] = files[i].Replace('\\', '/');
        if (subfolders.Count > 0 || files.Count > 0) {
            newFolderMap[path].name = path.IsolatePathEnd("\\/");
            newFolderMap[path].subfolders = new List<string>(subfolders);
            newFolderMap[path].files = files;
            foreach (string subfolder in subfolders) {
                BuildFolderMap(subfolder, externalCall, newFolderMap);
            } if (!externalCall) {
                modelList.AddRange(files);
                if (files.Count > 0) folderList.Add(path);
            }
        } else {
            string parentPath = path.RemovePathEnd("\\/");
            newFolderMap[parentPath].subfolders.Remove(path);
            newFolderMap.Remove(path);
        } return newFolderMap;
    }

    /// <summary>
    /// Sets a Selected Model in the Model Reader;
    /// </summary>
    /// <param name="path"> Path of the Model Importer; </param>
    private static void SetSelectedModel(string path) { if (SelectedModelPath != path) ModelReader.SetSelectedModel(path); }

    /// <summary>
    /// Sets a Selected Category in the Prefab Organizer;
    /// </summary>
    /// <param name="path"> Path of the folder; </param>
    private static void SetSelectedPrefabFolder(string path) { if (SelectedCategoryPath != path) PrefabOrganizer.SetSelectedCategory(path); }

    /// <summary>
    /// Generates a Results List using the Search String obtained through the Hierarchy Search Bar; 
    /// </summary>
    /// <param name="searchString"> Search String to process; </param>
    /// <returns> A list containing all matching results depending on the active tool; </returns>
    private static List<string> GetSearchQuery(string searchString, ToolMode toolMode) {
        switch (toolMode) {
            case ToolMode.ModelReader:
                return modelList.FindAll((str) => str.Contains(searchString));
            case ToolMode.PrefabOrganizer:
                return folderList.FindAll((str) => str.Contains(searchString));
            case ToolMode.MaterialManager:
                return null;
        } return null;
    }

    // GUI

    /// <summary>
    /// Show a Hierarchy Preview applicable to the current tool;
    /// </summary>
    public static void DisplayToolDirectory(ToolMode toolMode) {

        switch (toolMode) {
            case ToolMode.ModelReader:
                DisplayModelReaderDirectory();
                break;
            case ToolMode.PrefabOrganizer:
                DisplayPrefabOrganizerDirectory();
                break;
            case ToolMode.MaterialManager:
                DisplayMaterialManagerDirectory();
                break;
        }
    }

    /// <summary>
    /// Display a Hierarchy Preview suitable for the Model Reader;
    /// <br></br> Shows a List of filtered buttons if there's an active search query;
    /// </summary>
    private static void DisplayModelReaderDirectory() {
        if (string.IsNullOrWhiteSpace(searchString)) {
            DrawModelDictionary(ModelAssetLibrary.RootAssetPath);
        } else DrawSearchQuery(searchString, ToolMode.ModelReader);
    }

    /// <summary>
    /// Display a Hierarchy Preview suitable for the Prefab Organizer;
    /// <br></br> Shows a List of filtered buttons if there's an active search query;
    /// </summary>
    private static void DisplayPrefabOrganizerDirectory() {
        if (string.IsNullOrWhiteSpace(searchString)) {
            DrawPrefabFolderDictionary(ModelAssetLibrary.RootAssetPath);
        } else DrawSearchQuery(searchString, ToolMode.PrefabOrganizer);
    }

    private static void DisplayMaterialManagerDirectory() {

    }

    #region | Search Bar |

    /// <summary>
    /// Draws the Search Bar atop the Hierarchy Preview;
    /// </summary>
    public static void DrawSearchBar() {
        using (new EditorGUILayout.HorizontalScope(UIStyles.PaddedToolbar)) {
            searchString = EditorGUILayout.TextField(searchString, EditorStyles.toolbarSearchField);
        }
    }

    /// <summary>
    /// Draws contents based on the current Search String;
    /// </summary>
    /// <param name="searchString"> Search String to filter the contents with; </param>
    /// <param name="toolMode"> Active tool type; </param>
    private static void DrawSearchQuery(string searchString, ToolMode toolMode) {
        switch (toolMode) {
            case ToolMode.ModelReader:
                List<string> filteredFileList = GetSearchQuery(searchString, toolMode);
                foreach (string file in filteredFileList) DrawModelButton(file);
                break;
            case ToolMode.PrefabOrganizer:
                List<string> filteredFolderList = GetSearchQuery(searchString, toolMode);
                foreach (string folder in filteredFolderList) DrawPrefabFolderButton(folder, false);
                break;
        }
    }

    #endregion

    /// <summary>
    /// Draws a folder + model hierarchy on the left-hand interface;
    /// </summary>
    /// <param name="path"> Path to the root folder where the hierarchy begins;
    /// <br></br> Note: The root folder path will be included in the hierarchy; </param>
    private static void DrawModelDictionary(string path) {

        if (FolderMap[path].subfolders.Count > 0 || FolderMap[path].files.Count > 0) {
            GUIContent foldoutContent = new GUIContent(" " + path.IsolatePathEnd("/\\"),
                                                   EditorUtils.FetchIcon(FolderMap[path].foldout ? "d_FolderOpened Icon" : "d_Folder Icon"));
            FolderMap[path].foldout = EditorGUILayout.Foldout(FolderMap[path].foldout, foldoutContent);
        } EditorGUI.indentLevel++;

        if (FolderMap[path].foldout) {
            foreach (string subfolder in FolderMap[path].subfolders) {
                DrawModelDictionary(subfolder);
                EditorGUI.indentLevel--;
            } foreach (string file in FolderMap[path].files) DrawModelButton(file);
        } 
    }

    /// <summary>
    /// Draws a button corresponding to model file in the hierarchy;
    /// </summary>
    /// <param name="path"> Path to the file; </param>
    private static void DrawModelButton(string path) {
        bool selected = path == SelectedModelPath;
        GUIStyle buttonStyle = selected ? UIStyles.HButtonSelected : UIStyles.HButton;
        string extension = path.IsolatePathEnd(".");
        string fileName = path.IsolatePathEnd("\\/").Replace(extension, extension.ToUpper());
        float width = EditorUtils.MeasureTextWidth(fileName, GUI.skin.font);
        var data = ModelAssetLibraryExtManager.FetchExtData(AssetDatabase.AssetPathToGUID(path));
        GUIContent modelContent;
        Texture2D icon;
        if (data != null) {
            if (selected) icon = EditorUtils.FetchIcon(data.isModel ? "d_PrefabModel Icon" : "AvatarSelector");
            else icon = EditorUtils.FetchIcon(data.isModel ? "d_PrefabModel On Icon" : "AvatarMask On Icon");
        } else {
            if (selected) icon = EditorUtils.FetchIcon("d_ScriptableObject Icon");
            else icon = EditorUtils.FetchIcon("d_ScriptableObject On Icon");
        } modelContent = new GUIContent(fileName, icon);
        if (GUILayout.Button(modelContent, buttonStyle, GUILayout.Width(width + 29), GUILayout.Height(20))) SetSelectedModel(path);
    }

    /// <summary>
    /// Draws a folder hierarchy on the left-hand interface;
    /// </summary>
    /// <param name="path"> Path to the root folder where the hierarchy begins;
    /// <br></br> Note: The root folder path will be included in the hierarchy; </param>
    private static void DrawPrefabFolderDictionary(string path) {

        using (new EditorGUILayout.HorizontalScope()) {
            bool hasFiles = FolderMap[path].files.Count > 0;
            bool hasSubfolders = FolderMap[path].subfolders.Count > 0;
            GUIContent folderContent;
            if (hasFiles) {
                folderContent = new GUIContent("");
            } else folderContent = new GUIContent(path.IsolatePathEnd("\\/"),
                                                  EditorUtils.FetchIcon(FolderMap[path].foldout ? "d_FolderOpened Icon" : "d_Folder Icon"));
            if (hasSubfolders) {
                Rect rect = GUILayoutUtility.GetRect(0, 18, GUILayout.Width(13));
                FolderMap[path].foldout = EditorGUI.Foldout(rect, FolderMap[path].foldout, folderContent,
                                                                   new GUIStyle(EditorStyles.foldout) { stretchWidth = false });
            } if (hasFiles) DrawPrefabFolderButton(path, hasSubfolders && FolderMap[path].foldout);
        } EditorGUI.indentLevel++;

        if (FolderMap[path].foldout) {
            foreach (string subfolder in FolderMap[path].subfolders) {
                DrawPrefabFolderDictionary(subfolder);
                EditorGUI.indentLevel--;
            } 
        } 
    }

    /// <summary>
    /// Draws a button corresponding to a relevant folder in the hierarchy;
    /// </summary>
    /// <param name="path"> Path to the folder; </param>
    /// <param name="folderOpened"> Whether the foldout is active, so the Folder icon can reflect it; </param>
    private static void DrawPrefabFolderButton(string path, bool folderOpened) {
        GUIStyle buttonStyle = path == SelectedCategoryPath ? UIStyles.HFButtonSelected : UIStyles.HFButton;
        GUIContent folderContent = new GUIContent(path.IsolatePathEnd("\\/"), EditorUtils.FetchIcon(folderOpened ? "d_FolderOpened Icon" : "d_Folder Icon"));
        float width = EditorUtils.MeasureTextWidth(folderContent.text, GUI.skin.font);
        if (GUILayout.Button(folderContent, buttonStyle, GUILayout.Width(width + 34), GUILayout.Height(20))) SetSelectedPrefabFolder(path);
    }
}