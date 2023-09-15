using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

/// <summary>
/// Because the structure I devised for the MADGUI has been quite comfortable to work with, I'm building 
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
            ActorManager = 0,
            BonbonManager = 1,
            SceneManager = 2,
        } private ToolType activeTool = 0;

        private BonBaseTool[] tools;

        void OnEnable() {
            tools = new BonBaseTool[] {
                BonBaseTool.CreateTool<ActorManager>(this),
                BonBaseTool.CreateTool<BonbonManager>(this),
            };
        }

        void OnGUI() {
            tools[(int) activeTool].ShowGUI();
        }
    }

    public class BonbonManager : BonBaseTool {

        private BonbonHierarchy hierarchy;

        private enum TabType {
            Storage = 0,
            Kitchen = 1,
            Factory = 2,
            Actors = 3,
        } private TabType activeTab;

        void OnEnable() {
            hierarchy = new BonbonHierarchy();
            tabs = new BonBaseTab[] {
                BonBaseTab.CreateTab<BonbonStorageTab>(this),
                BonBaseTab.CreateTab<BonbonKitchenTab>(this),
                BonBaseTab.CreateTab<BonbonFactoryTab>(this),
                BonBaseTab.CreateTab<BonbonActorsTab>(this),
            };
        }

        public override void ShowGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                hierarchy.ShowGUI();
                DrawToolbar();
            } tabs[(int) activeTab].ShowGUI();
        }

        public void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                foreach (TabType tabMode in System.Enum.GetValues(typeof(TabType))) {
                    if (GUILayout.Button(System.Enum.GetName(typeof(TabType), tabMode), activeTab == tabMode
                                                                                      ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                                                      GUILayout.ExpandWidth(true))) SwitchTab(tabMode);
                }
            }
        }

        private void SwitchTab(TabType tabMode) {
            activeTab = tabMode;
        }
    }

    public class BonbonHierarchy {

        private string[] bonbonObjects;

        public BonbonHierarchy() {
            var bonbonGUIDs = AssetDatabase.FindAssets($"t:{nameof(BonbonObject)}");
            bonbonObjects = new string[bonbonGUIDs.Length];
            for (int i = 0; i < bonbonGUIDs.Length; i++) bonbonObjects[i] = AssetDatabase.GUIDToAssetPath(bonbonGUIDs[i]);
        }

        public void ShowGUI() {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(200))) {
                using (new EditorGUILayout.HorizontalScope(UIStyles.PaddedToolbar)) {
                    GUILayout.TextField("", EditorStyles.toolbarSearchField);
                }
                using (new EditorGUILayout.ScrollViewScope(Vector2.zero, UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                    foreach (string path in bonbonObjects) {
                        GUILayout.Button(path.IsolatePathEnd("\\//").RemovePathEnd("."));
                    }
                }
            }
        }
    }

    public abstract class BonbonTab : BonBaseTab {

        protected BonbonManager BonbonManager;

        protected override void InitializeTab() {
            if (Tool is BonbonManager) {
                BonbonManager = Tool as BonbonManager;
            } else Debug.LogError(INVALID_MANAGER);
        }
    }

    public class BonbonStorageTab : BonbonTab {

        public override void ShowGUI() {

        }
    }

    public class BonbonKitchenTab : BonbonTab {

        public override void ShowGUI() {
        
        }
    }

    public class BonbonFactoryTab : BonbonTab {

        public override void ShowGUI() {

        }
    }

    public class BonbonActorsTab : BonbonTab {

        public override void ShowGUI() {

        }
    }
}