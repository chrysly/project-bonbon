using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

/// <summary>
/// Because the structure I got for the MADGUI has been quite comfortable to work with, I'm building 
/// this tool using a similar structure. This tool has different needs and less complexity than the MAD,
/// thus I'm creating a new tool altogether rather than inheriting from the former's abstract base;
/// </summary>
namespace BonbonAssetManager {

    public class BAMGUI : EditorWindow {

        [MenuItem("Testing/Bonbon Asset Manager")]
        public static void ShowWindow() {
            var window = GetWindow<BAMGUI>();
        }

        private enum ToolType {
            BonbonManager = 0,
            ActorManager = 1,
            SceneManager = 2,
        } private ToolType activeTool = 0;

        private BonBaseTool[] tools;

        void OnEnable() {
            tools = new BonBaseTool[] {
                BonBaseTool.CreateTool<BonbonManager>(this),
                //BonBaseTool.CreateTool<ActorManager>(this),
            };
        }

        void OnGUI() => tools[(int) activeTool].ShowGUI();

        public void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                foreach (ToolType toolType in System.Enum.GetValues(typeof(ToolType))) {
                    var name = System.Enum.GetName(typeof(ToolType), toolType).CamelSpace();
                    if (GUILayout.Button(name, activeTool == toolType
                                               ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                               GUILayout.ExpandWidth(true))) SwitchTool(toolType);
                }
            }
        }

        private void SwitchTool(ToolType toolType) {
            activeTool = toolType;
        }
    }

    public abstract class BaseHierarchy<T> {

        protected List<string> objectPaths;
        protected List<string> filteredPaths;
        private string searchString = "";

        public System.Action<string> OnPathSelection;

        public static E CreateHierarchy<E>(BonBaseTool tool) where E : BaseHierarchy<T> {
            var hierarchy = System.Activator.CreateInstance<E>();
            hierarchy.AssignTool(tool);
            return hierarchy;
        }

        protected abstract void AssignTool(BonBaseTool tool);

        public BaseHierarchy() {
            var typeName = typeof(T).FullName;
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
                        SearchUpdate();                    } 
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

    public class BonbonHierarchy : BaseHierarchy<BonbonObject> {

        private BonbonManager bonbonManager;

        protected override void AssignTool(BonBaseTool tool) {
            if (tool is BonbonManager) {
                bonbonManager = tool as BonbonManager;
            } else Debug.LogError("Invalid tool assigned to hierarchy tab");
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
}