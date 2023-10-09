using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

namespace BonbonAssetManager {

    /// <summary>
    /// Main Window of the BAMGUI;
    /// </summary>
    public class BAMGUI : EditorWindow {

        [MenuItem("Testing/Bonbon Asset Manager")]
        public static void ShowWindow() {
            var window = GetWindow<BAMGUI>();
        }

        /// <summary>
        /// An enum for every subtool of the BAMGUI;
        /// </summary>
        public enum ToolType {
            BonbonManager = 0,
            SkillManager = 1,
            EffectManager = 2,
            ActorManager = 3,
            Configuration = 4,
        } /// <summary> Type of the tool currently selected; </summary>
        private ToolType activeTool = 0;

        /// <summary> An array containing all the tools included in the BAMGUI; </summary>
        public BonBaseTool[] tools { get; private set; }

        public string[] assetPaths;

        /// <summary> A list of all the bonbon assets in the project; </summary>
        public List<BonbonBlueprint> GlobalBonbonList { get; private set; }
        /// <summary> A list of all the skill assets in the project; </summary>
        public List<SkillObject> GlobalSkillList { get; private set; }

        public CJToolAssets assetRefs { get; private set; }

        void OnEnable() {
            IntializeLists();
            tools = new BonBaseTool[] {
                BonBaseTool.CreateTool<BonbonManager>(this),
                BonBaseTool.CreateTool<SkillManager>(this),
                BonBaseTool.CreateTool<EffectManager>(this),
                BonBaseTool.CreateTool<ActorManager>(this),
                BonBaseTool.CreateTool<ConfigManager>(this),
            }; assetPaths = (tools[(int) ToolType.Configuration] as ConfigManager).Init();
            foreach (BonBaseTool tool in tools) tool.Initialize();
            InitializeAssetRefs();
        }

        void OnDisable() {
            for (int i = 0; i < tools.Length; i++) DestroyImmediate(tools[i]);
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Initialize the global lists in the Main GUI;
        /// </summary>
        private void IntializeLists() {
            GlobalBonbonList = BAMUtils.InitializeList<BonbonBlueprint>();
            GlobalSkillList = BAMUtils.InitializeList<SkillObject>();
        }

        private void InitializeAssetRefs() => assetRefs = FieldUtils.GetToolAssets();

        void OnGUI() => tools[(int) activeTool].ShowGUI();

        public void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                foreach (ToolType toolType in System.Enum.GetValues(typeof(ToolType))) {
                    var name = System.Enum.GetName(typeof(ToolType), toolType).CamelSpace();
                    if (GUILayout.Button(name, activeTool == toolType
                                               ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                               GUILayout.MinWidth(150), GUILayout.ExpandWidth(true))) SwitchTool(toolType);
                }
            }
        }

        private void SwitchTool(ToolType toolType) {
            BAMUtils.ResetHotControl();
            activeTool = toolType;
        }
    }

    public class BaseHierarchy<O> {

        private BonBaseTool tool;

        protected List<string> objectPaths;
        protected List<string> filteredPaths;
        private string searchString = "";

        public System.Action<string> OnPathSelection;

        public BaseHierarchy(BonBaseTool tool) {
            this.tool = tool;
            ReloadHierarchy();
        }

        protected void AssignTool(BonBaseTool tool) {
            this.tool = tool;
        }

        public void ReloadHierarchy() {
            var typeName = typeof(O).FullName;
            var bonbonGUIDs = AssetDatabase.FindAssets($"t:{typeName}");
            objectPaths = new List<string>();
            for (int i = 0; i < bonbonGUIDs.Length; i++) objectPaths.Add(AssetDatabase.GUIDToAssetPath(bonbonGUIDs[i]));
            filteredPaths = new List<string>(objectPaths);
        }

        public virtual void ShowGUI() {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(205))) {
                using (new EditorGUILayout.HorizontalScope(UIStyles.PaddedToolbar)) {
                    var potentialSearch = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField);
                    if (potentialSearch != searchString) {
                        searchString = potentialSearch;
                        SearchUpdate(); }
                }
                using (new EditorGUILayout.ScrollViewScope(Vector2.zero, UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                    foreach (string path in filteredPaths) {
                        DrawButton(path);
                    }
                }
            }
        }

        private void SearchUpdate() {
            filteredPaths = ModelAssetDatabase.MADUtils.SearchingUtils.GetSearchQuery(searchString, objectPaths);
        }

        public void DrawButton(string path) {
            using (new EditorGUILayout.HorizontalScope()) {
                GUI.color = UIColors.Red;
                GUIContent deleteButton = new GUIContent(EditorUtils.FetchIcon("TreeEditor.Trash"));
                if (GUILayout.Button(deleteButton, tool.SelectedPath == path
                                                   ? new GUIStyle(UIStyles.HButton) { margin = UIStyles.HButtonSelected.margin } 
                                                   : UIStyles.HButton, GUILayout.Width(30), GUILayout.Height(20))) {
                    if (ModalAssetDeletion.ConfirmAssetDeletion(path.IsolatePathEnd("/"))) {
                        BAMUtils.DeleteAsset(path.RemovePathEnd("/"), path.IsolatePathEnd("/").RemovePathEnd("."));
                    } ReloadHierarchy();
                    GUIUtility.ExitGUI();
                } GUI.color = Color.white;
                string pathName = path.IsolatePathEnd("/").RemovePathEnd(".");
                float width = EditorUtils.MeasureTextWidth(pathName, GUI.skin.font);
                if (GUILayout.Button(pathName, tool.SelectedPath == path
                                       ? UIStyles.HButtonSelected : UIStyles.HButton,
                                       GUILayout.Width(width + 15), GUILayout.Height(20))) {
                    OnPathSelection?.Invoke(path);
                }
            }
        }
    }

    public class AssetCreator<T> where T : ScriptableObject {

        public event System.Action OnAssetCreation;

        private string folderPath;
        private string newAssetName = "";
        private GeneralUtils.InvalidNameCondition NameCondition;

        public AssetCreator(string folderPath) {
            this.folderPath = folderPath;
        }

        private ScriptableObject CreateAsset(string folderPath, ref string assetName) {
            ScriptableObject so = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(so, BAMUtils.ToFilePath(folderPath, assetName));
            OnAssetCreation?.Invoke();
            assetName = "";
            BAMUtils.ResetHotControl();
            return so;
        }

        public void ShowCreator() {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.Width(205))) {
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    using (new EditorGUILayout.VerticalScope(GUILayout.Height(22))) {
                        GUILayout.FlexibleSpace();
                        string impendingName = EditorGUILayout.TextField(newAssetName);
                        if (impendingName != newAssetName) {
                            newAssetName = impendingName;
                            NameCondition = 0;
                        } GUILayout.FlexibleSpace();
                    }
                    if (GUILayout.Button(EditorUtils.FetchIcon("P4_AddedRemote@2x"), GUILayout.Width(30), GUILayout.Height(22))) {
                        NameCondition = GeneralUtils.ValidateFilename(BAMUtils.ToFilePath(folderPath, newAssetName), newAssetName);
                        if (NameCondition == 0) {
                            NameCondition = GeneralUtils.InvalidNameCondition.Success;
                            CreateAsset(folderPath, ref newAssetName);
                        }
                    }
                } using (new EditorGUILayout.VerticalScope(GUILayout.Height(45))) {
                    GUILayout.FlexibleSpace();
                    DrawNameConditionBox();
                    GUILayout.FlexibleSpace();
                }
            }
        }

        /// <summary>
        /// Draw a box with useful information about the chosen file name and prefab creation;
        /// </summary>
        private void DrawNameConditionBox() {
            switch (NameCondition) {
                case GeneralUtils.InvalidNameCondition.None:
                    EditorGUILayout.HelpBox("Messages concerning the availability of the name written above will be displayed here;", MessageType.Info);
                    break;
                case GeneralUtils.InvalidNameCondition.Empty:
                    EditorGUILayout.HelpBox("The name of the file cannot be empty;", MessageType.Error);
                    break;
                case GeneralUtils.InvalidNameCondition.Overwrite:
                    EditorGUILayout.HelpBox("A file with that name already exists in the target directory. Do you wish to overwrite it?", MessageType.Warning);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button("Overwrite")) {
                            BAMUtils.DeleteAsset(folderPath, newAssetName);
                            CreateAsset(folderPath, ref newAssetName);
                        } if (GUILayout.Button("Cancel")) {
                            NameCondition = 0;
                        }
                    } break;
                case GeneralUtils.InvalidNameCondition.Symbol:
                    EditorGUILayout.HelpBox("The filename can only contain alphanumerical values and/or whitespace characters;", MessageType.Error);
                    break;
                case GeneralUtils.InvalidNameCondition.Convention:
                    EditorGUILayout.HelpBox("This name violates the project's naming convention;", MessageType.Error);
                    break;
                case GeneralUtils.InvalidNameCondition.Success:
                    GUIContent messageContent = new GUIContent(" Asset created successfully!", EditorUtils.FetchIcon("d_PreMatCube@2x"));
                    EditorGUILayout.HelpBox(messageContent);
                    break;
            }
        }
    }
}