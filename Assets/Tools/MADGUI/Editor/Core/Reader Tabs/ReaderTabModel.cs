using System.IO;
using UnityEditor;
using UnityEngine;
using CJUtils;
using static ModelAssetDatabase.Reader;

namespace ModelAssetDatabase {
    public class ReaderTabModel : ReaderTab {

        /// <summary> String to display on property undoes; </summary>
        private const string UNDO_PROPERTY = "Model Importer Property Change";
        private ModelImporter Model { get { return Reader.Model; } }
        /// <summary> Directory information on the target file; </summary>
        private FileInfo FileInfo;

        /// <summary> Temporary variable storing potential asset notes; </summary>
        private static bool editNotes;
        /// <summary> Temporary notes stored in the GUI; </summary>
        private static string notes;

        private static Vector2 noteScroll;

        public override void LoadData(string path) {
            FileInfo = new FileInfo(path);
        }

        public override void ResetData() {
            Reader.CleanObjectPreview();
            notes = null;
            editNotes = false;
        }

        /// <summary>
        /// Updates the Model Notes and disables hot control to properly update the Text Area;
        /// </summary>
        /// <param name="notes"> Notes to pass to the ExtData; </param>
        private void UpdateAssetNotes(string notes) {
            using (var so = new SerializedObject(Reader.ModelExtData)) {
                SerializedProperty noteProperty = so.FindProperty("notes");
                noteProperty.stringValue = notes;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            GUIUtility.keyboardControl = 0;
            GUIUtility.hotControl = 0;
        }

        /// <summary> GUI Display for the Model Section </summary>
        public override void ShowGUI() {

            using (new EditorGUILayout.HorizontalScope()) {
                /// Model Preview + Model Details;
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(200), GUILayout.Height(200))) {
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(192), GUILayout.Height(192))) {
                        GUILayout.Label("Model Preview", UIStyles.CenteredLabel);
                        Reader.DrawObjectPreviewEditor(Reader.RootPrefab, GUILayout.ExpandWidth(true), GUILayout.Height(192));

                        using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.FlexibleSpace();
                            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(196), GUILayout.Height(100))) {
                                GUILayout.Label("Model Details", UIStyles.CenteredLabel);
                                GUILayout.FlexibleSpace();
                                EditorUtils.DrawLabelPair("Vertex Count:", Reader.GlobalVertexCount.ToString());
                                EditorUtils.DrawLabelPair("Triangle Count: ", Reader.GlobalTriangleCount.ToString());
                                EditorUtils.DrawLabelPair("Mesh Count: ", Reader.MeshRenderers.Count.ToString());
                                EditorUtils.DrawLabelPair("Rigged: ", Model.avatarSetup == 0 ? "No" : "Yes");
                            } GUILayout.FlexibleSpace();
                        }
                    }
                }
                /// Model Data;
                using (var view = new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(Reader.PANEL_WIDTH))) {

                    EditorUtils.DrawSeparatorLines("External File Info", true);
                    using (new EditorGUILayout.HorizontalScope()) {
                        GUILayout.Label("File Path:", new GUIStyle(GUI.skin.label) { contentOffset = new Vector2(0, 1) }, GUILayout.MaxWidth(55));
                        GUIStyle pathStyle = new GUIStyle(EditorStyles.textField) { margin = new RectOffset(0, 0, 3, 2) };
                        EditorGUILayout.SelectableLabel(Model.assetPath, pathStyle, GUILayout.MaxWidth(260), GUILayout.MaxHeight(19));
                        GUIContent content = new GUIContent("  Open Folder", EditorUtils.FetchIcon("Profiler.Open"));
                        if (GUILayout.Button(content, UIStyles.TextureButton, GUILayout.MinWidth(120), GUILayout.Height(20))) {
                            EditorUtility.RevealInFinder(Model.assetPath);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope()) {
                        GUIStyle extStyle = new GUIStyle(GUI.skin.label) { contentOffset = new Vector2(0, 2) };
                        EditorUtils.DrawLabelPair("File Size:", EditorUtils.ProcessFileSize(FileInfo.Length), extStyle, GUILayout.MaxWidth(115));
                        GUILayout.FlexibleSpace();
                        EditorUtils.DrawLabelPair("Date Imported:",
                                                  FileInfo.CreationTime.ToString().RemovePathEnd(" ").RemovePathEnd(" "), extStyle, GUILayout.MaxWidth(165));
                        GUIContent content = new GUIContent("    Open File   ", EditorUtils.FetchIcon("d_Import"));
                        if (GUILayout.Button(content, UIStyles.TextureButton, GUILayout.MinWidth(120), GUILayout.Height(20))) {
                            AssetDatabase.OpenAsset(Model);
                        }
                    }

                    EditorUtils.DrawSeparatorLines("Internal File Info", true);
                    using (new EditorGUILayout.HorizontalScope()) {
                        EditorGUIUtility.labelWidth = 90;
                        Model.globalScale = EditorGUILayout.FloatField("Model Scale", Model.globalScale, GUILayout.MaxWidth(120));
                        EditorGUIUtility.labelWidth = 108;
                        Model.useFileScale = EditorGUILayout.Toggle("Use Unity Units", Model.useFileScale, GUILayout.MaxWidth(120));
                        EditorGUIUtility.labelWidth = 100;
                        EditorGUILayout.LabelField("1 mm (File) to 0.001 m (Unity)", GUILayout.MaxWidth(210));
                        EditorGUIUtility.labelWidth = -1;
                    }
                    EditorGUILayout.Separator();
                    using (new EditorGUILayout.HorizontalScope()) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            using (new EditorGUILayout.VerticalScope()) {
                                using (new EditorGUILayout.HorizontalScope()) {
                                    bool value = GUILayout.Toggle(Model.importBlendShapes, "", UIStyles.LowerToggle);
                                    if (Model.importBlendShapes != value) Undo.RegisterCompleteObjectUndo(Model, UNDO_PROPERTY);
                                    Model.importBlendShapes = value;
                                    GUILayout.Label("Import BlendShapes", UIStyles.LeftAlignedLabel);
                                    GUILayout.FlexibleSpace();
                                }
                                EditorGUILayout.Separator();
                                using (new EditorGUILayout.HorizontalScope()) {
                                    bool value = GUILayout.Toggle(Model.importVisibility, "", UIStyles.LowerToggle);
                                    if (Model.importVisibility != value) Undo.RegisterCompleteObjectUndo(Model, UNDO_PROPERTY);
                                    Model.importVisibility = value;
                                    GUILayout.Label("Import Visibility", UIStyles.LeftAlignedLabel);
                                    GUILayout.FlexibleSpace();
                                }
                            }
                        } using (new EditorGUILayout.VerticalScope()) {
                            using (new EditorGUILayout.HorizontalScope()) {
                                GUILayout.Label("Mesh Optimization");
                                var value = (MeshOptimizationFlags) EditorGUILayout.EnumPopup(Model.meshOptimizationFlags, GUILayout.MaxWidth(150));
                                if (Model.meshOptimizationFlags != value) Undo.RegisterCompleteObjectUndo(Model, UNDO_PROPERTY);
                                Model.meshOptimizationFlags = value;
                            }
                            EditorGUILayout.Separator();
                            using (new EditorGUILayout.HorizontalScope()) {
                                GUILayout.Label("Import Normals");
                                var value = (ModelImporterNormals) EditorGUILayout.EnumPopup(Model.importNormals, GUILayout.MaxWidth(150));
                                if (Model.importNormals != value) Undo.RegisterCompleteObjectUndo(Model, UNDO_PROPERTY);
                                Model.importNormals = value;
                            }
                        }
                    }
                    EditorGUILayout.Separator();
                    using (new EditorGUILayout.HorizontalScope()) {
                        GUIContent importerContent = new GUIContent(" Open Model Importer", EditorUtils.FetchIcon("Settings"));
                        if (GUILayout.Button(importerContent, GUILayout.MaxWidth(PANEL_WIDTH / 2), GUILayout.MaxHeight(19))) {
                            EditorUtils.OpenAssetProperties(Model.assetPath);
                        }
                        GUIContent projectContent = new GUIContent(" Show Model In Project", EditorUtils.FetchIcon("d_Folder Icon"));
                        if (GUILayout.Button(projectContent, GUILayout.MaxWidth(PANEL_WIDTH / 2), GUILayout.MaxHeight(19))) {
                            EditorUtils.PingObject(Model);
                        }
                    }
                    EditorGUILayout.Separator();
                    EditorUtils.DrawSeparatorLines("Ext Model Utilities", true);
                    using (new EditorGUILayout.HorizontalScope()) {
                        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(PANEL_WIDTH * 3f/5f), GUILayout.Height(60))) {
                            if (notes == null) {
                                string defaultText = editNotes ? "" : "<i>No notes were found;</i>";
                                notes = Reader.ModelExtData.notes != null ? string.IsNullOrWhiteSpace(Reader.ModelExtData.notes) 
                                                                   ? defaultText : Reader.ModelExtData.notes : defaultText;
                            } using (new EditorGUILayout.HorizontalScope()) {
                                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandHeight(false))) {
                                    GUILayout.FlexibleSpace();
                                    using (new EditorGUILayout.HorizontalScope()) {
                                        GUILayout.FlexibleSpace();
                                        GUILayout.Label("Notes:");
                                        GUILayout.FlexibleSpace();
                                    }
                                    GUILayout.FlexibleSpace();
                                }
                                GUIStyle noteStyle = editNotes
                                    ? new GUIStyle(GUI.skin.label) { wordWrap = true, richText = true }
                                    : new GUIStyle(EditorStyles.boldLabel) { wordWrap = true , richText = true };
                                using (new EditorGUILayout.VerticalScope(editNotes ? new GUIStyle(EditorStyles.textArea) { padding = new RectOffset(4, 4, 4, 4) }
                                                                                   : new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(4, 4, 4, 4) })) {
                                    using (var noteView = new EditorGUILayout.ScrollViewScope(noteScroll, false, false, GUIStyle.none,
                                           GUI.skin.verticalScrollbar, GUI.skin.scrollView, GUILayout.Height(60))) {
                                        noteScroll = noteView.scrollPosition;
                                        using (new EditorGUILayout.VerticalScope()) {
                                            using (new EditorGUILayout.HorizontalScope()) {
                                                if (!editNotes) {
                                                    GUILayout.Label(notes, noteStyle, GUILayout.MinWidth(185));
                                                } else notes = GUILayout.TextArea(notes, noteStyle, GUILayout.MinWidth(185));
                                                EditorGUILayout.Space(15);
                                            }
                                        }     
                                    }
                                }
                            } using (new EditorGUILayout.HorizontalScope()) {
                                if (editNotes) {
                                    GUI.color = UIColors.Green;
                                    if (GUILayout.Button("Save", EditorStyles.miniButton)) {
                                        UpdateAssetNotes(notes);
                                        noteScroll = Vector2.zero;
                                        notes = null;
                                        editNotes = false;
                                    }
                                    GUI.color = UIColors.Red;
                                    if (GUILayout.Button("Cancel", EditorStyles.miniButton)) {
                                        UpdateAssetNotes(Reader.ModelExtData.notes);
                                        noteScroll = Vector2.zero;
                                        notes = null;
                                        editNotes = false;
                                    }
                                    GUI.color = Color.white;
                                } else if (GUILayout.Button("Edit Notes")) {
                                    notes = Reader.ModelExtData.notes;
                                    editNotes = true;
                                }
                            }
                        } using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.Height(22))) {
                                GUIStyle labelStyle = new GUIStyle(UIStyles.CenteredLabelBold) { contentOffset = new Vector2(0, 2) };
                                labelStyle.fontSize -= 1;
                                GUILayout.Label("Ext Data Status", labelStyle);
                            } using (new EditorGUILayout.HorizontalScope()) {
                                EditorUtils.DrawCustomHelpBox("Version Up-To-Date", EditorUtils.FetchIcon("Valid"), 0, 18);
                            } using (new EditorGUILayout.HorizontalScope()) {
                                EditorUtils.DrawCustomHelpBox("Reimported In Library", EditorUtils.FetchIcon("Valid"), 0, 18);
                            } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.Height(18))) {
                                GUI.color = UIColors.Blue;
                                if (GUILayout.Button("<b>Reimport</b>", new GUIStyle(GUI.skin.button) { 
                                                                           fontSize = 11, richText = true }, GUILayout.Height(19))) {
                                    AssetPreprocessorGUI.LibraryReimport(Model);
                                }
                                GUI.color = Color.white;
                            }
                        }
                    }
                }
            }
        }
    }
}