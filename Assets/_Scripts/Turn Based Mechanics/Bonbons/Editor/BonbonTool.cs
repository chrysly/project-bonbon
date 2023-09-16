using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

public class BonbonTool : EditorWindow {

    [MenuItem("Testing/BonbonTool")]
    public static void ShowWindow() {
        var window = GetWindow<BonbonTool>();
    }

    private BonbonHierarchy hierarchy;
    private BonbonBaseTab[] tabs;

    private enum TabMode {
        Storage = 0,
        Kitchen = 1,
        Factory = 2,
        Actors = 3,
    } private TabMode activeMode;

    void OnEnable() {
        hierarchy = new BonbonHierarchy();
        tabs = new BonbonBaseTab[] {
            BonbonBaseTab.CreateTab<BonbonStorageTab>(this),
            BonbonBaseTab.CreateTab<BonbonKitchenTab>(this),
            BonbonBaseTab.CreateTab<BonbonFactoryTab>(this),
            BonbonBaseTab.CreateTab<BonbonActorsTab>(this),
        };
    }

    private void OnGUI() {
        using (new EditorGUILayout.HorizontalScope()) {
            hierarchy.ShowGUI();
            DrawToolbar();
        } tabs[(int) activeMode].ShowGUI();
    }

    private void DrawToolbar() {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
            foreach (TabMode tabMode in System.Enum.GetValues(typeof(TabMode))) {
                if (GUILayout.Button(System.Enum.GetName(typeof(TabMode), tabMode), activeMode == tabMode
                                                                                  ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                                                  GUILayout.ExpandWidth(true))) SwitchTab(tabMode);
            }
        }
    }

    private void SwitchTab(TabMode tabMode) {
        activeMode = tabMode;
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

public abstract class BonbonBaseTab : ScriptableObject {

    protected BonbonTool Tool;

    public static T CreateTab<T>(BonbonTool tool) where T : BonbonBaseTab {
        var tab = CreateInstance<T>();
        tab.Tool = tool;
        return tab;
    }

    public abstract void ShowGUI();
}

public class BonbonStorageTab : BonbonBaseTab {

    public override void ShowGUI() {

    }
}

public class BonbonKitchenTab : BonbonBaseTab {

    public override void ShowGUI() {
        
    }
}

public class BonbonFactoryTab : BonbonBaseTab {

    public override void ShowGUI() {

    }
}

public class BonbonActorsTab : BonbonBaseTab {

    public override void ShowGUI() {

    }
}