using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;
using ModelAssetDatabase.MADShaderUtility;

namespace ModelAssetDatabase {

    /// <summary> Component class of the Model Asset Database;
    /// <br></br> Allows for the edition, creation, and general manipulation of Material Assets; </summary>
    public class MaterialManager : BaseTool {

        private MaterialTab[] tabs;

        /// <summary> Distinct tabs separating Manager Functions; </summary>
        public enum SectionType {
            Editor = 0,
            Creator = 1,
            Organizer = 2,
            Replacer = 3
        } /// <summary> Section currently selected in the GUI; </summary>
        public SectionType ActiveSection { get; private set; }

        private GameObject previewTarget;
        private GenericPreview preview;

        private MADAssets customPrefabs;

        // Remove
        private static bool abbreviateEditMode;

        #region | Initialization & Cleanup |

        protected override void InitializeData() {
            if (customPrefabs == null) customPrefabs = ConfigurationCore.ToolAssets;

            tabs = new MaterialTab[] {
                BaseTab.CreateTab<MaterialTabEditor>(this),
                BaseTab.CreateTab<MaterialTabCreator>(this),
                BaseTab.CreateTab<MaterialTabOrganizer>(this),
                BaseTab.CreateTab<MaterialTabReplacer>(this),
            };
        }

        public override void ResetData() => tabs[(int) ActiveSection].ResetData();
        public override void FlushData() => CleanPreviewTarget();

        /// <summary>
        /// Change the selected asset in the active tab;
        /// </summary>
        /// <param name="path"> Path to the selected asset; </param>
        public override void SetSelectedAsset(string path) => tabs[(int) ActiveSection].SetSelectedAsset(path);

        /// <summary>
        /// Sets the GUI's selected Manager Section;
        /// </summary>
        /// <param name="sectionType"> Type of the prospective section to show; </param>
        private void SetSelectedSection(SectionType sectionType) {
            ActiveSection = sectionType;
        }

        #endregion

        #region | Tool GUI |

        /// <summary>
        /// Draws the toolbar for the Material Manager;
        /// </summary>
        public override void DrawToolbar() {
            foreach (SectionType sectionType in System.Enum.GetValues(typeof(SectionType))) {
                DrawMaterialToolbarButton(sectionType);
            }
        }

        /// <summary>
        /// Draws a button on the Material Toolbar;
        /// </summary>
        /// <param name="section"> Section to draw the button for; </param>
        private void DrawMaterialToolbarButton(SectionType section) {
            if (GUILayout.Button(System.Enum.GetName(typeof(SectionType), section), ActiveSection == section
                                           ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton, GUILayout.MinWidth(140), GUILayout.ExpandWidth(true))) {
                SetSelectedSection(section);
            }
        }

        /// <summary>
        /// Select a GUI display based on the currently active section;
        /// </summary>
        public override void ShowGUI() => tabs[(int) ActiveSection].ShowGUI();

        #endregion

        #region | Preview Helpers |

        public void DrawMaterialPreview() {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (preview == null) {
                if (previewTarget != null) {
                    preview = GenericPreview.CreatePreview(previewTarget);
                } else EditorUtils.DrawScopeCenteredText("Select a Preview Object");
            } else {
                preview.preview.DrawPreview(rect);
                CleanPreviewTarget();
            }
        }

        public enum PreviewTarget {
            Sphere,
            Cube,
            Other
        } private PreviewTarget activeTarget;

        /// <summary>
        /// Draw an Enum Popup to choose the preview target;
        /// </summary>
        public void DrawMaterialPreviewOptions() {
            GUILayout.Label("Preview Object:");
            PreviewTarget selection = (PreviewTarget) EditorGUILayout.EnumPopup(activeTarget);
            if (activeTarget != selection) SetPreviewTarget(selection);
            /// If an object must be selected for preview, wait until the object is assigned;
            if (activeTarget == PreviewTarget.Other) {
                GameObject potentialObject = EditorGUILayout.ObjectField(previewTarget, typeof(GameObject), false) as GameObject;
                if (potentialObject != previewTarget) SetPreviewObject(potentialObject);
            }
        }

        /// <summary>
        /// Define the target preview object and call for the object creation;
        /// </summary>
        /// <param name="selection"> Selected target preview; </param>
        public void SetPreviewTarget(PreviewTarget selection) {
            CleanPreview();
            previewTarget = null;
            switch (selection) {
                case PreviewTarget.Sphere:
                    SetPreviewObject(customPrefabs.spherePrefab);
                    break;
                case PreviewTarget.Cube:
                    SetPreviewObject(customPrefabs.cubePrefab);
                    break;
            } activeTarget = selection;
        }

        /// <summary>
        /// Create a preview object whose materials slots are replaced by the target material;
        /// </summary>
        /// <param name="gameObject"> Object to preview; </param>
        /// <param name="material"> Material to place in the material slots of the preview object; </param>
        private void SetPreviewObject(GameObject gameObject, Material material = null) {
            previewTarget = Instantiate(gameObject);
            Renderer[] renderers = previewTarget.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers) {
                Material[] nArr = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
                    //nArr[i] = material;
                } //renderer.sharedMaterials = nArr;
            } CleanPreview();
        }

        /// <summary>
        /// Destroy the active Material Preview, if any;
        /// </summary>
        public void CleanPreview() => DestroyImmediate(preview);

        /// <summary>
        /// Destroy the active Preview Object, if any;
        /// </summary>
        private void CleanPreviewTarget() => DestroyImmediate(previewTarget);

        #endregion

        private void ShowCreatorSection() {
            //if (null == null) {
                EditorUtils.DrawScopeCenteredText("Select a Material from the Hierarchy to begin;");
            /*} else {
                using (new EditorGUILayout.HorizontalScope()) {
                    /// Editor side of the Editor Tab;
                    using (new EditorGUILayout.VerticalScope()) {
                        /// Editing mode selection box: Abbreviated/Built-in;
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            GUILayout.Label("Editing Mode:");
                            if (GUILayout.Button("Abbreviated")) {
                                abbreviateEditMode = true;
                            } if (GUILayout.Button("Built-In")) {
                                abbreviateEditMode = false;
                            }
                        }  /// Selected Editing Mode;
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            using (new EditorGUILayout.VerticalScope()) {
                                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                                        GUILayout.Label("Material Editor", UIStyles.CenteredLabelBold);
                                    }
                                } if (abbreviateEditMode) {
                                    /// Shader Selection
                                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                        GUILayout.Label("Shader:");
                                        /// Shader Popup Function;
                                        /// Shader History Function:
                                        GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_UnityEditor.AnimationWindow")));
                                    } /// Applied Fields (Based on available fields, the default are those whose keywords are enabled);
                                    using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                        using (new EditorGUILayout.ScrollViewScope(Vector2.zero)) {

                                        }
                                    } /// Field Selector;
                                    using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                        using (new EditorGUILayout.ScrollViewScope(Vector2.zero)) {

                                        }
                                    }
                                } else {
                                    /// Use Assembly to extract material editor;
                                }
                            }
                        }
                    } /// Preview side of the Editor Tab;
                    using (new EditorGUILayout.VerticalScope()) {
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            GUILayout.Label("Material Preview", UIStyles.CenteredLabelBold);
                        }
                    }
                }
            }*/
        }

        private void ShowOrganizerSection() {

        }

        private void ShowReplacerSection() {

        }
    }
}