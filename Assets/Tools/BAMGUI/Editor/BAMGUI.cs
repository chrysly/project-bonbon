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
        /// <summary> A list of all the actor data assets in the project; </summary>
        public List<ActorData> GlobalActorList { get; private set; }

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
            GlobalActorList = BAMUtils.InitializeList<ActorData>();
        }

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
            activeTool = toolType;
        }
    }

    public abstract class BaseHierarchy<O> {

        protected List<string> objectPaths;
        protected List<string> filteredPaths;
        private string searchString = "";

        public System.Action<string> OnPathSelection;

        protected const string INVALID_TOOL = "Invalid tool assigned to hierarchy tab";

        public static T CreateHierarchy<T>(BonBaseTool tool) where T : BaseHierarchy<O> {
            var hierarchy = System.Activator.CreateInstance<T>();
            hierarchy.AssignTool(tool);
            return hierarchy;
        }

        protected abstract void AssignTool(BonBaseTool tool);

        public BaseHierarchy() => ReloadHierarchy();

        public void ReloadHierarchy() {
            var typeName = typeof(O).FullName;
            var bonbonGUIDs = AssetDatabase.FindAssets($"t:{typeName}");
            objectPaths = new List<string>();
            for (int i = 0; i < bonbonGUIDs.Length; i++) objectPaths.Add(AssetDatabase.GUIDToAssetPath(bonbonGUIDs[i]));
            filteredPaths = new List<string>(objectPaths);
        }

        public virtual void ShowGUI() {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(200))) {
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

        public abstract void DrawButton(string path);
    }

    public class AssetCreator<T> where T : ScriptableObject {

        public event System.Action OnAssetCreation;

        private string folderPath;
        private string newAssetName = "";
        private GeneralUtils.InvalidNameCondition NameCondition;

        public AssetCreator(string folderPath) {
            this.folderPath = folderPath;
        }

        private ScriptableObject CreateAsset(string folderPath, string assetName) {
            ScriptableObject so = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(so, ToFilePath(folderPath, assetName));
            OnAssetCreation?.Invoke();
            return so;
        }

        private string ToFilePath(string path, string name) => path + "/" + name + ".asset";

        public void ShowCreator() {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.Width(200))) {
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    string impendingName = EditorGUILayout.TextField(newAssetName);
                    if (impendingName != newAssetName) {
                        newAssetName = impendingName;
                        NameCondition = 0;
                    } if (GUILayout.Button(EditorUtils.FetchIcon("Settings"))) {
                        if (GeneralUtils.ValidateFilename(ToFilePath(folderPath, impendingName), impendingName) == 0) {
                            NameCondition = GeneralUtils.InvalidNameCondition.Success;
                            CreateAsset(folderPath, newAssetName);
                        }
                    }
                } DrawNameConditionBox();
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
                            /// Create Asset here;
                        } if (GUILayout.Button("Cancel")) {
                            NameCondition = 0;
                        }
                    } break;
                case GeneralUtils.InvalidNameCondition.Symbol:
                    EditorGUILayout.HelpBox("The filename can only contain alphanumerical values and/or whitespace characters;", MessageType.Error);
                    break;
                case GeneralUtils.InvalidNameCondition.Convention:
                    GUIStyle simulateMargins = new GUIStyle(EditorStyles.helpBox) { margin = new RectOffset(18, 0, 0, 0) };
                    using (new EditorGUILayout.HorizontalScope(simulateMargins, GUILayout.MaxHeight(30))) {
                        GUIStyle labelStyle = new GUIStyle();
                        labelStyle.normal.textColor = EditorStyles.helpBox.normal.textColor;
                        labelStyle.fontSize = EditorStyles.helpBox.fontSize;
                        GUILayout.Label(new GUIContent(EditorUtils.FetchIcon("console.erroricon.sml@2x")), labelStyle);
                        using (new EditorGUILayout.VerticalScope()) {
                            GUILayout.FlexibleSpace(); GUILayout.FlexibleSpace(); /// Do not judge me. IT LOOKED OFF OK?!
                            GUILayout.Label("This name violates the project's naming convention;", labelStyle);
                            using (new EditorGUILayout.HorizontalScope()) {
                                GUILayout.Label("More information can be found ", labelStyle, GUILayout.ExpandWidth(false));
                                GUIStyle linkStyle = new GUIStyle(labelStyle);
                                linkStyle.normal.textColor = EditorStyles.linkLabel.normal.textColor;
                                if (GUILayout.Button("here", linkStyle, GUILayout.ExpandWidth(false))) {
                                    Application.OpenURL("");
                                } GUILayout.Label(";", labelStyle);
                            } GUILayout.FlexibleSpace(); GUILayout.FlexibleSpace();
                        }
                    } break;
                case GeneralUtils.InvalidNameCondition.Success:
                    GUIContent messageContent = new GUIContent(" Asset created successfully!", EditorUtils.FetchIcon("d_PreMatCube@2x"));
                    EditorGUILayout.HelpBox(messageContent);
                    break;
            }
        }
    }

    public class BonbonHierarchy : BaseHierarchy<BonbonBlueprint> {

        private BonbonManager bonbonManager;

        protected override void AssignTool(BonBaseTool tool) {
            if (tool is BonbonManager) {
                bonbonManager = tool as BonbonManager;
            } else Debug.LogError(INVALID_TOOL);
        }

        public override void DrawButton(string path) {
            string pathName = path.IsolatePathEnd("\\//").RemovePathEnd(".");
            float width = EditorUtils.MeasureTextWidth(pathName, GUI.skin.font);
            if (GUILayout.Button(pathName, bonbonManager.SelectedPath == path
                                   ? UIStyles.HButtonSelected : UIStyles.HButton,
                                   GUILayout.Width(width + 15), GUILayout.Height(20))) {
                OnPathSelection?.Invoke(path);
            }
        }
    }

    public class SkillHierarchy : BaseHierarchy<SkillObject> {

        private SkillManager skillManager;

        protected override void AssignTool(BonBaseTool tool) {
            if (tool is SkillManager) {
                skillManager = tool as SkillManager;
            } else Debug.LogError(INVALID_TOOL);
        }

        public override void DrawButton(string path) {
            string pathName = path.IsolatePathEnd("\\//").RemovePathEnd(".");
            float width = EditorUtils.MeasureTextWidth(pathName, GUI.skin.font);
            if (GUILayout.Button(pathName, skillManager.SelectedPath == path
                                   ? UIStyles.HButtonSelected : UIStyles.HButton,
                                   GUILayout.Width(width + 15), GUILayout.Height(20))) {
                OnPathSelection?.Invoke(path);
            }
        }
    }
    
    public class EffectHierarchy : BaseHierarchy<EffectBlueprint> {

        private EffectManager bonbonManager;

        protected override void AssignTool(BonBaseTool tool) {
            if (tool is EffectManager) {
                bonbonManager = tool as EffectManager;
            } else Debug.LogError(INVALID_TOOL);
        }

        public override void DrawButton(string path) {
            string pathName = path.IsolatePathEnd("\\//").RemovePathEnd(".");
            float width = EditorUtils.MeasureTextWidth(pathName, GUI.skin.font);
            if (GUILayout.Button(pathName, bonbonManager.SelectedPath == path
                                   ? UIStyles.HButtonSelected : UIStyles.HButton,
                                   GUILayout.Width(width + 15), GUILayout.Height(20))) {
                OnPathSelection?.Invoke(path);
            }
        }
    }


    public class ActorHierarchy : BaseHierarchy<ActorData> {

        private ActorManager actorManager;

        protected override void AssignTool(BonBaseTool tool) {
            if (tool is ActorManager) {
                actorManager = tool as ActorManager;
            } else Debug.LogError(INVALID_TOOL);
        }

        public override void DrawButton(string path) {
            string pathName = path.IsolatePathEnd("\\//").RemovePathEnd(".");
            float width = EditorUtils.MeasureTextWidth(pathName, GUI.skin.font);
            if (GUILayout.Button(pathName, actorManager.SelectedPath == path
                                   ? UIStyles.HButtonSelected : UIStyles.HButton,
                                   GUILayout.Width(width + 15), GUILayout.Height(20))) {
                OnPathSelection?.Invoke(path);
            }
        }
    }
}