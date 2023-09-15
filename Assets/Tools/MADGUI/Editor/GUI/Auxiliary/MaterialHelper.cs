using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

namespace ModelAssetDatabase {
    
    /// <summary> 
    /// Shows an Editor Window with a GUI that compares the Material Reader with the Material Editor override; 
    /// </summary>
    public class MaterialHelper : EditorWindow {

        public static MaterialHelper ShowWindow(ReaderTabMaterials reader) {
            var window = GetWindow<MaterialHelper>("Material Save Guide", new System.Type[] { typeof(ModelAssetDatabaseGUI) });
            window.reader = reader;
            return window;
        }

        private ReaderTabMaterials reader;

        private Dictionary<string, Material> staticDict { get { return reader.StaticMaterialSlots; } }

        private Dictionary<string, Material> persistentDict { get { return reader.OriginalMaterialSlots; } }

        private Vector2 editorScrollPosition;

        void OnGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope()) {
                    GUIContent buttonContent = new GUIContent("Close Window", EditorUtils.FetchIcon("d_winbtn_win_close"));
                    if (GUILayout.Button(buttonContent, UIStyles.TextureButton)) {
                        Close();
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
                            GUILayout.Label("• To verify that the changes are being applied correctly, you can use the button below to find the" +
                                            " Model Importer file you are editing.", EditorStyles.boldLabel);
                            GUILayout.Label("   Make sure the Preview in the Inspector matches the preview shown in the tool!", EditorStyles.boldLabel);
                            GUILayout.Label("• You can double-click the Object Fields below to highlight the materials in the Asset Explorer;", EditorStyles.boldLabel);
                        } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                            if (reader is null || reader.Model is null) GUI.enabled = false;
                            if (GUILayout.Button("Highlight Model in the Project Window")) {
                                EditorUtils.OpenProjectWindow();
                                EditorGUIUtility.PingObject(reader.Model);
                            } GUI.enabled = true;
                        } EditorUtils.DrawSeparatorLines("Remapped Material Summary", true);
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                            using (var view = new EditorGUILayout.ScrollViewScope(editorScrollPosition)) {
                                editorScrollPosition = view.scrollPosition;
                                using (new EditorGUILayout.VerticalScope()) {
                                    if (reader is not null && reader.Model is not null) {
                                        if (DrawPropertyArray()) {
                                            GUILayout.FlexibleSpace();
                                            GUIContent successContent = new GUIContent("No changes pending review;",
                                                                                       EditorUtils.FetchIcon("Progress"));
                                            GUI.color = new Vector4(0.825f, 0.99f, 0.99f, 1);
                                            GUILayout.Label(successContent, EditorStyles.helpBox);
                                            GUI.color = Color.white;
                                        } else {
                                            GUILayout.FlexibleSpace();
                                            GUI.color = UIColors.Green;
                                            if (GUILayout.Button("Assign All")) {
                                                reader.AssignMaterialsPersistently();
                                            } GUI.color = UIColors.Red;
                                            if (GUILayout.Button("Discard All")) {
                                                reader.ResetSlotChanges();
                                            } GUI.color = Color.white;
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

            if (staticDict != null && persistentDict != null) {

                bool allValuesAssigned = true;
                using (new EditorGUILayout.VerticalScope()) {
                    int index = 0;
                    foreach (KeyValuePair<string, Material> kvp in persistentDict) {
                        index++;
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                            GUIStyle rectStyle = new GUIStyle(EditorStyles.selectionRect);
                            rectStyle.margin = new RectOffset(0, 0, 6, 1);
                            using (new EditorGUILayout.HorizontalScope(rectStyle, GUILayout.Width(60), GUILayout.MaxHeight(15))) {
                                GUIStyle textStyle = new GUIStyle(UIStyles.CenteredLabel);
                                GUILayout.Label(kvp.Key, textStyle);
                            } EditorGUILayout.Space(2, false);
                            GUIStyle fieldStyle = new GUIStyle();
                            fieldStyle.padding = new RectOffset(0, 0, 6, 1);
                            using (new EditorGUILayout.HorizontalScope(fieldStyle)) {
                                EditorGUILayout.ObjectField(kvp.Value, typeof(Material), false);
                            } GUIStyle tempStyle = new GUIStyle(EditorStyles.helpBox);
                            bool valueChanged = kvp.Value != staticDict[kvp.Key];
                            if (valueChanged) {
                                GUILayout.Label(" ", GUILayout.MaxWidth(2));
                                GUIStyle arrowStyle = new GUIStyle();
                                arrowStyle.padding = new RectOffset(0, 0, 7, 1);
                                GUILayout.Label(new GUIContent(EditorUtils.FetchIcon("tab_next")), arrowStyle, GUILayout.MaxWidth(22));
                                using (new EditorGUILayout.HorizontalScope(fieldStyle)) {
                                    EditorGUILayout.ObjectField(staticDict[kvp.Key], typeof(Material), false);
                                }
                            } float buttonSize = 28;
                            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { padding = new RectOffset() };
                            if (valueChanged) {
                                if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_P4_CheckOutRemote")),
                                                        buttonStyle, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize))) {
                                    persistentDict[kvp.Key] = staticDict[kvp.Key];
                                    reader.UpdateSlotChangedStatus();
                                    break;
                                } if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_P4_DeletedLocal")),
                                                        buttonStyle, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize))) {
                                    reader.ReplacePersistentMaterial(kvp.Key, kvp.Value);
                                } allValuesAssigned = false;
                            } else {
                                GUILayout.Label(EditorUtils.FetchIcon("CacheServerConnected@2x"), EditorStyles.helpBox,
                                                GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
                            }
                        }
                    }
                } return allValuesAssigned;
            } return false;
        }
    }
}