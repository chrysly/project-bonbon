using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CJUtils;
using ModelAssetDatabase.MADUtils;

namespace ModelAssetDatabase {

    /// <summary> Component class of the Model Asset Database;
    /// <br></br> Reads asset data and displays the corresponding properties in the GUI; </summary>
    public class Reader : BaseTool {

        #region | Variables |

        #region | Tab Variables |

        /// <summary> Array containing all reader tabs; </summary>
        private ReaderTab[] tabs;

        /// <summary> Component Tools separated by General purpose; </summary>
        public enum SectionType {
            Model = 0,
            Meshes = 1,
            Materials = 2,
            Prefabs = 3,
            Animations = 4,
        } /// <summary> Section currently selected in the GUI; </summary>
        private SectionType ActiveSection;

        /// <summary> Potential Model content limitations; </summary>
        public enum AssetMode {
            Model,
            Animation
        } /// <summary> Asset Mode currently selected in the GUI; </summary>
        private AssetMode ActiveAssetMode;

        #endregion

        #region | Model & Tool Data |

        public System.Action OnModelReimport;

        /// <summary> Reference to the model importer file; </summary>
        public ModelImporter Model { get; private set; }
        /// <summary> GUID of the currently selected model; </summary>
        public string ModelID { get; private set; }
        /// <summary> Ext Data of the selected mode; </summary>
        public ExtData ModelExtData { get; private set; }
        /// <summary> Reference to the prefab, if any, contained in the model; </summary>
        public GameObject RootPrefab { get; private set; }
        /// <summary> Reference to the Custom Icons Scriptable Object; </summary>
        public MADAssets CustomTextures { get; private set; }

        /// <summary> Mesh preview dictionary; </summary>
        public Dictionary<Renderer, Texture2D> MeshPreviewDict { get; private set; }

        /// <summary> The sum of all the vertices in a composite model; </summary>
        public int GlobalVertexCount;
        /// <summary> The sum of all the triangles in a composite model; </summary>
        public int GlobalTriangleCount;

        /// <summary> Struct to store renderers with filters and avoid unnecesary GetComponent() calls; 
        /// <br></br> I can use a tuple here, ik. But I forget which one is which ;-;
        /// </summary>
        public struct MeshRendererPair {
            public MeshFilter filter;
            public Renderer renderer;
            public MeshRendererPair(MeshFilter filter, Renderer renderer) {
                this.filter = filter;
                this.renderer = renderer;
            }
            public override bool Equals(object obj) {
                if (obj is MeshRendererPair) {
                    MeshRendererPair mrp = (MeshRendererPair) obj;
                    return mrp.filter == filter && mrp.renderer == renderer;
                } return false;
            }
            public override int GetHashCode() {
                return System.HashCode.Combine(filter, renderer);
            }
        } /// <summary> List of all the mesh renderers and mesh filters contained in the model </summary>
        public List<MeshRendererPair> MeshRenderers { get; private set; }

        /// <summary> A disposable Editor class embedded in the Editor Window to show a preview of an instantiable asset; </summary>
        private GenericPreview objectPreview;

        /// <summary> Dictionary mapping each material to the renderers it is available in; </summary>
        public Dictionary<Material, List<MeshRendererPair>> MaterialDict { get; private set; }

        /// <summary> Found myself using this number a lot. Right side width; </summary>
        public const float PANEL_WIDTH = 440;

        #endregion

        #endregion

        #region | Initialization & Cleanup |

        protected override void InitializeData() {
            tabs = new ReaderTab[] { 
                BaseTab.CreateTab<ReaderTabModel>(this),
                BaseTab.CreateTab<ReaderTabMeshes>(this),
                BaseTab.CreateTab<ReaderTabMaterials>(this),
                BaseTab.CreateTab<ReaderTabPrefabs>(this),
                BaseTab.CreateTab<ReaderTabAnimations>(this),
            };

            if (CustomTextures == null) CustomTextures = ConfigurationCore.ToolAssets;
            /// Materials Section Variables;
            MaterialDict = new Dictionary<Material, List<MeshRendererPair>>();
            /// Meshes Section Variables;
            MeshRenderers = new List<MeshRendererPair>();
        }

        /// <summary>
        /// Resets variables whose contents depend on a specific section;
        /// </summary>
        public override void ResetData() {
            CleanObjectPreview();
            tabs[(int) ActiveSection].ResetData();
            RefreshSections();
        }

        /// <summary>
        /// Discard any read information;
        /// <br></br> Required to load new information without generating persistent garbage;
        /// </summary>
        public override void FlushData() {
            CleanMeshPreviewDictionary();
            ResetData();
            DestroyImmediate(DummyGameObject);
            //Undo.undoRedoPerformed -= UpdateSlotChangedStatus;
        }

        /// <summary>
        /// Checks if the model reference is available;
        /// </summary>
        /// <returns> True if the model reference is null, false otherwise; </returns>
        private bool ReferencesAreFlushed() => Model == null;

        #endregion

        #region | Selection Methods |

        /// <summary>
        /// Set the currently selected asset;
        /// </summary>
        /// <param name="path"> Path to the selected asset; </param>
        public override void SetSelectedAsset(string path) {
            FlushData();
            LoadSelectedAsset(path);
            SetSelectedAssetMode(ModelExtData.isModel ? AssetMode.Model : AssetMode.Animation);
        }

        /// <summary>
        /// Change the Toolbar to deal with a different type of Model content;
        /// </summary>
        /// <param name="newAssetMode"> New model type to atune the toolbar to; </param>
        public void SetSelectedAssetMode(AssetMode newAssetMode) {
            switch (newAssetMode) {
                case AssetMode.Model:
                    SetSelectedSection(SectionType.Model);
                    break;
                case AssetMode.Animation:
                    SetSelectedSection(SectionType.Animations);
                    break;
            } ActiveAssetMode = newAssetMode;
        }

        /// <summary>
        /// Sets the GUI's selected Reader Section;
        /// </summary>
        /// <param name="sectionType"> Type of the prospective section to show; </param>
        public void SetSelectedSection(SectionType sectionType) {
            if (ActiveSection != sectionType) {
                ResetData();
                ActiveSection = sectionType;
            }
        }

        #endregion

        #region | Loading Helpers |

        /// <summary>
        /// Reimports the current Model and updates the Reader accordingly;
        /// </summary>
        public void ReimportModel() {
            Model.SaveAndReimport();
            OnModelReimport?.Invoke();
            UpdateMeshAndMaterialProperties();
        }

        /// <summary>
        /// Assign a reference to the Model importer at the designated path and load corresponding references;
        /// </summary>
        /// <param name="path"> Path to the model to read; </param>
        private void LoadSelectedAsset(string path) {
            Model = AssetImporter.GetAtPath(path) as ModelImporter;
            ModelID = AssetDatabase.AssetPathToGUID(Model.assetPath);
            ModelExtData = ModelID != null ? ModelAssetDatabase.ModelDataDict[ModelID].extData : null;
            RootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            foreach (BaseTab tab in tabs) tab.LoadData(path);
            UpdateMeshAndMaterialProperties();
            //Undo.undoRedoPerformed += UpdateSlotChangedStatus;
        }

        /// <summary>
        /// Reads all 'accesible' mesh and material data from a model;
        /// </summary>
        public void UpdateMeshAndMaterialProperties() {
            CleanMeshPreviewDictionary();
            MeshPreviewDict = new Dictionary<Renderer, Texture2D>();
            MeshRenderers = new List<MeshRendererPair>();
            MeshFilter[] mfs = RootPrefab.GetComponentsInChildren<MeshFilter>();
            SkinnedMeshRenderer[] smrs = RootPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();

            Mesh dummyMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            MeshPreview mp = new MeshPreview(dummyMesh);
            Resources.UnloadAsset(dummyMesh);
            foreach (MeshFilter mf in mfs) {
                MeshRenderer mr = mf.GetComponent<MeshRenderer>();
                MeshRenderers.Add(new MeshRendererPair(mf, mr));
                var sharedMesh = mf.sharedMesh;
                GlobalVertexCount += sharedMesh.vertexCount;
                GlobalTriangleCount += sharedMesh.triangles.Length;
                mp.mesh = sharedMesh;
                Texture2D previewTexture = mp.RenderStaticPreview(200, 200);
                previewTexture.hideFlags = HideFlags.HideAndDontSave;
                MeshPreviewDict[mr] = previewTexture;
                AssignAllMaterialsInRenderer(mr);
            } foreach (SkinnedMeshRenderer smr in smrs) {
                MeshRenderers.Add(new MeshRendererPair(null, smr));
                var sharedMesh = smr.sharedMesh;
                GlobalVertexCount = sharedMesh.vertexCount;
                GlobalTriangleCount += sharedMesh.triangles.Length;
                mp.mesh = sharedMesh;
                Texture2D previewTexture = mp.RenderStaticPreview(200, 200);
                previewTexture.hideFlags = HideFlags.HideAndDontSave;
                MeshPreviewDict[smr] = previewTexture;
                AssignAllMaterialsInRenderer(smr);
            } mp.Dispose();
        }

        /// <summary>
        /// Add all the unique material assets on a Mesh Renderer to the Material Dictionary;
        /// </summary>
        /// <param name="renderer"> Renderer to get the materials from; </param>
        private void AssignAllMaterialsInRenderer(Renderer renderer) {
            foreach (Material material in renderer.sharedMaterials) {
                if (material == null) continue;
                var mrp = GetMRP(renderer);
                if (!MaterialDict.ContainsKey(material)) MaterialDict[material] = new List<MeshRendererPair>();
                var res = MaterialDict[material].Find((pair) => pair.renderer.gameObject == mrp.renderer.gameObject);
                if (res.Equals(default(MeshRendererPair))) MaterialDict[material].Add(mrp);
            }
        }

        /// <summary>
        /// Create a MeshRendererPair based on the type of renderer passed;
        /// </summary>
        /// <param name="renderer"> Mesh Renderer which must be turned into a Mesh Renderer Pair; </param>
        /// <returns> Adequate MeshRendererPair for the renderer passed; </returns>
        private MeshRendererPair GetMRP(Renderer renderer) {
            if (renderer is SkinnedMeshRenderer) {
                return new MeshRendererPair(null, renderer);
            } else {
                return new MeshRendererPair(renderer.GetComponent<MeshFilter>(), renderer);
            }
        }

        /// <summary>
        /// Takes an array of materials and removes all duplicates;
        /// </summary>
        /// <param name="materials"> Array to process; </param>
        /// <returns> Array of unique material assets; </returns>
        public Material[] GetUniqueMaterials(Material[] materials) {
            List<Material> materialList = new List<Material>();
            foreach (Material material in materials) {
                if (!materialList.Contains(material)) materialList.Add(material);
            } return materialList.ToArray();
        }

        #endregion

        #region | Preview Helpers |

        /// <summary> Disposable GameObject instantiated to showcase GUI changes non-invasively; </summary>
        public GameObject DummyGameObject { get; private set; }

        /// <summary>
        /// Creates a dummy GameObject to showcase changes without changing the model prefab;
        /// </summary>
        /// <param name="gameObject"> GameObject to reproduce; </param>
        public void CreateDummyGameObject(GameObject gameObject) {
            if (DummyGameObject) DestroyImmediate(DummyGameObject);
            DummyGameObject = Instantiate(gameObject);
        }

        /// <summary>
        /// An internal overload of the Object Preview method that cleans up the Dummy Object when needed;
        /// </summary>
        /// <param name="gameObject"> GameObject to show in the Preview; </param>
        /// <param name="width"> Width of the Preview's Rect; </param>
        /// <param name="height"> Height of the Preview's Rect; </param>
        public void DrawObjectPreviewEditor(GameObject gameObject, params GUILayoutOption[] options) {
            if (objectPreview == null) {
                objectPreview = GenericPreview.CreatePreview(gameObject);
            } objectPreview.DrawPreview(options);
            DestroyImmediate(DummyGameObject);
        }

        /// <summary>
        /// Clean up all textures stored in the preview dictionary, as well as the dictionary itself;
        /// <br></br> According to Unity's Memory Profiler, this is safe... probably;
        /// </summary>
        private void CleanMeshPreviewDictionary() {
            if (MeshPreviewDict == null) return;
            foreach (KeyValuePair<Renderer, Texture2D> kvp in MeshPreviewDict) {
                if (kvp.Value != null) DestroyImmediate(kvp.Value);
            } MeshPreviewDict = null;
        }

        /// <summary>
        /// Dispose of the current Object Preview Editor;
        /// <br></br> May be called by the Material Inspector to update the Preview;
        /// </summary>
        public void CleanObjectPreview() => DestroyImmediate(objectPreview);

        #endregion

        #region | Tool GUI |

        /// <summary>
        /// Call the appropriate section function based on GUI Selection;
        /// </summary>
        public override void ShowGUI() {
            if (ReferencesAreFlushed()) {
                EditorUtils.DrawScopeCenteredText("Selected Asset Data will be displayed here;");
            } else tabs[(int) ActiveSection].ShowGUI();
        }

        /// <summary>
        /// Refresh the scrolling variables and reset all section dependent variables;
        /// </summary>
        private void RefreshSections() {
            MainGUI.SetHighRepaintFrequency(false);
        }

        /// <summary>
        /// Draws the toolbar for the Model Reader;
        /// </summary>
        public override void DrawToolbar() {
            GUI.enabled = Model != null;
            switch (ActiveAssetMode) {
                case AssetMode.Model:
                    DrawToolbarButton(SectionType.Model, 72, 245);
                    DrawToolbarButton(SectionType.Meshes, 72, 245);
                    DrawToolbarButton(SectionType.Materials, 72, 245);
                    DrawToolbarButton(SectionType.Prefabs, 112, 245);
                    break;
                case AssetMode.Animation:
                    DrawToolbarButton(SectionType.Animations, 328, 980);
                    break;
            } GUILayout.FlexibleSpace();
            if (ModelExtData != null && !ModelExtData.isModel) GUI.enabled = false;
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
        private void DrawToolbarButton(SectionType sectionType, float minWidth, float maxWidth) {
            GUIStyle buttonStyle = sectionType == ActiveSection ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton;
            if (GUILayout.Button(System.Enum.GetName(typeof(SectionType), sectionType),
                                 buttonStyle, GUILayout.MinWidth(minWidth), GUILayout.MaxWidth(maxWidth))) {
                SetSelectedSection(sectionType);
            }
        }

        #endregion

        #region | Inter-Tab Bridge |

            /// <summary>
        /// A button to switch to the Material section with the current mesh selected;
        /// </summary>
        /// <param name="renderer"> The renderer to keep through the section change; </param>
        public void SwitchToMaterials(Renderer renderer) {
            SetSelectedSection(SectionType.Materials);
            var materialsTab = tabs[(int) SectionType.Materials] as ReaderTabMaterials;
            materialsTab.SetSelectedRenderer(renderer);
        }

        /// <summary>
        /// Open the currently selected Mesh in the Meshes tab;
        /// </summary>
        /// <param name="renderer"> Renderer holding the mesh; </param>
        public void SwitchToMeshes(Renderer renderer) {
            SetSelectedSection(SectionType.Meshes);
            MeshRendererPair mrp = GetMRP(renderer);
            Mesh mesh = null;
            if (renderer is SkinnedMeshRenderer) {
                mesh = (mrp.renderer as SkinnedMeshRenderer).sharedMesh;
            } else if (renderer is MeshRenderer) {
                mesh = mrp.filter.sharedMesh;
            } var meshesTab = tabs[(int) SectionType.Meshes] as ReaderTabMeshes;
            meshesTab.SetSelectedMesh(mesh, renderer);
        }

        #endregion
    }
}