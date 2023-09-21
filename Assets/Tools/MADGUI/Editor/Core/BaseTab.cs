using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;
using ModelAssetDatabase.MADShaderUtility;

namespace ModelAssetDatabase {

    /// <summary>
    /// A base class for all database tool tabs;
    /// </summary>
    public abstract class BaseTab : ScriptableObject {

        /// <summary> Every tab will be managed by a parent tool, and will have a handy reference to it; </summary>
        protected BaseTool Tool;

        protected const string INVALID_MANAGER = "You attempted to create a new tab without a proper manager to handle it! The tab was not instantiated;";

        /// <summary>
        /// Initialize base tab data when constructing the tab;
        /// </summary>
        public static T CreateTab<T>(BaseTool tool) where T : BaseTab {
            var tab = CreateInstance<T>();
            tab.Tool = tool;
            tab.InitializeData();
            return tab;
        }

        /// <summary>
        /// Override this method to implement a custom initialization when the tab is created;
        /// </summary>
        protected abstract void InitializeData();

        /// <summary>
        /// Override to load the data corresponding to a path, usually on asset creation;
        /// This method may also be used by the Hierarchy for GUI purposes;
        /// </summary>
        public virtual void LoadData(string path) { }

        /// <summary>
        /// Reset tab dependent data when abandoning the tab;
        /// </summary>
        public virtual void ResetData() { }
        void OnDisable() => ResetData();

        /// <summary>
        /// Override to display Custom GUI controls in the Tab;
        /// </summary>
        public virtual void ShowGUI() => EditorUtils.DrawScopeCenteredText("No GUI has been implemented for this tab;");
    }

    /// <summary>
    /// Base class for all tabs managed by the Material Manager Tool;
    /// </summary>
    public abstract class MaterialTab : BaseTab {

        /// <summary> The Material Manager parent tool of this tab; </summary>
        protected MaterialManager MaterialManager;

        protected override void InitializeData() {
            if (Tool is MaterialManager) {
                MaterialManager = Tool as MaterialManager;
            } else Debug.LogError(INVALID_MANAGER);
        }

        /// <summary>
        /// Override to choose how the tab reacts to the hierarchy builder;
        /// </summary>
        public virtual void SetSelectedAsset(string path) {
            Debug.LogWarning("You shouldn't be able to select this, it's not even implemented >.>");
        }
    }

    /// <summary>
    /// A subset of material tabs that requires specific material manipulation;
    /// </summary>
    public abstract class MaterialModifierTab : MaterialTab {
        public class ManagedMaterialData {
            public string path;
            public Material material;

            public ManagedMaterialData(string path, Material material) {
                this.path = path;
                this.material = material;
            }
        }
        protected ManagedMaterialData managedMaterial;

        protected MaterialEditorBundle materialEditor;

        /// <summary>
        /// Creates a material editor if one is not available;
        /// </summary>
        protected void ExtractMaterialEditor() => materialEditor = MaterialEditorBundle.CreateBundle(managedMaterial.material);

        /// <summary>
        /// A shorthand for drawing the extracted editor;
        /// </summary>
        protected void DrawMaterialEditor() => materialEditor.DrawEditor();

        /// <summary>
        /// Clean the material editor;
        /// </summary>
        protected void CleanMaterialEditor() => DestroyImmediate(materialEditor);

        /// <summary>
        /// A method group to use on the Shader Popups and replace a target material's shader;
        /// </summary>
        /// <param name="shader"> Shader obtained from the Shader Popup utilities; </param>
        public void ReplaceMaterialShader(Shader shader) {
            if (managedMaterial != null && materialEditor != null) {
                if (managedMaterial.material.shader == shader) return;
                materialEditor.editor.SetShader(shader);
            } else Debug.LogWarning("Shader could not be set;");
        }
    }

    public class MaterialTabEditor : MaterialModifierTab {

        private Vector2 editorScroll;

        public override void ResetData() {
            CleanMaterialEditor();
            MaterialManager.CleanPreview();
        }

        /// <summary>
        /// Set the currently edited material data;
        /// </summary>
        /// <param name="path"> Path of the Material to read; </param>
        public override void SetSelectedAsset(string path) {
            ResetData();
            MaterialManager.SetPreviewTarget(MaterialManager.PreviewTarget.Sphere);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            managedMaterial = new ManagedMaterialData(path, material);
        }

        public override void ShowGUI() {
            if (managedMaterial == null) {
                EditorUtils.DrawScopeCenteredText("Select a Material from the Hierarchy to begin;");
            } else {
                float panelWidth = 620;
                if (materialEditor == null) ExtractMaterialEditor();
                using (new EditorGUILayout.HorizontalScope()) {
                    /// Editor side of the Editor Tab;
                    using (new EditorGUILayout.VerticalScope(GUILayout.Width(panelWidth / 2))) {
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            using (new EditorGUILayout.VerticalScope()) {
                                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                                        GUILayout.Label("Material Editor", UIStyles.CenteredLabelBold);
                                    }
                                } using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                    GUILayout.Label("Shader:");
                                    Rect shaderPosition = EditorGUILayout.GetControlRect(GUILayout.MinWidth(105));
                                    GUIContent shaderContent = new GUIContent(managedMaterial.material.shader == null
                                                                              ? "Missing Shader" : managedMaterial.material.shader.name);
                                    MADShaderUtils.DrawDefaultShaderPopup(shaderPosition, shaderContent, ReplaceMaterialShader);
                                    Rect historyPosition = EditorGUILayout.GetControlRect(GUILayout.Width(38));
                                    MADShaderUtils.DrawShaderHistoryPopup(historyPosition, ReplaceMaterialShader);
                                } using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                    using (var view = new EditorGUILayout.ScrollViewScope(editorScroll)) {
                                        editorScroll = view.scrollPosition;
                                        DrawMaterialEditor();
                                    }
                                }
                            }
                        }
                    } /// Preview side of the Editor Tab;
                    using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.Width(panelWidth / 2))) {
                        EditorUtils.WindowBoxLabel("Material Preview");
                        MaterialManager.DrawMaterialPreview();
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            MaterialManager.DrawMaterialPreviewOptions();
                        }
                    }
                }
            }
        }
    }

    public class MaterialTabCreator : MaterialModifierTab {

    }

    public class MaterialTabOrganizer : MaterialTab {

    }

    public class MaterialTabReplacer : MaterialTab {

    }
}