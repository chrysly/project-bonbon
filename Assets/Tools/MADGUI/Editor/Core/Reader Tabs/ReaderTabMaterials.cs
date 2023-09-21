using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CJUtils;
using ModelAssetDatabase.MADUtils;
using static ModelAssetDatabase.ModelAssetDatabaseGUI;
using static ModelAssetDatabase.Reader;

namespace ModelAssetDatabase {
    public class ReaderTabMaterials : ReaderTab {

        public ModelImporter Model { get { return Reader.Model; } }

        /// <summary> Dictionary mapping the current material slot selection; </summary>
        public Dictionary<string, Material> StaticMaterialSlots { get; private set; }

        /// <summary> Dictionary mapping the original material slot selection; </summary>
        public Dictionary<string, Material> OriginalMaterialSlots { get; private set; }

        /// <summary> Whether the current slot selection differs from the old selection; </summary>
        private bool hasStaticSlotChanges;

        /// <summary> Class that bundles properties relevant to the selected material for quick handling and disposal; </summary>
        private class SelectedMaterialProperties {
            public Material material;
            public GameObject gameObject;
            public Renderer renderer;

            public SelectedMaterialProperties(Material material, GameObject gameObject, Renderer renderer) {
                this.material = material;
                this.gameObject = gameObject;
                this.renderer = renderer;
            }

            public SelectedMaterialProperties(Material material) {
                this.material = material;
                gameObject = null;
                renderer = null;
            }

            public SelectedMaterialProperties(GameObject gameObject, Renderer renderer) {
                material = null;
                this.gameObject = gameObject;
                this.renderer = renderer;
            }
        } /// <summary> Relevant properties of the material selected in the GUI; </summary>
        private SelectedMaterialProperties selectedMaterial;

        /// <summary> An Editor Window displayed when a material asset is selected; </summary>
        private MaterialInspector MaterialInspectorWindow;

        /// <summary> An Editor Window displaying useful information about staging changes in the Materials Section; </summary>
        private MaterialHelper MaterialHelperWindow;

        /// <summary> String to display on material swap undoes; </summary>
        private const string UNDO_MATERIAL_CHANGE = "Material Swap";
    
        /// <summary> Whether to show the whole model or a selected mesh in the material preview; </summary>
        private bool useModelPreview;

        /// <summary> Potential search modes for the Materials Section; </summary>
        private enum MaterialSearchMode {
            /// <summary> Start from a list of meshes and pick from the materials they containt; </summary>
            Mesh,
            /// <summary> Start from a list of materials and pick from a list of meshes that have them assigned; </summary>
            Material
        } /// <summary> Selected distribution of Available Meshes and Materials in the GUI; </summary>
        private static MaterialSearchMode materialSearchMode;

        private static Vector2 topMaterialScroll;
        private static Vector2 leftMaterialScroll;
        private static Vector2 rightMaterialScroll;

        protected override void InitializeData() {
            base.InitializeData();
            Reader.OnModelReimport += UpdateObjectPreview;
        }

        public override void LoadData(string path) {
            LoadInternalMaterialMap();
            useModelPreview = true;
        }

        public override void ResetData() {
            /// Materials Section Dependencies;
            useModelPreview = true;
            selectedMaterial = null;
            if (hasStaticSlotChanges) {
                if (ModalMaterialChanges.ConfirmMaterialChanges()) {
                    AssignMaterialsPersistently();
                } else {
                    ResetSlotChanges();
                } try {
                    GUIUtility.ExitGUI();
                } catch (ExitGUIException) {
                    /// We good :)
                }
            } materialSearchMode = 0;
            CloseMaterialInspectorWindow();
            CloseMaterialHelperWindow();
        }

        /// <summary>
        /// Assigns copies of the material maps in the importer to the static maps in the reader;
        /// </summary>
        private void LoadInternalMaterialMap() {
            OriginalMaterialSlots = MaterialUtils.LoadInternalMaterialMap(Model);
            StaticMaterialSlots = new Dictionary<string, Material>(OriginalMaterialSlots);
        }

        /// <summary>
        /// Override of the Material Replacement method for simple internal use;
        /// </summary>
        /// <param name="key"> Name of the material binding to change; </param>
        /// <param name="newMaterial"> Material to place in the binding; </param>
        public void ReplacePersistentMaterial(string key, Material newMaterial) {
            MaterialUtils.ReplacePersistentMaterial(key, newMaterial, Model);
            StaticMaterialSlots[key] = newMaterial;
            Reader.ReimportModel();
            UpdateSlotChangedStatus();
        }

        /// <summary>
        /// Reverts the serialized references back to their original state;
        /// </summary>
        public void ResetSlotChanges() {
            if (Model == null) return;
            foreach (KeyValuePair<string, Material> kvp in OriginalMaterialSlots) {
                MaterialUtils.ReplacePersistentMaterial(kvp.Key, kvp.Value, Model);
            } StaticMaterialSlots = new Dictionary<string, Material>(OriginalMaterialSlots);
            Reader.ReimportModel();
            hasStaticSlotChanges = false;
        }


        /// <summary>
        /// Assigns the current static dictionary as the persistent material dictionary;
        /// </summary>
        public void AssignMaterialsPersistently() {
            OriginalMaterialSlots = new Dictionary<string, Material>(StaticMaterialSlots);
            hasStaticSlotChanges = false;
        }

        /// <summary>
        /// Compares the current material mapping with the original and decides if they are different;
        /// </summary>
        public void UpdateSlotChangedStatus() {
            if (Model.materialImportMode == 0 || Model.materialLocation == 0) {
                hasStaticSlotChanges = false;
                return;
            }

            foreach (KeyValuePair<string, Material> kvp in StaticMaterialSlots) {
                if (OriginalMaterialSlots[kvp.Key] != kvp.Value) {
                    hasStaticSlotChanges = true;
                    return;
                }
            } hasStaticSlotChanges = false;
        }

        /// <summary>
        /// Set the Material field of the Selected Material;
        /// <br></br> May be called by the Inspector Window to deselect the current material;
        /// </summary>
        /// <param name="material"> Material to showcase and edit; </param>
        private void SetSelectedMaterial(Material material) {
            if (material != null) CloseMaterialInspectorWindow();
            if (selectedMaterial == null) selectedMaterial = new SelectedMaterialProperties(material);
            else selectedMaterial.material = material;
        }

        /// <summary>
        /// Set the GameObject and Renderer fields of the Selected Material;
        /// </summary>
        /// <param name="gameObject"> GameObject showcasing the material; </param>
        /// <param name="renderer"> Renderer holding the showcased mesh; </param>
        public void SetSelectedRenderer(Renderer renderer) {
            Reader.CleanObjectPreview();
            if (selectedMaterial == null) {
                selectedMaterial = new SelectedMaterialProperties(renderer.gameObject, renderer);
            } else if (selectedMaterial.renderer != renderer) {
                selectedMaterial.gameObject = renderer.gameObject;
                selectedMaterial.renderer = renderer;
            } Reader.CreateDummyGameObject(renderer.gameObject);
            SetPreviewToModel(false);
        }

        /// <summary>
        /// Create a Material Editor and show its OnInspectorGUI() layout;
        /// </summary>
        /// <param name="targetMaterial"> Material to show in the Editor; </param>
        private void DrawMaterialInspector(Material targetMaterial) {
            if (MaterialInspectorWindow == null) {
                MaterialInspectorWindow = MaterialInspector.ShowWindow(targetMaterial, MaterialInspectorCallback);
            }
        }

        private void MaterialInspectorCallback(bool closeWindow) {
            if (closeWindow) SetSelectedMaterial(null);
            UpdateObjectPreview();
            MainGUI.Repaint();
        }

        /// <summary>
        /// A method wrapping two other methods often called together to update the object preview;
        /// </summary>
        public void UpdateObjectPreview() {
            Reader.CleanObjectPreview();
            if (!useModelPreview) {
                if (selectedMaterial != null && selectedMaterial.gameObject != null) {
                    Reader.CreateDummyGameObject(selectedMaterial.gameObject);
                } else Debug.LogWarning("Whoops, a preview of the chosen Mesh Object couldn't be created. And it is likely your fault, for the record;");
            }
        }

        private void SetPreviewToModel(bool useModelPreview) {
            this.useModelPreview = useModelPreview;
            UpdateObjectPreview();
        }

        /// <summary>
        /// Close the Material Inspector Window;
        /// </summary>
        private void CloseMaterialInspectorWindow() {
            if (MaterialInspectorWindow != null && EditorWindow.HasOpenInstances<MaterialInspector>()) {
                MaterialInspectorWindow.Close();
            }
        }

        /// <summary>
        /// Close the Material Helper Window;
        /// </summary>
        private void CloseMaterialHelperWindow() {
            if (MaterialHelperWindow is not null && EditorWindow.HasOpenInstances<MaterialHelper>()) {
                MaterialHelperWindow.Close();
            }
        }


            /// <summary> GUI Display for the Materials Section </summary>
        public override void ShowGUI() {

            if (selectedMaterial != null && selectedMaterial.material != null) {
                DrawMaterialInspector(selectedMaterial.material);
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
                            Reader.DrawObjectPreviewEditor(useModelPreview ? Reader.RootPrefab : Reader.DummyGameObject, 
                                                           GUILayout.ExpandWidth(true), GUILayout.Height(192));
                            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                                if (GUILayout.Button(new GUIContent(" Model", EditorUtils.FetchIcon("d_PrefabModel Icon")), useModelPreview
                                                     ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                     GUILayout.Width(100), GUILayout.Height(20))) SetPreviewToModel(true);
                                if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("Refresh")),
                                                         EditorStyles.toolbarButton, GUILayout.Height(20))) UpdateObjectPreview();
                                if (selectedMaterial == null || selectedMaterial.renderer == null) GUI.enabled = false;
                                if (GUILayout.Button(new GUIContent(" Mesh", EditorUtils.FetchIcon("d_MeshRenderer Icon")), useModelPreview
                                                     ? EditorStyles.toolbarButton : UIStyles.SelectedToolbar,
                                                     GUILayout.Width(100), GUILayout.Height(20))) SetPreviewToModel(false);
                                GUI.enabled = true;
                            }
                        }

                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                            GUILayout.Label("Search Mode:", UIStyles.RightAlignedLabel);
                            materialSearchMode = (MaterialSearchMode) EditorGUILayout.EnumPopup(materialSearchMode);
                        }
                        if (selectedMaterial == null || selectedMaterial.renderer == null) GUI.enabled = false;
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                            if (GUILayout.Button("Open In Meshes")) Reader.SwitchToMeshes(selectedMaterial.renderer);
                        } GUI.enabled = true;
                    }

                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(PANEL_WIDTH))) {

                        switch (materialSearchMode) {
                            case MaterialSearchMode.Mesh:
                                DrawMeshSearchArea(0.76f);
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
                                if (hasStaticSlotChanges) {
                                    GUI.color = UIColors.Green;
                                    if (GUILayout.Button("<b>Assign Materials</b>", UIStyles.SquashedButton, GUILayout.MaxWidth(125))) {
                                        AssignMaterialsPersistently();
                                    } GUI.color = UIColors.Red;
                                    if (GUILayout.Button("<b>Revert Materials</b>", UIStyles.SquashedButton, GUILayout.MaxWidth(125))) {
                                        Undo.RecordObject(selectedMaterial.renderer, UNDO_MATERIAL_CHANGE);
                                        ResetSlotChanges();
                                    } GUI.color = Color.white;
                                    GUIContent helperContent = new GUIContent(EditorUtils.FetchIcon("d__Help"));
                                    if (GUILayout.Button(helperContent, GUILayout.MaxWidth(25), GUILayout.MaxHeight(18))) {
                                        MaterialHelperWindow = MaterialHelper.ShowWindow(this);
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
        private void DrawMaterialSearchArea(float scaleMultiplier = 1f) {

            using (new EditorGUILayout.VerticalScope(GUILayout.Width(PANEL_WIDTH), GUILayout.Height(145))) {
                EditorUtils.DrawSeparatorLines("All Materials", true);
                using (var view = new EditorGUILayout.ScrollViewScope(topMaterialScroll, true, false,
                                                              GUI.skin.horizontalScrollbar, GUIStyle.none,
                                                              GUI.skin.box, GUILayout.MaxWidth(PANEL_WIDTH), GUILayout.MaxHeight(110))) {
                    topMaterialScroll = view.scrollPosition;
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(PANEL_WIDTH), GUILayout.Height(110))) {
                        foreach (Material material in Reader.MaterialDict.Keys) DrawMaterialButton(material, scaleMultiplier);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the 'Available Materials' scrollview at the top;
        /// <br></br> Drawn in Mesh Search Mode;
        /// </summary>
        /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
        private void DrawAvailableMaterials(float scaleMultiplier = 1f) {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(PANEL_WIDTH / 2), GUILayout.Height(145))) {
                EditorUtils.DrawSeparatorLines("Available Materials", true);
                if (selectedMaterial != null && selectedMaterial.renderer != null) {
                    using (var view = new EditorGUILayout.ScrollViewScope(leftMaterialScroll, true, false,
                                                          GUI.skin.horizontalScrollbar, GUIStyle.none,
                                                          GUI.skin.box, GUILayout.MaxWidth(PANEL_WIDTH / 2), GUILayout.MaxHeight(110))) {
                        leftMaterialScroll = view.scrollPosition;
                        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(PANEL_WIDTH / 2), GUILayout.Height(110))) {
                            Material[] uniqueMaterials = Reader.GetUniqueMaterials(selectedMaterial.renderer.sharedMaterials);
                            foreach (Material material in uniqueMaterials) {
                                DrawMaterialButton(material, scaleMultiplier);
                            }
                        }
                    }
                } else EditorUtils.DrawScopeCenteredText("No Mesh Selected");
            }
        }

        /// <summary>
        /// Displays a horizontal scrollview with all the meshes available in the model to select from;
        /// </summary>
        /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
        private void DrawMeshSearchArea(float scaleMultiplier = 1f) {

            EditorUtils.DrawSeparatorLines("All Meshes", true);
            using (var view = new EditorGUILayout.ScrollViewScope(topMaterialScroll, true, false,
                                                                  GUI.skin.horizontalScrollbar, GUIStyle.none,
                                                                  GUI.skin.box, GUILayout.MaxWidth(PANEL_WIDTH), GUILayout.MaxHeight(scaleMultiplier == 1 ? 130 : 110))) {
                topMaterialScroll = view.scrollPosition;
                using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(PANEL_WIDTH), GUILayout.MaxHeight(scaleMultiplier == 1 ? 130 : 110))) {
                    foreach (MeshRendererPair mrp in Reader.MeshRenderers) {
                        if (mrp.renderer is SkinnedMeshRenderer) {
                            DrawMeshSelectionButton(mrp.renderer, scaleMultiplier);
                        } else if (mrp.renderer is MeshRenderer) {
                            DrawMeshSelectionButton(mrp.renderer, scaleMultiplier);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the 'Available Meshes' scrollview at the bottom left;
        /// <br></br> Drawn in Material Search Mode;
        /// </summary>
        /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
        private void DrawAvailableMeshes(float scaleMultiplier = 1f) {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(PANEL_WIDTH / 2), GUILayout.Height(145))) {
                EditorUtils.DrawSeparatorLines("Available Meshes", true);
                if (selectedMaterial != null && selectedMaterial.material != null) {
                    using (var view = new EditorGUILayout.ScrollViewScope(leftMaterialScroll, true, false,
                                                                      GUI.skin.horizontalScrollbar, GUIStyle.none,
                                                                      GUI.skin.box, GUILayout.MaxWidth(PANEL_WIDTH / 2), GUILayout.MaxHeight(110))) {
                        leftMaterialScroll = view.scrollPosition;
                        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(PANEL_WIDTH / 2), GUILayout.Height(110))) {
                            foreach (MeshRendererPair mrp in Reader.MaterialDict[selectedMaterial.material]) {
                                if (mrp.renderer is SkinnedMeshRenderer) {
                                    DrawMeshSelectionButton(mrp.renderer, scaleMultiplier);
                                } else if (mrp.renderer is MeshRenderer) {
                                    DrawMeshSelectionButton(mrp.renderer, scaleMultiplier);
                                }
                            }
                        }
                    }
                } else EditorUtils.DrawScopeCenteredText("No Material Selected");
            }
        }

        /// <summary>
        /// Draw a button to select a given submesh;
        /// </summary>
        /// <param name="renderer"> Renderer containing the mesh; </param>
        /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
        private void DrawMeshSelectionButton(Renderer renderer, float scaleMultiplier) {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxWidth(1))) {
                EditorUtils.DrawTexture(Reader.MeshPreviewDict[renderer], 80 * scaleMultiplier, 80 * scaleMultiplier);
                if (selectedMaterial != null && selectedMaterial.renderer == renderer) {
                    GUILayout.Label("Selected", UIStyles.CenteredLabelBold, GUILayout.MaxWidth(80 * scaleMultiplier), GUILayout.MaxHeight(20 * scaleMultiplier));
                } else if (GUILayout.Button("Open", GUILayout.MaxWidth(80 * scaleMultiplier))) SetSelectedRenderer(renderer);
            }
        }

        /// <summary>
        /// Draws the 'Material Slots' scrollview at the bottom right;
        /// <br></br> Drawn for all search modes;
        /// </summary>
        private void DrawMaterialSlots() {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(PANEL_WIDTH / 2), GUILayout.Height(145))) {
                EditorUtils.DrawSeparatorLines("Material Slots", true);
                using (var view = new EditorGUILayout.ScrollViewScope(rightMaterialScroll, true, false,
                                                                    GUI.skin.horizontalScrollbar, GUIStyle.none,
                                                                    GUI.skin.box, GUILayout.MaxWidth(PANEL_WIDTH / 2), GUILayout.MaxHeight(110))) {
                    rightMaterialScroll = view.scrollPosition;
                    using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(PANEL_WIDTH / 2), GUILayout.MaxHeight(110))) {
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
            }
        }

        /// <summary>
        /// Draws a button for a selectable material;
        /// </summary>
        /// <param name="material"> Material to draw the button for; </param>
        /// <param name="scaleMultiplier"> A lazy scale multiplier; </param>
        private void DrawMaterialButton(Material material, float scaleMultiplier = 1f) {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxWidth(1))) {
                EditorUtils.DrawTexture(AssetPreview.GetAssetPreview(material), 80 * scaleMultiplier, 80 * scaleMultiplier);
                if (selectedMaterial != null && selectedMaterial.material == material) {
                    GUILayout.Label("Selected", UIStyles.CenteredLabelBold, GUILayout.MaxWidth(80 * scaleMultiplier), GUILayout.MaxHeight(20 * scaleMultiplier));
                } else if (GUILayout.Button("Open", GUILayout.MaxWidth(80 * scaleMultiplier))) {
                    SetSelectedMaterial(material);
                }
            }
        }
    }
}