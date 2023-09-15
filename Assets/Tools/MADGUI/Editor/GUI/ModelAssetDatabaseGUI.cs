using UnityEngine;
using UnityEditor;
using CJUtils;

namespace ModelAssetDatabase {
    
    /// <summary> Main GUI of the Model Asset Library;
    /// <br></br> Connects a number of tools together in a single window;
    /// </summary>
    public class ModelAssetDatabaseGUI : EditorWindow {

        /// <summary>
        /// Shows the Main Window of the Model Asset Library;
        /// </summary>
        [MenuItem("Tools/Model Asset Library")]
        public static void ShowWindow() {
            if (HasOpenInstances<ModelAssetDatabaseGUI>()) MainGUI.Close();
            ConfigurationCore.LoadConfig();
            if (string.IsNullOrWhiteSpace(ModelAssetDatabase.RootAssetPath)) {
                ConfigurationGUI.ShowWindow();
                return;
            } MainGUI = GetWindow<ModelAssetDatabaseGUI>("Model Asset Library", typeof(ConfigurationGUI));
            if (HasOpenInstances<ConfigurationGUI>()) {
                ConfigurationGUI.ConfigGUI.Close();
            }
        }

        #region | LibraryGUI-only variables |

        /// <summary> Reference to the active GUI Window; </summary>
        public static ModelAssetDatabaseGUI MainGUI { get; private set; }

        private HierarchyBuilder hierarchyBuilder;
        public BaseTool[] Subtools { get; private set; }

        /// <summary> Available tools to display in the library; </summary>
        public enum ToolMode {
            ModelReader = 0,
            PrefabOrganizer = 1,
            MaterialManager = 2,
        } /// <summary> The tool actively displayed within the library; </summary>
        public ToolMode ActiveTool { get; private set; }

        /// <summary> Whether the window should repaint every frame; </summary>
        private bool highRepaintFrequency;

        private Vector2 directoryScroll;
        private Vector2 toolScroll;

        #endregion

        #region | Hierarchy-Tool Bridge |

        /// Though in separate classes, the hierarchy builder and the standard tools must occasionally communicate with one another;
        /// Since references to these instances are stored in the Main GUI, communication between the two happens here;

        public Reader ModelReader { get { return Subtools[(int) ToolMode.ModelReader] as Reader; } }
        public PrefabOrganizer PrefabOrganizer { get { return Subtools[(int) ToolMode.PrefabOrganizer] as PrefabOrganizer; } }
        public MaterialManager MaterialManager { get { return Subtools[(int) ToolMode.MaterialManager] as MaterialManager; } }
        public string SelectedAssetPath { get; private set; }

        /// <summary>
        /// Sets the selected asset on the active tool;
        /// <br></br> The tool chooses whether the change is valid;
        /// </summary>
        /// <param name="path"></param>
        public void SetSelectedAsset(string path) {
            SelectedAssetPath = path;
            Subtools[(int) ActiveTool].SetSelectedAsset(path);
        }

        /// <summary>
        /// Switch to the Prefabs Section of the Model Reader on the corresponding model;
        /// </summary>
        /// <param name="modelID"> ID of the model used to redirect the GUI; </param>
        public void SwitchToLibrary(string modelID) {
            MainGUI.SwitchActiveTool(ToolMode.ModelReader);
            SetSelectedAsset(ModelAssetDatabase.ModelDataDict[modelID].path);
            ModelReader.SetSelectedAssetMode(Reader.AssetMode.Model);
            ModelReader.SetSelectedSection(Reader.SectionType.Prefabs);
            GUIUtility.ExitGUI();
        }

        /// <summary>
        /// Switch to the Prefab Organizer tool and place a Search String with prefab's name;
        /// </summary>
        /// <param name="prefabID"> ID of the prefab used to redirect the GUI; </param>
        public void SwitchToOrganizer(string prefabID) {
            MainGUI.SwitchActiveTool(ToolMode.PrefabOrganizer);
            string modelID = ModelAssetDatabase.PrefabDataDict[prefabID].modelID;
            string path = ModelAssetDatabase.ModelDataDict[modelID].path.RemovePathEnd("\\/");
            string name = ModelAssetDatabase.PrefabDataDict[prefabID].name;
            SetSelectedAsset(path);
            PrefabOrganizer.SetSearchString(name);
            GUIUtility.ExitGUI();
        }

        #endregion

        void OnEnable() {
            ConfigurationCore.LoadConfig();
            ModelAssetDatabase.Refresh();
            hierarchyBuilder = BaseTool.CreateTool<HierarchyBuilder>(this);
            Subtools = new BaseTool[] { 
                BaseTool.CreateTool<Reader>(this),
                BaseTool.CreateTool<PrefabOrganizer>(this),
                BaseTool.CreateTool<MaterialManager>(this),
            };
        }

        void OnDisable() => FlushGlobalData();

        void OnFocus() {
            if (HasOpenInstances<ModelAssetDatabaseGUI>()
                && MainGUI == null) MainGUI = GetWindow<ModelAssetDatabaseGUI>();
        }

        void Update() {
            if (highRepaintFrequency) Repaint();
        }

        void OnGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(200))) {
                    hierarchyBuilder.DrawToolbar();
                    using (var leftScope = new EditorGUILayout.ScrollViewScope(directoryScroll,
                                                                         false, true, GUI.skin.horizontalScrollbar,
                                                                         GUI.skin.verticalScrollbar, UIStyles.PaddedScrollView)) {
                        directoryScroll = leftScope.scrollPosition;
                        hierarchyBuilder.ShowGUI();
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
                if (ActiveTool == ToolMode.ModelReader) { GUI.color = UIColors.Blue; GUI.enabled = false; }
                if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_PrefabModel Icon")),
                                     EditorStyles.toolbarButton, GUILayout.MaxHeight(20))) {
                    SwitchActiveTool(ToolMode.ModelReader);
                } if (ActiveTool == ToolMode.PrefabOrganizer) { GUI.color = UIColors.Blue; GUI.enabled = false; }
                else { GUI.color = Color.white; GUI.enabled = true; }
                if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_PrefabVariant Icon")),
                                       EditorStyles.toolbarButton, GUILayout.MaxHeight(20))) {
                    SwitchActiveTool(ToolMode.PrefabOrganizer);
                } if (ActiveTool == ToolMode.MaterialManager) { GUI.color = UIColors.Blue; GUI.enabled = false; }
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
        public void SwitchActiveTool(ToolMode newToolMode) {
            Subtools[(int) ActiveTool].ResetData();
            Subtools[(int) newToolMode].RefreshData();
            ActiveTool = newToolMode;
        }

        /// <summary> Draws the toolbar buttons depending on the currently selected tool; </summary>
        private void DrawToolbarButtons() {
            Subtools[(int) ActiveTool].DrawToolbar();
            if (GUILayout.Button(EditorUtils.FetchIcon("_Popup"), EditorStyles.toolbarButton, GUILayout.MinWidth(32), GUILayout.MaxWidth(32))) {
                ConfigurationGUI.ShowWindow();
            }
        }

        /// <summary> Draws the currently selected tool on the right side of the window; </summary>
        private void DrawActiveTool() => Subtools[(int) ActiveTool].ShowGUI();

        /// <summary>
        /// Unloads all data contained in the tool components;
        /// </summary>
        private void FlushGlobalData() {
            //ModelAssetLibrary.UnloadDictionaries();
            Resources.UnloadUnusedAssets();
            SetHighRepaintFrequency(false);
            ActiveTool = 0;
        }

        /// <summary>
        /// Determines whether the Library should repaint on the Editor Update loop;
        /// </summary>
        /// <param name="useHigh"> True to repaint on Update, false otherwise; </param>
        public void SetHighRepaintFrequency(bool useHigh) {
            highRepaintFrequency = useHigh;
        }
    }
}