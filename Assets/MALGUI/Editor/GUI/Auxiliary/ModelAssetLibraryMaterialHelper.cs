using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using CJUtils;

/// <summary> Shows an Editor Window with a GUI that compares the Material Reader with the Material Editor override; </summary>
public class ModelAssetLibraryMaterialHelper : EditorWindow {

    public static ModelAssetLibraryMaterialHelper ShowWindow(ModelImporter model) {
        window = GetWindow<ModelAssetLibraryMaterialHelper>("Material Save Guide", new System.Type[] { typeof(ModelAssetLibraryGUI) });
        ModelAssetLibraryMaterialHelper.model = model;
        return window;
    }

    private static Dictionary<string, Material> StaticDict { get { return ModelAssetLibraryModelReader.StaticMaterialSlots; } }

    private static Dictionary<string, Material> PersistentDict { get { return ModelAssetLibraryModelReader.OriginalMaterialSlots; } }

    private static ModelAssetLibraryMaterialHelper window;

    private static ModelImporter model;

    private static Vector2 editorScrollPosition;

    void OnGUI() {
        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUILayout.VerticalScope()) {
                GUIContent buttonContent = new GUIContent("Close Window", EditorUtils.FetchIcon("d_winbtn_win_close"));
                if (GUILayout.Button(buttonContent, UIStyles.TextureButton)) {
                    CloseWindow();
                } EditorGUILayout.Separator();

                using (new EditorGUILayout.VerticalScope()) {

                    EditorGUILayout.HelpBox("Some Properties in the Importer will override the Material Slots in the Model Renderers. This tool " +
                                            "overwrites these properties on the go, but keeps a reference to the original ones at all times to ensure these " +
                                            "actions are reversible. Below you'll find a list of these changes, as well as some tips to keep in mind as " +
                                            "you work!", MessageType.Info);
                    EditorUtils.DrawSeparatorLines("Some Helpful Tips", true);
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                        GUILayout.Label("• This tool overwrites the Embedded Material Map in the Material Editor tab of this model's importer.", EditorStyles.boldLabel);
                        GUILayout.Label("   Updating these properties while the tool is running won't break your model, but it's not recommended!", EditorStyles.boldLabel);
                        GUILayout.Label("• To verify that the changes are being applied correctly, you can use the buttons below to find the" +
                                        " Model Importer file you are editing.", EditorStyles.boldLabel);
                        GUILayout.Label("   Make sure the Preview in the Inspector matches the preview shown in the tool!", EditorStyles.boldLabel);
                        GUILayout.Label("• You can double-click the Object Fields below to highlight the materials in the Asset Explorer;", EditorStyles.boldLabel);
                    } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                        if (GUILayout.Button("Highlight Model in the Project Window")) {
                            EditorUtils.OpenProjectWindow();
                            EditorGUIUtility.PingObject(model);
                        }
                    } EditorUtils.DrawSeparatorLines("Remapped Material Summary", true);
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                        using (var view = new EditorGUILayout.ScrollViewScope(editorScrollPosition)) {
                            editorScrollPosition = view.scrollPosition;
                            using (new EditorGUILayout.VerticalScope()) {
                                if (model != null) {
                                    if (DrawPropertyArray()) {
                                        GUILayout.FlexibleSpace();
                                        GUIContent successContent = new GUIContent("All the materials are assigned in the right order. You can close this window now;",
                                                                                   EditorUtils.FetchIcon("Progress"));
                                        GUI.color = new Vector4(0.825f, 0.99f, 0.99f, 1);
                                        GUILayout.Label(successContent, EditorStyles.helpBox);
                                        GUI.color = Color.white;
                                    }
                                } else EditorUtils.DrawScopeCenteredText("Whoops! An assembly reload happened. These references are gone!");
                            }
                        }
                    } 
                } /// Same thing...
            } /// For this one...
        } /// Please send help...
    } /// And coconut water...

    private bool DrawPropertyArray() {

        if (StaticDict != null && PersistentDict != null) {

            bool allValuesAssigned = true;
            using (new EditorGUILayout.VerticalScope()) {
                int index = 0;
                foreach (KeyValuePair<string, Material> kvp in PersistentDict) {
                    index++;
                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                        GUIStyle rectStyle = new GUIStyle(EditorStyles.selectionRect);
                        rectStyle.margin = new RectOffset(0, 0, 6, 1);
                        using (new EditorGUILayout.HorizontalScope(rectStyle, GUILayout.MaxWidth(30), GUILayout.MaxHeight(15))) {
                            GUIStyle textStyle = new GUIStyle(UIStyles.CenteredLabel);
                            GUILayout.Label(index.ToString(), textStyle);
                        } EditorGUILayout.Space(2, false);
                        GUIStyle fieldStyle = new GUIStyle();
                        fieldStyle.padding = new RectOffset(0, 0, 6, 1);
                        using (new EditorGUILayout.HorizontalScope(fieldStyle)) {
                            EditorGUILayout.ObjectField(kvp.Value, typeof(Material), false);
                        } GUIStyle tempStyle = new GUIStyle(EditorStyles.helpBox);
                        bool valueChanged = kvp.Value != StaticDict[kvp.Key];
                        if (valueChanged) {
                            GUILayout.Label(" ", GUILayout.MaxWidth(2));
                            GUIStyle arrowStyle = new GUIStyle();
                            arrowStyle.padding = new RectOffset(0, 0, 7, 1);
                            GUILayout.Label(new GUIContent(EditorUtils.FetchIcon("tab_next")), arrowStyle, GUILayout.MaxWidth(22));
                            using (new EditorGUILayout.HorizontalScope(fieldStyle)) {
                                EditorGUILayout.ObjectField(StaticDict[kvp.Key], typeof(Material), false);
                            }
                        } using (new EditorGUILayout.HorizontalScope(tempStyle, GUILayout.MaxWidth(25))) {
                            if (valueChanged) {
                                EditorUtils.DrawTexture(EditorUtils.FetchIcon("d_P4_DeletedLocal"), 20, 20);
                                allValuesAssigned = false;
                            } else {
                                EditorUtils.DrawTexture(EditorUtils.FetchIcon("d_P4_CheckOutRemote"), 20, 20);
                            }
                        }
                    }
                }
            } return allValuesAssigned;
        } return false;
    }

    public void CloseWindow() {
        Close();
    }
}