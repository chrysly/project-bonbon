using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HierarchyBuilder = ModelAssetLibraryHierarchyBuilder;

/// <summary> Component class of the Model Asset Library;
/// <br></br> Organizes Prefab & Model Data in the GUI for DnD and categorization functions; </summary>
public static class ModelAssetLibraryPrefabOrganizer {

    /// <summary> Data relevant to a folder category, namely, name and file contents; </summary>
    public class CategoryData {
        /// <summary> Parsed name of this category; </summary>
        public string name;
        /// <summary> ID list of all prefabs filed under this category;
        /// <br></br> Note that prefab categories is based on model category; </summary>
        public List<string> prefabIDs;
        /// <summary> ID list of all models filed under this category; </summary>
        public List<string> modelIDs;

        public CategoryData(string name) {
            this.name = name;
            prefabIDs = new List<string>();
            modelIDs = new List<string>();
        }
    } /// <summary> The folder path of the category selected in the GUI; </summary>
    public static string SelectedCategory { get; private set; }

    /// <summary> Maps a folder path to its category data; </summary>
    public static Dictionary<string, CategoryData> CategoryMap { get; private set; }

    /// <summary> Data used to draw prefab cards; </summary>
    public class PrefabCardData {
        /// <summary> Root gameObject atop the prefab file hierarchy; </summary>
        public GameObject rootObject;
        /// <summary> Asset preview of the prefab; </summary>
        public Texture2D preview;

        public PrefabCardData(GameObject rootObject, Texture2D preview) {
            this.rootObject = rootObject;
            this.preview = preview;
        }
    } /// <summary> Dictionary mapping path keys to the corresponding Prefab Card Data; </summary>
    public static Dictionary<string, PrefabCardData> PrefabCardMap { get; private set; }

    /// <summary> Selection group that may be passed to the DragAndDrop class; </summary>
    public static List<Object> DragSelectionGroup { get; private set; }
    /// <summary> Whether the mouse hovered over a button in the current frame; </summary>
    public static bool MouseOverButton;

    /// <summary> Sorting Modes for the Prefab Organizer; </summary>
    public enum PrefabSortMode {
        /// <summary> The prefabs cards will be drawn in alphabetical order; </summary>
        Name,
        /// <summary> The prefab cards will be grouped by parent model; </summary>
        Model
    } /// <summary> Sorting Mode selected on the Prefab Organizer GUI; </summary>
    public static PrefabSortMode SortMode { get; private set; }

    /// <summary> Search String obtained from the Prefab Organizer Search Bar; </summary>
    public static string SearchString { get; private set; }

    /// <summary> List mapping the name of each prefab to their IDs; </summary>
    private static List<KeyValuePair<string, string>> prefabNameMapList;
    /// <summary> List holding IDs filtered using the current Search String, if any; </summary>
    public static List<string> SearchResultList { get; private set; }

    /// <summary>
    /// Creates a map of folders path and the data they contain that's relevant to the tool;
    /// </summary>
    public static void BuildCategoryMap() {
        Dictionary<string, HierarchyBuilder.FolderData> folderMap;
        if (HierarchyBuilder.FolderMap == null) folderMap = HierarchyBuilder.FolderMap;
        else folderMap = HierarchyBuilder.BuildFolderMap(ModelAssetLibrary.RootAssetPath);
        CategoryMap = new Dictionary<string, CategoryData>();
        foreach (KeyValuePair<string, HierarchyBuilder.FolderData> kvp in folderMap) {
            CategoryMap[kvp.Key] = new CategoryData(kvp.Value.name);
            foreach (string modelPath in kvp.Value.files) {
                CategoryMap[kvp.Key].modelIDs.Add(AssetDatabase.AssetPathToGUID(modelPath));
            }
            foreach (string modelID in CategoryMap[kvp.Key].modelIDs) {
                CategoryMap[kvp.Key].prefabIDs.AddRange(ModelAssetLibrary.ModelDataDict[modelID].prefabIDList);
            }
        }
    }

    /// <summary>
    /// Sets the selected folder path;
    /// </summary>
    /// <param name="path"> Folder path to select; </param>
    public static void SetSelectedCategory(string path) {
        DragSelectionGroup = new List<Object>();
        LoadCategoryData(path);
        SelectedCategory = path;
    }

    /// <summary>
    /// Unloads all static data contained in the tool;
    /// </summary>
    public static void FlushCategoryData() {
        if (PrefabCardMap != null) {
            foreach (PrefabCardData data in PrefabCardMap.Values) {
                if (data != null && data.preview != null) {
                    Object.DestroyImmediate(data.preview);
                }
            } PrefabCardMap = null;
        } CategoryMap = null;
        SelectedCategory = null;
        DragSelectionGroup = null;
        SortMode = 0;

        SearchString = null;
        prefabNameMapList = null;
        SearchResultList = null;
    }

    /// <summary>
    /// Loads all relevant static data;
    /// </summary>
    /// <param name="path"> Category path to load; </param>
    public static void LoadCategoryData(string path) {
        if (CategoryMap == null) BuildCategoryMap();
        if (PrefabCardMap == null) PrefabCardMap = new Dictionary<string, PrefabCardData>();
        prefabNameMapList = new List<KeyValuePair<string, string>>();
        foreach (string prefabID in CategoryMap[path].prefabIDs) {
            if (!PrefabCardMap.ContainsKey(prefabID)) {
                string assetPath = ModelAssetLibrary.PrefabDataDict[prefabID].path;
                GameObject rootObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                Texture2D preview = AssetPreview.GetAssetPreview(rootObject);
                while (preview == null) {
                    preview = AssetPreview.GetAssetPreview(rootObject);
                } preview.hideFlags = HideFlags.HideAndDontSave;
                PrefabCardMap[prefabID] = new PrefabCardData(rootObject, preview);
            } string name = ModelAssetLibrary.PrefabDataDict[prefabID].name;
            prefabNameMapList.Add(new KeyValuePair<string, string>(name, prefabID));
        } DragSelectionGroup = new List<Object>();
    }

    /// <summary>
    /// Sets the current prefab sort mode;
    /// <br></br> Honestly, why did I even write this method;
    /// </summary>
    /// <param name="sortMode"> New sort mode; </param>
    public static void SetSortMode(PrefabSortMode sortMode) {
        /// Originally I did a thing or two here, but turns out I don't want to anymore;
        /// So I'll leave this method here anyways <3
        SortMode = sortMode;
    }

    /// <summary>
    /// Updates the current Search String to a new value;
    /// <br></br> Discards the previous results, if any;
    /// </summary>
    /// <param name="searchString"> New search string; </param>
    public static void SetSearchString(string searchString) {
        SearchResultList = null;
        SearchString = searchString;
    }

    /// <summary>
    /// Process the Search Results upon request;
    /// </summary>
    public static void ProcessSearchList() {
        List<KeyValuePair<string, string>> processList = prefabNameMapList.FindAll((kvp) => kvp.Key.Contains(SearchString));
        SearchResultList = new List<string>();
        foreach (KeyValuePair<string, string> kvp in processList) SearchResultList.Add(kvp.Value);
    }
}