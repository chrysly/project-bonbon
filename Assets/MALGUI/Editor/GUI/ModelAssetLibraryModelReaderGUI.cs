using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using MainGUI = ModelAssetLibraryGUI;
using PrefabOrganizer = ModelAssetLibraryPrefabOrganizer;
using static ModelAssetLibraryModelReader;

/// <summary> 
/// Displays the interface of the Model Asset Library Reader; </summary>
/// </summary>
public static class ModelAssetLibraryModelReaderGUI {

    #region | GUI-Exclusive Variables |

    /// <summary> Found myself using this number a lot. Right side width; </summary>
    private readonly static float panelWidth = 440;

    /// <summary> String to display on property undoes; </summary>
    private const string UNDO_PROPERTY = "Model Importer Property Change";

    /// <summary> String to display on material swap undoes; </summary>
    private const string UNDO_MATERIAL_CHANGE = "Material Swap";

    /// <summary> Potential search modes for the Materials Section; </summary>
    private enum MaterialSearchMode {
        /// <summary> Start from a list of meshes and pick from the materials they containt; </summary>
        Mesh,
        /// <summary> Start from a list of materials and pick from a list of meshes that have them assigned; </summary>
        Material
    } /// <summary> Selected distribution of Available Meshes and Materials in the GUI; </summary>
    private static MaterialSearchMode materialSearchMode;

    private static bool editNotes;

    /// <summary> Temporary notes stored in the GUI; </summary>
    private static string notes;

    /// <summary>
    /// Behold! My scroller stuff...
    /// </summary>

    private static Vector2 noteScroll;

    private static Vector2 meshUpperScroll;
    private static Vector2 meshLowerScroll;

    private static Vector2 topMaterialScroll;
    private static Vector2 leftMaterialScroll;
    private static Vector2 rightMaterialScroll;

    private static Vector2 prefabLogScroll;
    private static Vector2 prefabListScroll;

    #endregion

    #region | Global Section Calls |

    /// <summary>
    /// Call the appropriate section function based on GUI Selection;
    /// </summary>
    /// <param name="sectionType"> Section selected in the GUI; </param>
    public static void ShowSelectedSection() {

        if (ReferencesAreFlushed()) {
            EditorUtils.DrawScopeCenteredText("Selected Asset Data will be displayed here;");
            return;
        }

        switch (ActiveSection) {
            case SectionType.Model:
                ShowModelSection();
                break;
            case SectionType.Meshes:
                ShowMeshesSection();
                break;
            case SectionType.Materials:
                ShowMaterialsSection();
                break;
            case SectionType.Prefabs:
                ShowPrefabsSection();
                break;
            case SectionType.Rig:
                WIP();
                break;
            case SectionType.Animations:
                WIP();
                break;
            case SectionType.Skeleton:
                WIP();
                break;
        }
    }

    /// <summary>
    /// Refresh the scrolling variables and reset all section dependent variables;
    /// </summary>
    public static void RefreshSections() {
        notes = null;
        editNotes = false;
        materialSearchMode = 0;
        meshUpperScroll = Vector2.zero;
        meshLowerScroll = Vector2.zero;
        topMaterialScroll = Vector2.zero;
        leftMaterialScroll = Vector2.zero;
        rightMaterialScroll = Vector2.zero;
        prefabLogScroll = Vector2.zero;
        prefabListScroll = Vector2.zero;
    }

    #endregion

    #region | Global Toolbar |

    /// <summary>
    /// Draws the toolbar for the Model Reader;
    /// </summary>
    public static void DrawModelReaderToolbar() {
        GUI.enabled = Model != null;
        switch (ActiveAssetMode) {
            case AssetMode.Model:
                DrawToolbarButton(SectionType.Model, 72, 245);
                DrawToolbarButton(SectionType.Meshes, 72, 245);
                DrawToolbarButton(SectionType.Materials, 72, 245);
                DrawToolbarButton(SectionType.Prefabs, 112, 245);
                break;
            case AssetMode.Animation:
                DrawToolbarButton(SectionType.Rig, 66, 245);
                DrawToolbarButton(SectionType.Animations, 82, 245);
                DrawToolbarButton(SectionType.Skeleton, 72, 245);
                break;
        } GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Asset Mode:", UIStyles.ToolbarText, GUILayout.Width(110));
        AssetMode newAssetMode = (AssetMode) EditorGUILayout.EnumPopup(ActiveAssetMode, UIStyles.ToolbarPaddedPopUp, GUILayout.MinWidth(100), GUILayout.MaxWidth(180));
        if (ActiveAssetMode != newAssetMode) SetSelectedAssetMode(newAssetMode);
        GUI.enabled = true;
    }

    /// <summary>
    /// Draws a toolbar button for a given section;
    /// </summary>
    /// <param name="sectionType"> Section selected by this button; </param>
    /// <param name="minWidth"> Minimum width of the button; </param>
    /// <param name="maxWidth"> Maximum width of the button; </param>
    private static void DrawToolbarButton(SectionType sectionType, float minWidth, float maxWidth) {
        GUIStyle buttonStyle = sectionType == ActiveSection ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton;
        if (GUILayout.Button(System.Enum.GetName(typeof(SectionType), sectionType),
                             buttonStyle, GUILayout.MinWidth(minWidth), GUILayout.MaxWidth(maxWidth))) {
            SetSelectedSection(sectionType);
        }
    }

    #endregion

    #region | Model Section |

    /// <summary> GUI Display for the Model Section </summary>
    public static void ShowModelSection() {

        using (new EditorGUILayout.HorizontalScope()) {
            /// Model Preview + Model Details;
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(200), GUILayout.Height(200))) {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(192), GUILayout.Height(192))) {
                    GUILayout.Label("Model Preview", UIStyles.CenteredLabel);
                    DrawObjectPreviewEditor(Prefab, 192, 192);

                    using (new EditorGUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(196), GUILayout.Height(100))) {
                            GUILayout.Label("Model Details", UIStyles.CenteredLabel);
                            GUILayout.FlexibleSpace();
                            EditorUtils.DrawLabelPair("Vertex Count:", GlobalVertexCount.ToString());
                            EditorUtils.DrawLabelPair("Triangle Count: ", GlobalTriangleCount.ToString());
                            EditorUtils.DrawLabelPair("Mesh Count: ", MeshRenderers.Count.ToString());
                            EditorUtils.DrawLabelPair("Rigged: ", Model.avatarSetup == 0 ? "No" : "Yes");
                        } GUILayout.FlexibleSpace();
                    }
                }
            }
            /// Model Data;
            using (var view = new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(panelWidth))) {

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
                } EditorGUILayout.Separator();
                using (new EditorGUILayout.HorizontalScope()) {
                    using (new EditorGUILayout.HorizontalScope()) {
                        using (new EditorGUILayout.VerticalScope()) {
                            using (new EditorGUILayout.HorizontalScope()) {
                                bool value = GUILayout.Toggle(Model.importBlendShapes, "", UIStyles.LowerToggle);
                                if (Model.importBlendShapes != value) Undo.RegisterCompleteObjectUndo(Model, UNDO_PROPERTY);
                                Model.importBlendShapes = value;
                                GUILayout.Label("Import BlendShapes", UIStyles.LeftAlignedLabel);
                                GUILayout.FlexibleSpace();
                            } EditorGUILayout.Separator();
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
                        } EditorGUILayout.Separator();
                        using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.Label("Import Normals");
                            var value = (ModelImporterNormals) EditorGUILayout.EnumPopup(Model.importNormals, GUILayout.MaxWidth(150));
                            if (Model.importNormals != value) Undo.RegisterCompleteObjectUndo(Model, UNDO_PROPERTY);
                            Model.importNormals = value;
                        }
                    }
                } EditorGUILayout.Separator();
                using (new EditorGUILayout.HorizontalScope()) {
                    GUIContent importerContent = new GUIContent(" Open Model Importer", EditorUtils.FetchIcon("Settings"));
                    if (GUILayout.Button(importerContent, GUILayout.MaxWidth(panelWidth / 2), GUILayout.MaxHeight(19))) {
                        EditorUtils.OpenAssetProperties(Model.assetPath);
                    } GUIContent projectContent = new GUIContent(" Show Model In Project", EditorUtils.FetchIcon("d_Folder Icon"));
                    if (GUILayout.Button(projectContent, GUILayout.MaxWidth(panelWidth / 2), GUILayout.MaxHeight(19))) {
                        EditorUtils.PingObject(Model);
                    }
                } EditorGUILayout.Separator();
                EditorUtils.DrawSeparatorLines("Ext Model Utilities", true);
                using (new EditorGUILayout.HorizontalScope()) {
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(panelWidth * 3f/5f), GUILayout.Height(60))) {
                        if (notes == null) {
                            string defaultText = editNotes ? "" : "<i>No notes were found;</i>";
                            notes = ModelExtData.notes != null ? string.IsNullOrWhiteSpace(ModelExtData.notes) 
                                                               ? defaultText : ModelExtData.notes : defaultText;
                        } using (new EditorGUILayout.HorizontalScope()) {
                            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.ExpandHeight(false))) {
                                GUILayout.FlexibleSpace();
                                using (new EditorGUILayout.HorizontalScope()) {
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label("Notes:");
                                    GUILayout.FlexibleSpace();
                                } GUILayout.FlexibleSpace();
                            } GUIStyle noteStyle = editNotes
                                ? new GUIStyle(GUI.skin.label) { wordWrap = true, richText = true }
                                : new GUIStyle(EditorStyles.boldLabel) { wordWrap = true , richText = true };
                            using (new EditorGUILayout.VerticalScope(editNotes ? new GUIStyle(EditorStyles.textArea) { margin = new RectOffset(0, 0, 0, 2) }
                                                                               : UIStyles.WindowBox)) {
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
                                } GUI.color = UIColors.Red;
                                if (GUILayout.Button("Cancel", EditorStyles.miniButton)) {
                                    UpdateAssetNotes(ModelExtData.notes);
                                    noteScroll = Vector2.zero;
                                    notes = null;
                                    editNotes = false;
                                } GUI.color = Color.white;
                            } else if (GUILayout.Button("Edit Notes")) {
                                notes = ModelExtData.notes;
                                editNotes = true;
                            }
                        }
                    } using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            GUIStyle labelStyle = new GUIStyle(UIStyles.CenteredLabelBold);
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
                                ModelAssetLibraryAssetPreprocessorGUI.LibraryReimport(Model);
                            } GUI.color = Color.white;
                        }

                    }
                }
            }
        }
    }

    #endregion

    #region | Meshes Section |

    /// <summary> GUI Display for the Meshes Section </summary>
    public static void ShowMeshesSection() {
        using (new EditorGUILayout.HorizontalScope()) {
            /// Mesh Preview + Mesh Details;
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(200), GUILayout.Height(200))) {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(192), GUILayout.Height(192))) {
                    GUILayout.Label("Mesh Preview", UIStyles.CenteredLabel);
                    if (SelectedMesh != null) {
                        if (SelectedSubmeshIndex == 0) {
                            DrawMeshPreviewEditor(SelectedMesh.mesh, 192, 192);
                            GUIContent settingsContent = new GUIContent(" Preview Settings", EditorUtils.FetchIcon("d_Mesh Icon"));
                            if (GUILayout.Button(settingsContent, GUILayout.MaxHeight(19))) {
                                ModelAssetLibraryExtraMeshPreview
                                    .ShowPreviewSettings(ReaderMeshPreview,
                                                         GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect()));
                            }
                        } else DrawObjectPreviewEditor(DummyGameObject, 192, 192);
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                            if (GUILayout.Button("Open In Materials")) SwitchToMaterials(SelectedMesh.renderer);
                        }
                    } else EditorUtils.DrawTexture(CustomTextures.noMeshPreview, 192, 192);

                    using (new EditorGUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(196), GUILayout.Height(60))) {
                            if (SelectedMesh != null) {
                                GUILayout.Label("Mesh Details", UIStyles.CenteredLabel);
                                GUILayout.FlexibleSpace();
                                EditorUtils.DrawLabelPair("Vertex Count:", LocalVertexCount.ToString());
                                EditorUtils.DrawLabelPair("Triangle Count: ", LocalTriangleCount.ToString());
                            } else {
                                EditorUtils.DrawScopeCenteredText("No Mesh Selected");
                            }
                        } GUILayout.FlexibleSpace();
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(panelWidth))) {
                EditorUtils.DrawSeparatorLines("Renderer Details", true);
                if (SelectedMesh != null) {
                    using (new EditorGUILayout.HorizontalScope()) {
                        EditorUtils.DrawLabelPair("Skinned Mesh:", SelectedMesh.renderer is SkinnedMeshRenderer ? "Yes" : "No");
                        GUILayout.FlexibleSpace();
                        EditorUtils.DrawLabelPair("No. Of Submeshes:", SelectedMesh.mesh.subMeshCount.ToString());
                        GUILayout.FlexibleSpace();
                        EditorUtils.DrawLabelPair("Materials Assigned:", SelectedMesh.renderer.sharedMaterials.Length.ToString());
                    } GUIContent propertiesContent = new GUIContent(" Open Mesh Properties", EditorUtils.FetchIcon("Settings"));
                    if (GUILayout.Button(propertiesContent, GUILayout.MaxHeight(19))) EditorUtils.OpenAssetProperties(SelectedMesh.mesh);
                } else {
                    EditorGUILayout.Separator();
                    GUILayout.Label("No Mesh Selected", UIStyles.CenteredLabelBold);
                    EditorGUILayout.Separator();
                }

                EditorUtils.DrawSeparatorLines("Submeshes", true);
                if (SelectedMesh != null) {
                    using (var view = new EditorGUILayout.ScrollViewScope(meshUpperScroll, true, false,
                                                                      GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar,
                                                                      GUI.skin.box, GUILayout.MaxWidth(panelWidth), GUILayout.MaxHeight(53))) {
                        meshUpperScroll = view.scrollPosition;
                        using (new EditorGUILayout.HorizontalScope()) {
                            for (int i = 0; i < SelectedMesh.mesh.subMeshCount; i++) DrawSubMeshSelectionButton(i + 1);
                        }
                    }
                } else {
                    EditorGUILayout.Separator();
                    GUILayout.Label("No Mesh Selected", UIStyles.CenteredLabelBold);
                    EditorGUILayout.Separator();
                }

                DrawMeshSearchArea();
            }
        }
    }

    /// <summary>
    /// Displays a horizontal scrollview with all the meshes available in the model to select from;
    /// </summary>
    /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
    /// <param name="selectMaterialRenderer"> Whether the button is being used in the Materials Section; </param>
    private static void DrawMeshSearchArea(float scaleMultiplier = 1f, bool selectMaterialRenderer = false) {

        EditorUtils.DrawSeparatorLines("All Meshes", true);

        using (var view = new EditorGUILayout.ScrollViewScope(meshLowerScroll, true, false,
                                                              GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar,
                                                              GUI.skin.box, GUILayout.MaxWidth(panelWidth), GUILayout.MaxHeight(scaleMultiplier == 1 ? 130 : 110))) {
            meshLowerScroll = view.scrollPosition;
            using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(panelWidth), GUILayout.MaxHeight(scaleMultiplier == 1 ? 130 : 110))) {
                foreach (MeshRendererPair mrp in MeshRenderers) {
                    if (mrp.renderer is SkinnedMeshRenderer) {
                        DrawMeshSelectionButton((mrp.renderer as SkinnedMeshRenderer).sharedMesh,
                                                mrp.renderer.gameObject, mrp.renderer, scaleMultiplier, selectMaterialRenderer);
                    } else if (mrp.renderer is MeshRenderer) {
                        DrawMeshSelectionButton(mrp.filter.sharedMesh, mrp.renderer.gameObject, mrp.renderer, scaleMultiplier, selectMaterialRenderer);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draw a button to select a given submesh;
    /// </summary>
    /// <param name="mesh"> Mesh that the button will select; </param>
    /// <param name="gameObject"> GameObject of the renderer; </param>
    /// <param name="renderer"> Renderer containing the mesh; </param>
    /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
    /// <param name="selectMaterialRenderer"> Whether the button is used for the Materials section; </param>
    private static void DrawMeshSelectionButton(Mesh mesh, GameObject gameObject, Renderer renderer, float scaleMultiplier, bool selectMaterialRenderer = false) {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxWidth(1))) {
            EditorUtils.DrawTexture(MeshPreviewDict[renderer], 80 * scaleMultiplier, 80 * scaleMultiplier);
            if ((SelectedMesh != null && SelectedMesh.mesh == mesh)
                || (SelectedMaterial != null && SelectedMaterial.renderer == renderer)) {
                GUILayout.Label("Selected", UIStyles.CenteredLabelBold, GUILayout.MaxWidth(80 * scaleMultiplier), GUILayout.MaxHeight(19 * scaleMultiplier));
            } else if (GUILayout.Button("Open", GUILayout.MaxWidth(80 * scaleMultiplier))) {
                if (selectMaterialRenderer) {
                    SetSelectedRenderer(gameObject, renderer);
                } else {
                    SetSelectedMesh(mesh, gameObject, renderer);
                }
            }
        }
    }

    /// <summary>
    /// Update the selected submesh index and update the preview to reflect it;
    /// </summary>
    /// <param name="index"> Index of the submesh to select; </param>
    private static void DrawSubMeshSelectionButton(int index) {
        bool isSelected = index == SelectedSubmeshIndex;
        GUIStyle buttonStyle = isSelected ? EditorStyles.helpBox : GUI.skin.box;
        using (new EditorGUILayout.VerticalScope(buttonStyle, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35))) {
            if (GUILayout.Button(index.ToString(), UIStyles.TextureButton, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35))) {
                if (isSelected) index = 0;
                SetSelectedSubMesh(index);
            }
        }
    }

    /// <summary>
    /// A button to switch to the Material section with the current mesh selected;
    /// </summary>
    /// <param name="renderer"> The renderer to keep through the section change; </param>
    private static void SwitchToMaterials(Renderer renderer) {
        SetSelectedSection(SectionType.Materials);
        SetSelectedRenderer(renderer.gameObject, renderer);
    }

    #endregion

    #region | Materials Section |

    /// <summary> GUI Display for the Materials Section </summary>
    public static void ShowMaterialsSection() {

        if (SelectedMaterial != null && SelectedMaterial.material != null) {
            DrawMaterialInspector(SelectedMaterial.material);
        }

        using (new EditorGUILayout.VerticalScope()) {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(660))) {
                EditorUtils.DrawSeparatorLine();
                using (var hscope = new EditorGUILayout.HorizontalScope()) {
                    GUIStyle style = new GUIStyle(UIStyles.CenteredLabelBold);
                    style.contentOffset = new Vector2(0, 1);
                    GUILayout.Label("Material Importer Settings:", style);
                    Model.materialImportMode = (ModelImporterMaterialImportMode) EditorGUILayout.EnumPopup(Model.materialImportMode,
                                                                                                           GUILayout.MaxWidth(140), GUILayout.Height(16));
                    switch (Model.materialImportMode) {
                        case ModelImporterMaterialImportMode.None:
                            EditorUtils.DrawCustomHelpBox(" None: Material Slots are strictly Preview Only;", EditorUtils.FetchIcon("Warning"), 320, 18);
                            UpdateSlotChangedStatus();
                            break;
                        case ModelImporterMaterialImportMode.ImportStandard:
                            EditorUtils.DrawCustomHelpBox(" Standard: Material Slots can be set manually;", EditorUtils.FetchIcon("Valid"), 320, 18);
                            break;
                        case ModelImporterMaterialImportMode.ImportViaMaterialDescription:
                            EditorUtils.DrawCustomHelpBox(" Material: Material Slots can be set manually;", EditorUtils.FetchIcon("Valid"), 320, 18);
                            break;
                    }
                } EditorUtils.DrawSeparatorLine();
            }

            using (new EditorGUILayout.HorizontalScope()) {

                using (new EditorGUILayout.VerticalScope(GUILayout.Width(200), GUILayout.Height(200))) {
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(200), GUILayout.Height(200))) {
                        GUILayout.Label("Material Preview", UIStyles.CenteredLabel);
                        if (SelectedMaterial != null && SelectedMaterial.renderer != null) {
                            DrawObjectPreviewEditor(DummyGameObject, 192, 192);
                            if (GUILayout.Button("Update Preview")) {
                                UpdateObjectPreview();
                            }
                        } else EditorUtils.DrawTexture(CustomTextures.noMaterialPreview, 192, 192);
                    }

                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                        GUILayout.Label("Search Mode:", UIStyles.RightAlignedLabel);
                        materialSearchMode = (MaterialSearchMode) EditorGUILayout.EnumPopup(materialSearchMode);
                    }
                    if (SelectedMaterial != null && SelectedMaterial.renderer != null) {
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                            if (GUILayout.Button("Open In Meshes")) SwitchToMeshes(SelectedMaterial.renderer);
                        }
                    }
                }

                using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(panelWidth))) {

                    switch (materialSearchMode) {
                        case MaterialSearchMode.Mesh:
                            DrawMeshSearchArea(0.76f, true);
                            using (new EditorGUILayout.HorizontalScope()) {
                                DrawAvailableMaterials(0.76f);
                                DrawMaterialSlots();
                            } break;
                        case MaterialSearchMode.Material:
                            DrawMaterialSearchArea(0.76f);
                            using (new EditorGUILayout.HorizontalScope()) {
                                DrawAvailableMeshes(0.76f);
                                DrawMaterialSlots();
                            } break;
                    }
                }
            }

            if (Model.materialImportMode == 0) GUI.enabled = false;
            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(660))) {
                EditorUtils.DrawSeparatorLine(); 
                using (var hscope = new EditorGUILayout.HorizontalScope()) {
                    GUIStyle style = new GUIStyle(UIStyles.CenteredLabelBold);
                    style.contentOffset = new Vector2(0, 1);
                    GUILayout.Label("Material Location Settings:", style);
                    var potentialLocation = (ModelImporterMaterialLocation) EditorGUILayout.EnumPopup(Model.materialLocation, 
                                                                                                                     GUILayout.MaxWidth(180));
                    if (Model.materialLocation != potentialLocation) {
                        Model.materialLocation = potentialLocation;
                        Model.SaveAndReimport();
                    } switch (Model.materialLocation) {
                        case ModelImporterMaterialLocation.External:
                            EditorUtils.DrawCustomHelpBox(" External: Material Slots are strictly Preview Only;", EditorUtils.FetchIcon("Warning"), 300, 18);
                            UpdateSlotChangedStatus();
                            break;
                        case ModelImporterMaterialLocation.InPrefab:
                            if (HasStaticSlotChanges) {
                                GUI.color = UIColors.Green;
                                if (GUILayout.Button("<b>Assign Materials</b>", UIStyles.SquashedButton, GUILayout.MaxWidth(125))) {
                                    AssignMaterialsPersistently();
                                } GUI.color = UIColors.Red;
                                if (GUILayout.Button("<b>Revert Materials</b>", UIStyles.SquashedButton, GUILayout.MaxWidth(125))) {
                                    Undo.RecordObject(SelectedMaterial.renderer, UNDO_MATERIAL_CHANGE);
                                    ResetSlotChanges();
                                } GUI.color = Color.white;
                                GUIContent helperContent = new GUIContent(EditorUtils.FetchIcon("d__Help"));
                                if (GUILayout.Button(helperContent, GUILayout.MaxWidth(25), GUILayout.MaxHeight(18))) {
                                    MaterialHelperWindow = ModelAssetLibraryMaterialHelper.ShowWindow(Model);
                                }
                            } else {
                                GUI.enabled = false;
                                GUILayout.Button("<b>Material Slots Up-to-Date</b>", UIStyles.SquashedButton, GUILayout.MaxWidth(300), GUILayout.MaxHeight(18));
                                GUI.enabled = true;
                            } break;
                    } 
                } EditorUtils.DrawSeparatorLine();
            } if (Model.materialImportMode == 0) GUI.enabled = true;
        }
    }

    /// <summary>
    /// Draws the 'All materials' scrollview at the top;
    /// <br></br> Drawn in Material Search Mode;
    /// </summary>
    /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
    private static void DrawMaterialSearchArea(float scaleMultiplier = 1f) {

        using (new EditorGUILayout.VerticalScope(GUILayout.Width(panelWidth), GUILayout.Height(145))) {
            EditorUtils.DrawSeparatorLines("All Materials", true);
            using (var view = new EditorGUILayout.ScrollViewScope(topMaterialScroll, true, false,
                                                          GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar,
                                                          GUI.skin.box, GUILayout.MaxWidth(panelWidth), GUILayout.MaxHeight(110))) {
                topMaterialScroll = view.scrollPosition;
                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(panelWidth), GUILayout.Height(110))) {
                    foreach (Material material in MaterialDict.Keys) DrawMaterialButton(material, scaleMultiplier);
                }
            }
        }
    }

    /// <summary>
    /// Draws the 'Available Materials' scrollview at the top;
    /// <br></br> Drawn in Mesh Search Mode;
    /// </summary>
    /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
    private static void DrawAvailableMaterials(float scaleMultiplier = 1f) {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(panelWidth / 2), GUILayout.Height(145))) {
            EditorUtils.DrawSeparatorLines("Available Materials", true);
            if (SelectedMaterial != null && SelectedMaterial.renderer != null) {
                using (var view = new EditorGUILayout.ScrollViewScope(leftMaterialScroll, true, false,
                                                      GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar,
                                                      GUI.skin.box, GUILayout.MaxWidth(panelWidth / 2), GUILayout.MaxHeight(110))) {
                    leftMaterialScroll = view.scrollPosition;
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(panelWidth / 2), GUILayout.Height(110))) {
                        Material[] uniqueMaterials = GetUniqueMaterials(SelectedMaterial.renderer.sharedMaterials);
                        foreach (Material material in uniqueMaterials) {
                            DrawMaterialButton(material, scaleMultiplier);
                        }
                    }
                }
            } else EditorUtils.DrawScopeCenteredText("No Material Selected");
        }
    }

    /// <summary>
    /// Draws the 'Available Meshes' scrollview at the bottom left;
    /// <br></br> Drawn in Material Search Mode;
    /// </summary>
    /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
    private static void DrawAvailableMeshes(float scaleMultiplier = 1f) {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(panelWidth / 2), GUILayout.Height(145))) {
            EditorUtils.DrawSeparatorLines("Available Meshes", true);
            if (SelectedMaterial != null && SelectedMaterial.material != null) {
                using (var view = new EditorGUILayout.ScrollViewScope(leftMaterialScroll, true, false,
                                                                  GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar,
                                                                  GUI.skin.box, GUILayout.MaxWidth(panelWidth / 2), GUILayout.MaxHeight(110))) {
                    leftMaterialScroll = view.scrollPosition;
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(panelWidth / 2), GUILayout.Height(110))) {
                        foreach (MeshRendererPair mrp in MaterialDict[SelectedMaterial.material]) {
                            if (mrp.renderer is SkinnedMeshRenderer) {
                                DrawMeshSelectionButton((mrp.renderer as SkinnedMeshRenderer).sharedMesh,
                                                        mrp.renderer.gameObject, mrp.renderer, scaleMultiplier, true);
                            } else if (mrp.renderer is MeshRenderer) {
                                DrawMeshSelectionButton(mrp.filter.sharedMesh, mrp.renderer.gameObject, mrp.renderer, scaleMultiplier, true);
                            }
                        }
                    }
                }
            } else EditorUtils.DrawScopeCenteredText("No Material Selected");
        }
    }

    /// <summary>
    /// Draws the 'Material Slots' scrollview at the bottom right;
    /// <br></br> Drawn for all search modes;
    /// </summary>
    private static void DrawMaterialSlots() {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(panelWidth / 2), GUILayout.Height(145))) {
            EditorUtils.DrawSeparatorLines("Material Slots", true);
            if (SelectedMaterial != null && SelectedMaterial.renderer != null) {
                using (var view = new EditorGUILayout.ScrollViewScope(rightMaterialScroll, true, false,
                                                                  GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar,
                                                                  GUI.skin.box, GUILayout.MaxWidth(panelWidth / 2), GUILayout.MaxHeight(110))) {
                    rightMaterialScroll = view.scrollPosition;
                    using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(panelWidth / 2), GUILayout.MaxHeight(110))) {
                        Dictionary<string, Material> tempDict = new Dictionary<string, Material>(StaticMaterialSlots);
                        foreach (KeyValuePair<string, Material> kvp in tempDict) {
                            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxWidth(50), GUILayout.MaxHeight(35))) {
                                using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(60))) {
                                    GUILayout.FlexibleSpace();
                                    Material material = (Material) EditorGUILayout.ObjectField(kvp.Value, typeof(Material), false, GUILayout.MaxWidth(50));
                                    if (material != kvp.Value) {
                                        ReplacePersistentMaterial(kvp.Key, material);
                                    }
                                } using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(45))) {
                                    GUILayout.FlexibleSpace();
                                    if (kvp.Value != null) EditorUtils.DrawTexture(AssetPreview.GetAssetPreview(kvp.Value), 40, 40);
                                    else EditorUtils.DrawTexture(EditorUtils.FetchIcon("d_AutoLightbakingOff"), 40, 40);
                                } using (new EditorGUILayout.HorizontalScope(EditorStyles.selectionRect, GUILayout.MaxWidth(40), GUILayout.MaxHeight(8))) {
                                    GUIStyle tempStyle = new GUIStyle(EditorStyles.boldLabel);
                                    tempStyle.fontSize = 8;
                                    GUILayout.Label(kvp.Key, tempStyle, GUILayout.MaxHeight(8), GUILayout.MaxWidth(40));
                                    GUILayout.FlexibleSpace();
                                }
                            }
                        }
                    }
                }
            } else EditorUtils.DrawScopeCenteredText("No Mesh Selected");
        }
    }

    /// <summary>
    /// Draws a button for a selectable material;
    /// </summary>
    /// <param name="material"> Material to draw the button for; </param>
    /// <param name="scaleMultiplier"> A lazy scale multiplier; </param>
    private static void DrawMaterialButton(Material material, float scaleMultiplier = 1f) {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxWidth(1))) {
            EditorUtils.DrawTexture(AssetPreview.GetAssetPreview(material), 80 * scaleMultiplier, 80 * scaleMultiplier);
            if (SelectedMaterial != null && SelectedMaterial.material == material) {
                GUILayout.Label("Selected", UIStyles.CenteredLabelBold, GUILayout.MaxWidth(80 * scaleMultiplier), GUILayout.MaxHeight(19 * scaleMultiplier));
            } else if (GUILayout.Button("Open", GUILayout.MaxWidth(80 * scaleMultiplier))) {
                SetSelectedMaterial(material);
            }
        }
    }

    /// <summary>
    /// Open the currently selected Mesh in the Meshes tab;
    /// </summary>
    /// <param name="renderer"> Renderer holding the mesh; </param>
    private static void SwitchToMeshes(Renderer renderer) {
        SetSelectedSection(SectionType.Meshes);
        MeshRendererPair mrp = GetMRP(renderer);
        Mesh mesh = null;
        if (renderer is SkinnedMeshRenderer) {
            mesh = (mrp.renderer as SkinnedMeshRenderer).sharedMesh;
        } else if (renderer is MeshRenderer) {
            mesh = mrp.filter.sharedMesh;
        } SetSelectedMesh(mesh, renderer.gameObject, renderer);
    }

    #endregion

    #region | Prefabs Section |

    /// <summary> GUI Display for the Prefabs Section </summary>
    public static void ShowPrefabsSection() {

        using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(660))) {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(330), GUILayout.MaxHeight(140))) {
                EditorUtils.DrawSeparatorLines("Prefab Variant Registry", true);
                using (new EditorGUILayout.HorizontalScope()) {
                    GUILayout.Label("Register New Prefab Variant:");
                    if (GUILayout.Button("Validate & Register")) {
                        if (ValidateFilename()) {
                            RegisterPrefab(ModelID, name);
                            RegisterPrefabLog("Added Prefab Variant: " + name + ".prefab;");
                        }
                    }
                } string impendingName = EditorGUILayout.TextField("Variant Name:", name);
                if (impendingName != name) {
                    if (NameCondition != 0) NameCondition = 0;
                    SetDefaultPrefabName(impendingName);
                } DrawNameConditionBox();
                GUILayout.FlexibleSpace();
                GUIContent folderContent = new GUIContent(" Show Prefabs Folder", EditorUtils.FetchIcon("d_Folder Icon"));
                if (GUILayout.Button(folderContent, EditorStyles.miniButton, GUILayout.MaxHeight(18))) {
                    EditorUtils.PingObject(AssetDatabase.LoadAssetAtPath<Object>(Model.assetPath.ToPrefabPath()));
                }
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(330), GUILayout.MaxHeight(140))) {
                EditorUtils.DrawSeparatorLines("Asset Library Logs", true);
                using (var view = new EditorGUILayout.ScrollViewScope(prefabLogScroll, GUI.skin.box)) {
                    prefabLogScroll = view.scrollPosition;
                    foreach (string line in PrefabActionLog) {
                        GUILayout.Label(line);
                    }
                } GUIContent clearContent = new GUIContent(" Clear", EditorUtils.FetchIcon("d_winbtn_win_close@2x"));
                if (GUILayout.Button(clearContent, EditorStyles.miniButton, GUILayout.MaxHeight(18))) PrefabActionLog.Clear();
            }
        }
        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(660))) {
            EditorUtils.DrawSeparatorLines("Registered Prefab Variants", true);
            using (var view = new EditorGUILayout.ScrollViewScope(prefabListScroll, GUILayout.ExpandHeight(false))) {
                prefabListScroll = view.scrollPosition;
                DrawPrefabCards();
            }
        }
    }

    /// <summary>
    /// Draw a box with useful information about the chosen file name and prefab creation;
    /// </summary>
    private static void DrawNameConditionBox() {
        switch (NameCondition) {
            case InvalidNameCondition.None:
                EditorGUILayout.HelpBox("Messages concerning the availability of the name written above will be displayed here;", MessageType.Info);
                break;
            case InvalidNameCondition.Empty:
                EditorGUILayout.HelpBox("The name of the file cannot be empty;", MessageType.Error);
                break;
            case InvalidNameCondition.Overwrite:
                EditorGUILayout.HelpBox("A file with that name already exists in the target directory. Do you wish to overwrite it?", MessageType.Warning);
                using (new EditorGUILayout.HorizontalScope()) {
                    if (GUILayout.Button("Overwrite")) {
                        RegisterPrefab(ModelID, name);
                        RegisterPrefabLog("Replaced Prefab Variant: " + name + ".prefab;");
                    } if (GUILayout.Button("Cancel")) {
                        NameCondition = InvalidNameCondition.None;
                    }
                } break;
            case InvalidNameCondition.Symbol:
                EditorGUILayout.HelpBox("The filename can only contain alphanumerical values and/or whitespace characters;", MessageType.Error);
                break;
            case InvalidNameCondition.Convention:
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
            case InvalidNameCondition.Success:
                GUIContent messageContent = new GUIContent(" Prefab Variant created successfully!", EditorUtils.FetchIcon("d_PreMatCube@2x"));
                EditorGUILayout.HelpBox(messageContent);
                break;
        }
    }

    /// <summary>
    /// Iterate over the prefab variants of the model and display a set of actions for each of them;
    /// </summary>
    private static void DrawPrefabCards() {
        if (PrefabVariantInfo != null && PrefabVariantInfo.Count > 0) {
            foreach (PrefabVariantData prefabData in PrefabVariantInfo) {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel) { contentOffset = new Vector2(2, 2) };
                    GUILayout.Label(prefabData.name, labelStyle, GUILayout.MaxWidth(260));
                    if (GUILayout.Button("Open Prefab", GUILayout.MaxWidth(150), GUILayout.MaxHeight(19))) {
                        EditorUtils.OpenAssetProperties(AssetDatabase.GUIDToAssetPath(prefabData.guid));
                    } if (GUILayout.Button("Open Organizer", GUILayout.MaxWidth(150), GUILayout.MaxHeight(19))) {
                        SwitchToOrganizer(prefabData.guid);
                    } GUI.color = UIColors.Red;
                    GUIContent deleteButton = new GUIContent(EditorUtils.FetchIcon("TreeEditor.Trash"));
                    if (GUILayout.Button(deleteButton, GUILayout.MaxWidth(75), GUILayout.MaxHeight(19))) {
                        if (ModelAssetLibraryModalPrefabDeletion.ConfirmPrefabDeletion(prefabData.name)) {
                            ModelAssetLibrary.DeletePrefab(prefabData.guid);
                            RegisterPrefabLog("Deleted Prefab Variant: " + prefabData.name + ";");
                            UpdatePrefabVariantInfo();
                        } GUIUtility.ExitGUI();
                    } GUI.color = Color.white;
                }
            }
        } else {
            GUILayout.Label("No Prefab Variants have been Registered for this Model;", UIStyles.CenteredLabelBold);
            EditorGUILayout.Separator();
        }
    }

    /// <summary>
    /// Switch to the Prefab Organizer tool and place a Search String with prefab's name;
    /// </summary>
    /// <param name="prefabID"> ID of the prefab used to redirect the GUI; </param>
    private static void SwitchToOrganizer(string prefabID) {
        MainGUI.SwitchActiveTool(MainGUI.ToolMode.PrefabOrganizer);
        string modelID = ModelAssetLibrary.PrefabDataDict[prefabID].modelID;
        string path = ModelAssetLibrary.ModelDataDict[modelID].path.RemovePathEnd("\\/");
        string name = ModelAssetLibrary.PrefabDataDict[prefabID].name;
        PrefabOrganizer.SetSelectedCategory(path);
        PrefabOrganizer.SetSearchString(name);
        GUIUtility.ExitGUI();
    }

    #endregion

    /// <summary>
    /// A neat message to display on unfinished sections;
    /// </summary>
    private static void WIP() {
        EditorUtils.DrawScopeCenteredText("This section is not fully implemented yet.\nYou should yell at Carlos in response to this great offense!");
    }
}