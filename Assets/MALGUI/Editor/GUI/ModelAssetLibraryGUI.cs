using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelReader = ModelAssetLibraryModelReader;
using ModelReaderGUI = ModelAssetLibraryModelReaderGUI;
using PrefabOrganizer = ModelAssetLibraryPrefabOrganizer;
using PrefabOrganizerGUI = ModelAssetLibraryPrefabOrganizerGUI;
using HierarchyBuilder = ModelAssetLibraryHierarchyBuilder;

/// <summary> Main GUI of the Model Asset Library;
/// <br></br> Connects a number of tools together in a single window;
/// </summary>
public class ModelAssetLibraryGUI : EditorWindow {

    /// <summary>
    /// Shows the Main Window of the Model Asset Library;
    /// </summary>
    [MenuItem("Tools/Model Asset Library")]
    public static void ShowWindow() {
        if (HasOpenInstances<ModelAssetLibraryGUI>()) MainGUI.Close();
        ModelAssetLibraryConfigurationCore.LoadConfig();
        if (string.IsNullOrWhiteSpace(ModelAssetLibrary.RootAssetPath)) {
            ModelAssetLibraryConfigurationGUI.ShowWindow();
            return;
        } MainGUI = GetWindow<ModelAssetLibraryGUI>("Model Asset Library", typeof(ModelAssetLibraryConfigurationGUI));
        if (HasOpenInstances<ModelAssetLibraryConfigurationGUI>()) {
            ModelAssetLibraryConfigurationGUI.ConfigGUI.Close();
        }
    }

    #region | LibraryGUI-only variables |

    /// <summary> Reference to the active GUI Window; </summary>
    public static ModelAssetLibraryGUI MainGUI { get; private set; }

    /// <summary> Available tools to display in the library; </summary>
    public enum ToolMode {
        ModelReader,
        PrefabOrganizer,
        MaterialManager
    } /// <summary> The tool actively displayed within the library; </summary>
    private static ToolMode toolMode;

    private Vector2 directoryScroll;
    private Vector2 toolScroll;

    #endregion

    void OnEnable() {
        ModelAssetLibraryConfigurationCore.LoadConfig();
        ModelAssetLibrary.Refresh();
        ModelReader.FlushAssetData();
        HierarchyBuilder.InitializeHierarchyData();
        PrefabOrganizer.BuildCategoryMap();
    }

    void OnDisable() {
        FlushGlobalToolData();
    }

    void OnFocus() {
        if (HasOpenInstances<ModelAssetLibraryGUI>()
            && MainGUI == null) MainGUI = GetWindow<ModelAssetLibraryGUI>();
    }

    void OnGUI() {
        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUILayout.VerticalScope(GUILayout.MinWidth(200), GUILayout.MaxWidth(220))) {
                HierarchyBuilder.DrawSearchBar();
                using (var leftScope = new EditorGUILayout.ScrollViewScope(directoryScroll,
                                                                     false, true, GUI.skin.horizontalScrollbar,
                                                                     GUI.skin.verticalScrollbar, UIStyles.PaddedScrollView)) {
                    directoryScroll = leftScope.scrollPosition;
                    HierarchyBuilder.DisplayToolDirectory(toolMode);
                } DrawToolSelectionButtons();
            } using (new GUILayout.VerticalScope()) {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                    DrawToolbarButtons();
                } using (var rightScope = new EditorGUILayout.ScrollViewScope(toolScroll, UIStyles.MorePaddingScrollView)) {
                    toolScroll = rightScope.scrollPosition;
                    DrawActiveTool();
                }
            }
        }
    }

    /// <summary>
    /// Draws buttons to switch from one tool to another below the hierarchy;
    /// </summary>
    private void DrawToolSelectionButtons() {
        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
            if (toolMode == ToolMode.ModelReader) { GUI.color = UIColors.Blue; GUI.enabled = false; }
            if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_PrefabModel Icon")),
                                 EditorStyles.toolbarButton, GUILayout.MaxHeight(20))) {
                SwitchActiveTool(ToolMode.ModelReader);
            } if (toolMode == ToolMode.PrefabOrganizer) { GUI.color = UIColors.Blue; GUI.enabled = false; }
            else { GUI.color = Color.white; GUI.enabled = true; }
            if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_PrefabVariant Icon")),
                                   EditorStyles.toolbarButton, GUILayout.MaxHeight(20))) {
                SwitchActiveTool(ToolMode.PrefabOrganizer);
            } if (toolMode == ToolMode.MaterialManager) { GUI.color = UIColors.Blue; GUI.enabled = false; }
            else { GUI.color = Color.white; GUI.enabled = true; }
            if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_Material Icon")),
                                   EditorStyles.toolbarButton, GUILayout.MaxHeight(20))) {
                SwitchActiveTool(ToolMode.MaterialManager);
            } GUI.color = Color.white; GUI.enabled = true;
        } 
    }

    /// <summary>
    /// Switch to a different tool;
    /// </summary>
    public static void SwitchActiveTool(ToolMode newToolMode) {
        FlushActiveToolData();
        toolMode = newToolMode;
    }

    /// <summary>
    /// Draws the toolbar buttons depending on the currently selected tool;
    /// </summary>
    private void DrawToolbarButtons() {
        switch (toolMode) {
            case ToolMode.ModelReader:
                ModelReaderGUI.DrawModelReaderToolbar();
                break;
            case ToolMode.PrefabOrganizer:
                PrefabOrganizerGUI.DrawPrefabOrganizerToolbar();
                break;
            case ToolMode.MaterialManager:
                break;
        } if (GUILayout.Button(EditorUtils.FetchIcon("_Popup"), EditorStyles.toolbarButton, GUILayout.MinWidth(32), GUILayout.MaxWidth(32))) {
            ModelAssetLibraryConfigurationGUI.ShowWindow();
        }
    }

    /// <summary>
    /// Draws the currently selected tool on the right side of the window;
    /// </summary>
    private void DrawActiveTool() {
        switch (toolMode) {
            case ToolMode.ModelReader:
                ModelReaderGUI.ShowSelectedSection();
                break;
            case ToolMode.PrefabOrganizer:
                PrefabOrganizerGUI.ShowSelectedCategory();
                break;
            case ToolMode.MaterialManager:
                break;
        }
    }

    /// <summary>
    /// Unloads the data contained in the active tool;
    /// </summary>
    private static void FlushActiveToolData() {
        switch (toolMode) {
            case ToolMode.ModelReader:
                ModelReader.FlushAssetData();
                break;
            case ToolMode.PrefabOrganizer:
                PrefabOrganizer.FlushCategoryData();
                break;
            case ToolMode.MaterialManager:
                break;
        }
    }

    /// <summary>
    /// Unloads all data contained in the tool components;
    /// </summary>
    private void FlushGlobalToolData() {
        ModelReader.FlushAssetData();
        PrefabOrganizer.FlushCategoryData();
        HierarchyBuilder.FlushHierarchyData();
        //ModelAssetLibrary.UnloadDictionaries();
        toolMode = 0;
    }
}