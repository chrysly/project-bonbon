using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;
using ModelAssetDatabase.MADShaderUtility;

namespace ModelAssetDatabase {

    /// <summary>
    /// Auxiliary Window for quick manipulation of Model Assets;
    /// </summary>
    public class AssetPreprocessorGUI : EditorWindow {

        #region | Core |

        #region | Core Import Data |

        /// <summary> References and values that govern the reimport process; </summary>
        public class ImportOverrideOptions {
            /// <summary> Reference to the model to be reimported; </summary>
            public ModelImporter model;
            /// <summary> GUID of the model to be reimported; </summary>
            public string modelID;
            /// <summary> Whether any meshes were found in the imported file; </summary>
            public bool hasMeshes;
            /// <summary> Whether the model should use materials or vertex color; </summary>
            public bool hasVertexColor;
            /// <summary> Way to replace materials in the model, if the model uses materials; </summary>
            public MaterialOverrideMode materialOverrideMode;
            /// <summary> Whether the material override should use a global shader for all new materials; </summary>
            public bool useSingleShader = true;
            /// <summary> Global shader used for new materials if instructed; </summary>
            public Shader shader;
            /// <summary> Category folder to relocate the model to, if any; </summary>
            public int folder;
            /// <summary> Relocate prefabs associated with the model; </summary>
            public bool relocatePrefabs;
            /// <summary> Relocate materials; </summary>
            public bool relocateMaterials;
        } /// <summary> Global Reimport values used to allocate the GUI and the final reimport; </summary>
        public ImportOverrideOptions Options { get; set; }

        /// <summary> An array of paths to pick from for the Model Relocation method; </summary>
        public string[] FolderPaths { get; private set; }

        #endregion

        #region | Material Override Data |

        /// <summary> Different ways to override the materials in the model, if any; </summary>
        public enum MaterialOverrideMode { None, Single, Multiple }

        /// <summary> Data used to reference and/or create materials in the Reimport Window; </summary>
        public class MaterialData {
            /// <summary> Whether the slot is being remapped or a new material is being created; </summary>
            public bool isNew;
            /// <summary> Reference to an external, (pre-existing or newly created) material; </summary>
            public Material materialRef;
            /// <summary> Name of the new material, which must abide by the project's convetion; </summary>
            public string name;
            /// <summary> Main texture used to create the material (required); </summary>
            public Texture2D albedoMap;
            /// <summary> Normal map used to create the material; </summary>
            public Texture2D normalMap;
            /// <summary> Shader to base the material on (required if not using a global shader); </summary>
            public Shader shader;
        } /// <summary> Map of identifier keys to the Material Data that has been generated on them; </summary>
        public Dictionary<string, MaterialData> MaterialOverrideMap { get; set; }

        /// <summary> External key reserved for the Single Material Override Mode; </summary>
        public string SingleKey { get { return ""; } private set { SingleKey = value; } }

        /// <summary> Map of identifier keys to newly generated materials; </summary>
        public Dictionary<string, Material> TempMaterialMap { get; private set; }

        /// <summary> Map of identifier keys to New Materials that will ultimately remain in use; </summary>
        public Dictionary<string, Material> PreservedMaterialMap { get; private set; }

        /// <summary> Original material map used by the Model to reimport; </summary>
        private Dictionary<string, Material> originalInternalMap;

        public GenericPreview preview;

        #endregion

        #region | Core Import Functions |

        /// <summary>
        /// Creates an Import Override Options object with publish-ready default values;
        /// </summary>
        /// <param name="modelImporter"> Model to create the Import Options object for; </param>
        public void LoadBasicOptions(ModelImporter modelImporter) {
            Options = new ImportOverrideOptions();
            Options.model = modelImporter;
            Options.modelID = AssetDatabase.AssetPathToGUID(Options.model.assetPath);
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(Options.model.assetPath);
            Options.hasMeshes = mesh != null;
            Options.hasVertexColor = Options.hasMeshes ? mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Color) : false;
            Options.folder = 0;
            FetchFolderPaths();
        }

        /// <summary>
        /// Builds a category name map using the Hierarchy Builder;
        /// <br></br> The implementation is too limited, so I'd really like to change it in the future;
        /// </summary>
        private void FetchFolderPaths() {
            var folderMap = ModelAssetDatabase.BuildFolderMap(ModelAssetDatabase.RootAssetPath);
            FolderPaths = new string[folderMap.Keys.Count + 1];
            int i = 1;
            foreach (KeyValuePair<string, ModelAssetDatabase.FolderData> kvp in folderMap) {
                FolderPaths[i] = kvp.Key;
                i++;
            } FolderPaths[0] = "None";
        }

        /// <summary>
        /// Mark the asset as reimported;
        /// </summary>
        public void SignExtData() {
            ExtManager.Refresh();
            ExtData extData = ExtManager.CreateExtData(Options.modelID);
            extData.isReimported = true;
        }

        /// <summary>
        /// Moves a model asset and the selected dependencies to a given folder;
        /// </summary>
        public void RelocateModelAsset() {
            if (Options.folder != 0) {
                string oldPath = Options.model.assetPath;
                string newPath = FolderPaths[Options.folder] + "/" + oldPath.IsolatePathEnd("\\/");
                string moveMessage = AssetDatabase.ValidateMoveAsset(oldPath, newPath);
                if (string.IsNullOrEmpty(moveMessage)) {
                    AssetDatabase.MoveAsset(Options.model.assetPath, newPath);
                } else Debug.LogWarning(moveMessage);
            } if (Options.relocatePrefabs) ModelAssetDatabase.RelocatePrefabsWithModel(Options.modelID);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Apply the Import Overrides designated in the GUI;
        /// </summary>
        public void ReimportAsset() {
            RelocateModelAsset();
            foreach (KeyValuePair<string, Material> kvp in PreservedMaterialMap) {
                TempMaterialMap.Remove(kvp.Key);
                TempMaterialManager.ReleaseMaterial(kvp.Value);
            } SignExtData();
            Options.model.SaveAndReimport();
            FlushImportData(false);
            Options = null;
        }

        /// <summary>
        /// Flushes all data held by this static class;
        /// </summary>
        public void FlushImportData(bool restore = true) {
            if (TempMaterialMap != null) {
                foreach (KeyValuePair<string, Material> kvp in TempMaterialMap) {
                    if (kvp.Value != null) Object.DestroyImmediate(kvp.Value, true);
                } TempMaterialManager.CleanAllMaterials();
            } if (restore) ReplaceGlobalMapping();
            Options = null;
            MaterialOverrideMap = null;
            TempMaterialMap = null;
            PreservedMaterialMap = null;
            originalInternalMap = null;
            FolderPaths = null;
        }

        #endregion

        #region | Material Override Module |

        /// <summary>
        /// Replicate the internal reference map of the Importer with potential override data;
        /// </summary>
        /// <param name="model"> Model to reimport; </param>
        public void ProcessLibraryMaterialData(ModelImporter model) {
            originalInternalMap = MaterialUtils.LoadInternalMaterialMap(model);
            MaterialOverrideMap = new Dictionary<string, MaterialData>();
            foreach (KeyValuePair<string, Material> kvp in originalInternalMap) {
                MaterialOverrideMap[kvp.Key] = new MaterialData() { name = kvp.Key };
            } MaterialOverrideMap[SingleKey] = new MaterialData() { name = SingleKey };
            TempMaterialMap = new Dictionary<string, Material>();
            PreservedMaterialMap = new Dictionary<string, Material>();
        }

        /// <summary>
        /// Whether the passed key is the default key corresponding to Single Material Mode;
        /// </summary>
        /// <param name="key"> Key to check; </param>
        /// <returns> True if the passed key is the key corresponding to Single Mode; </returns>
        private bool IsSingleKey(string key) => string.IsNullOrWhiteSpace(key);

        /// <summary>
        /// A call to replace a material binding through the Reader;
        /// <br></br> Accounts for whether the passed materials should be preserved;
        /// </summary>
        /// <param name="key"> Key of the entry to remap; </param>
        /// <param name="material"> Material to place in the map; </param>
        /// <param name="reimport"> Whether the model should be reimported within this call; </param> 
        private void ReplacePersistentMaterial(string key, Material material, bool reimport = true) {

            /// If a new material is incoming to the slot, place a reserve in the slot;
            /// Else, if the slot was preserving a material but the replacement isn't new, remove the reserve;
            bool isNewMaterial = TempMaterialMap.ContainsValue(material);
            bool hadNewMaterial = PreservedMaterialMap.ContainsKey(key);
            if (isNewMaterial) PreservedMaterialMap[key] = material;
            else if (hadNewMaterial && !isNewMaterial) PreservedMaterialMap.Remove(key);
            /// Note that any key can reserve a single material at any given time;
            /// Multiple reserves are not an issue;

            MaterialUtils.ReplacePersistentMaterial(key, material, Options.model);
            if (reimport) {
                Options.model.SaveAndReimport();
                CleanPreview();
            }
        }

        /// <summary>
        /// Switch between None, Single, and Multiple Material Override modes;
        /// <br></br> Updates the model and preview accordingly; values are preserved statically;
        /// </summary>
        /// <param name="mom"> Material Override Mode; </param>
        public void SetMaterialOverrideMode(MaterialOverrideMode mom) {
            PreservedMaterialMap.Clear();
            switch (mom) {
                case MaterialOverrideMode.Single:
                    if (MaterialOverrideMap[SingleKey].isNew) {
                        if (TempMaterialMap.ContainsKey(SingleKey)) {
                            ReplaceGlobalMapping(TempMaterialMap[SingleKey]);
                        } else ReplaceGlobalMapping();
                    } else {
                        if (MaterialOverrideMap[SingleKey].materialRef != null) {
                            ReplaceGlobalMapping(MaterialOverrideMap[SingleKey].materialRef);
                        } else ReplaceGlobalMapping();
                    } break;
                case MaterialOverrideMode.Multiple:
                    foreach (KeyValuePair<string, MaterialData> kvp in MaterialOverrideMap) {
                        if (IsSingleKey(kvp.Key)) continue;
                        if (!kvp.Value.isNew && kvp.Value.materialRef != null) {
                            ReplacePersistentMaterial(kvp.Key, kvp.Value.materialRef, false);
                        } else if (kvp.Value.isNew && TempMaterialMap.ContainsKey(kvp.Key)) {
                            ReplacePersistentMaterial(kvp.Key, TempMaterialMap[kvp.Key], false);
                        } else ReplacePersistentMaterial(kvp.Key, originalInternalMap[kvp.Key], false);
                    } Options.model.SaveAndReimport();
                    CleanPreview();
                    break;
            } Options.materialOverrideMode = mom;
        }

        /// <summary>
        /// Update a material reference in a slot data;
        /// </summary>
        /// <param name="key"> Slot key to override; </param>
        /// <param name="material"> Material to place in the slot;
        /// <br></br> If set to null, restores the original material assigned in this slot; </param>
        public void UpdateMaterialRef(string key, Material material) {
            bool restore = material == null;
            MaterialOverrideMap[key].materialRef = material;
            if (IsSingleKey(key)) ReplaceGlobalMapping(restore ? null : material);
            else ReplacePersistentMaterial(key, restore ? originalInternalMap[key] : material);
        }

        /// <summary>
        /// Checks whether the given paramaters are enough to generate a material;
        /// </summary>
        /// <param name="key"> Key of the data to pull from the dictionary; </param>
        /// <returns> An array of invalid conditions, if any; </returns>
        public bool ValidateTemporaryMaterial(string key) {
            MaterialData data = MaterialOverrideMap[key];
            if (IsSingleKey(key)) {
                if (data.shader == null) return false;
            } else {
                if (Options.useSingleShader && Options.shader == null) return false;
                if (!Options.useSingleShader && data.shader == null) return false;
            } return true;
        }

        /// <summary>
        /// Check whether the current material data could be used to create a material ditinct from the active one;
        /// </summary>
        /// <param name="key"></param>
        /// <returns> True if the present and prospective materials are equal; </returns>
        public bool ValidateMaterialEquality(string key) {
            MaterialData data = MaterialOverrideMap[key];
            Material material = TempMaterialMap[key];
            if (material.mainTexture != data.albedoMap) return false;
            if ((material.IsKeywordEnabled("_NORMALMAP") && material.GetTexture("_BumpMap") != data.normalMap)
                || (!material.IsKeywordEnabled("_NORMALMAP") && data.normalMap != null)) return false;
            if (IsSingleKey(key)) {
                if (material.shader != data.shader) return false;
            } else if ( (Options.useSingleShader && material.shader != Options.shader)
                       || (!Options.useSingleShader && material.shader != data.shader) ) {
                return false;
            } return true;
        }

        /// <summary>
        /// Check whether the name abides by the project convention and is a valid file name;
        /// <br></br> Will return false if a material with the same name already exists in the temporary folder;
        /// </summary>
        /// <returns> True if the name is valid; </returns>
        public bool ValidateMaterialName(string name) {
            return GeneralUtils.ValidateFilename(TempMaterialManager.TempMaterialPath + "/" + name + ".mat", name) == 0;
        }

        /// <summary>
        /// Generate a Temporary material reference, as well as an external asset through an External Manager;
        /// </summary>
        /// <param name="key"> Slot key where the new material will be placed; </param>
        public void GenerateTemporaryMaterial(string key) {
            MaterialData data = MaterialOverrideMap[key];
            Material newMaterial;
            if (Options.materialOverrideMode == MaterialOverrideMode.Single) {
                newMaterial = new Material(data.shader);
            } else newMaterial = new Material(Options.useSingleShader ? Options.shader : data.shader);
            newMaterial.name = data.name;
            newMaterial.mainTexture = data.albedoMap;
            if (data.normalMap != null) {
                newMaterial.EnableKeyword("_NORMALMAP");
                newMaterial.SetTexture("_BumpMap", data.normalMap);
            } if (TempMaterialMap.ContainsKey(key)) RemoveNewMaterial(key);
            TempMaterialManager.CreateTemporaryMaterialAsset(newMaterial);
            TempMaterialMap[key] = newMaterial;
            if (IsSingleKey(key)) ReplaceGlobalMapping(newMaterial);
            else ReplacePersistentMaterial(key, newMaterial);
        }

        /// <summary>
        /// Switches between the New Material and Reference Material modes;
        /// <br></br> If a material replacement has not been defined, it restores the original mapping;
        /// </summary>
        /// <param name="key"> Key where the material origin will be toggled; </param>
        public void ToggleMaterialMap(string key) {
            MaterialData data = MaterialOverrideMap[key];
            if (originalInternalMap.ContainsKey(key)) {
                if (data.isNew && data.materialRef != null) ReplacePersistentMaterial(key, data.materialRef);
                else if (!data.isNew && TempMaterialMap.ContainsKey(key)) ReplacePersistentMaterial(key, TempMaterialMap[key]);
                else ReplacePersistentMaterial(key, originalInternalMap[key]);
            } else {
                if (data.isNew && data.materialRef != null) {
                    ReplaceGlobalMapping(data.materialRef);
                } else if (!data.isNew && TempMaterialMap.ContainsKey(key)) ReplaceGlobalMapping(TempMaterialMap[key]);
                else ReplaceGlobalMapping();
            } data.isNew = !data.isNew;
        }

        /// <summary>
        /// Replaces every material slot in the importer with a given material;
        /// </summary>
        /// <param name="newMaterial"> Material to place in every material slot; 
        /// <br></br> If set to null, restores every material slot to its original assignment; </param>
        public void ReplaceGlobalMapping(Material newMaterial = null) {
            foreach (KeyValuePair<string, Material> kvp in originalInternalMap) {
                if (newMaterial == null) ReplacePersistentMaterial(kvp.Key, kvp.Value, false);
                else ReplacePersistentMaterial(kvp.Key, newMaterial, false);
            } Options.model.SaveAndReimport();
            CleanPreview();
        }

        /// <summary>
        /// Remove a temporary material both from this and the external manager;
        /// </summary>
        /// <param name="key"> Key where the material to remove resides; </param>
        public void RemoveNewMaterial(string key) {
            TempMaterialManager.CleanMaterial(TempMaterialMap[key]);
            TempMaterialMap.Remove(key);
            if (originalInternalMap.ContainsKey(key)) ReplacePersistentMaterial(key, originalInternalMap[key]);
            else ReplaceGlobalMapping();
            PreservedMaterialMap.Remove(key);
        }

        public void CleanPreview() => Object.DestroyImmediate(preview);

        #endregion

        #endregion

        #region | GUI |

        /// <summary>
        /// Library Reimport context menu;
        /// </summary>
        [MenuItem("Assets/Library Reimport", false, 50)]
        public static void LibraryReimport() {
            string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            ModelImporter model = AssetImporter.GetAtPath(path) as ModelImporter;
            LibraryReimport(model);
        }

        /// <summary>
        /// Library Reimport function;
        /// </summary>
        /// <param name="model"> Model to reimport; </param>
        public static void LibraryReimport(ModelImporter model) {
            var window = ShowWindow();
            if (model != null) window.LoadBasicOptions(model);
        }

        /// <summary>
        /// Validate function for the Library Reimport context menu;
        /// </summary>
        /// <returns> Whether the selected asset is a Model; </returns>
        [MenuItem("Assets/Library Reimport", true)]
        private static bool LibraryReimportValidate() {
            return Selection.assetGUIDs.Length == 1 && ConfigurationCore.Config.rootAssetPath != null
                   && AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0])) is ModelImporter;
        }

        /// <summary>
        /// Show the Reimport Window;
        /// </summary>
        public static AssetPreprocessorGUI ShowWindow() {
            var window = GetWindow<AssetPreprocessorGUI>("Library Reimport");
            window.ShowAuxWindow();
            return window;
        }

        /// <summary> Array of new materials pending verification in the GUI; </summary>
        private Material[] tempMaterials;
        /// <summary> Key used by the Shader Selection event; </summary>
        private string shaderKey;

        /// <summary> Root GameObject of the target model;
        /// <br></br> Used for the Object Preview; </summary>
        private GameObject modelGO;

        private Vector2 globalScroll;
        private Vector2 materialsNewScroll;
        private Vector2 materialSlotScroll;

        void OnEnable() {
            if (Options == null) return;
            if (Options.model != null) {
                modelGO = AssetDatabase.LoadAssetAtPath<GameObject>(Options.model.assetPath);
            } ProcessLibraryMaterialData(Options.model);
            tempMaterials = new Material[MaterialOverrideMap.Count];
        }

        void OnDisable() {
            CleanPreview();
            if (Options != null) FlushImportData();

            tempMaterials = null;
            shaderKey = null;
            modelGO = null;
        }

        void OnGUI() {
            if (Options == null || Options.model == null) {
                EditorUtils.DrawScopeCenteredText("Unity revolted against this window.\nPlease reload it!") ;
                return;
            } using (var view = new EditorGUILayout.ScrollViewScope(globalScroll)) {
                globalScroll = view.scrollPosition;
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                    EditorUtils.DrawSeparatorLines("Core Import Settings", true);
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUILayout.Label("Import Mode:", UIStyles.ArrangedLabel, GUILayout.MaxWidth(125));
                        GUI.enabled = Options.hasMeshes;
                        GUILayout.Label("Model", 
                                        Options.hasMeshes ? UIStyles.ArrangedButtonSelected 
                                        : UIStyles.ArrangedBoxUnselected, GUILayout.MinWidth(105));
                        GUI.enabled = !Options.hasMeshes;
                        GUILayout.Label("Animation(s)",
                                        !Options.hasMeshes ? UIStyles.ArrangedButtonSelected
                                        : UIStyles.ArrangedBoxUnselected, GUILayout.MinWidth(105));
                        GUI.enabled = true;
                    } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUILayout.Label("Category:", UIStyles.ArrangedLabel, GUILayout.MaxWidth(125));
                        Options.folder = EditorGUILayout.Popup(Options.folder, FolderPaths, GUILayout.MinWidth(215));
                    } using (new EditorGUILayout.HorizontalScope()) {
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                            GUILayout.Label("Relocate Prefabs:", UIStyles.ArrangedLabel);
                            Options.relocatePrefabs = EditorGUILayout.Toggle(Options.relocatePrefabs, GUILayout.Width(16));
                        } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                            GUI.enabled = false;
                            GUILayout.Label("Relocate Materials:", UIStyles.ArrangedLabel);
                            Options.relocateMaterials = EditorGUILayout.Toggle(Options.relocateMaterials, GUILayout.Width(16));
                            GUI.enabled = true;
                        }
                    } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUILayout.Label("Material Settings:", UIStyles.ArrangedLabel, GUILayout.MaxWidth(125));
                        GUI.enabled = Options.hasMeshes;
                        Options.model.materialImportMode = (ModelImporterMaterialImportMode)
                                                           EditorGUILayout.EnumPopup(Options.model.materialImportMode, GUILayout.MinWidth(40));
                        Options.model.materialLocation = (ModelImporterMaterialLocation)
                                                         EditorGUILayout.EnumPopup(Options.model.materialLocation, GUILayout.MinWidth(40));
                    }
                } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                    EditorUtils.DrawSeparatorLines("Additional Import Settings", true);
                    if (Options.hasMeshes && Options.model.materialImportMode > 0 && Options.model.materialLocation > 0) {
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                            GUILayout.Label("Material Override:", UIStyles.ArrangedLabel);
                            if (GUILayout.Button("None", Options.materialOverrideMode == MaterialOverrideMode.None
                                                         ? UIStyles.ArrangedButtonSelected : GUI.skin.button, GUILayout.MinWidth(70))) {
                                SetMaterialOverrideMode(MaterialOverrideMode.None);
                            } if (GUILayout.Button("Single", Options.materialOverrideMode == MaterialOverrideMode.Single
                                                             ? UIStyles.ArrangedButtonSelected : GUI.skin.button, GUILayout.MinWidth(70))) {
                                SetMaterialOverrideMode(MaterialOverrideMode.Single);
                            } if (GUILayout.Button("Multiple", Options.materialOverrideMode == MaterialOverrideMode.Multiple
                                                               ? UIStyles.ArrangedButtonSelected : GUI.skin.button, GUILayout.MinWidth(70))) {
                                SetMaterialOverrideMode(MaterialOverrideMode.Multiple);
                            }
                        } if (Options.materialOverrideMode == MaterialOverrideMode.Multiple) {
                            using (new EditorGUILayout.HorizontalScope()) {
                                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.MaxWidth(150))) {
                                    GUILayout.Label("Use Single Shader:");
                                    Options.useSingleShader = EditorGUILayout.Toggle(Options.useSingleShader);
                                } bool noShaderSelected = Options.shader == null;
                                if (!Options.useSingleShader) GUI.enabled = false;
                                else if (noShaderSelected) GUI.color = UIColors.DarkRed;
                                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                                    GUI.color = Color.white;
                                    GUIContent shaderContent = new GUIContent(noShaderSelected ? "No Shader Selected" : Options.shader.name);
                                    DrawShaderPopup(shaderContent, null);
                                    DrawShaderHistoryPopup(null);
                                    GUI.enabled = true;
                                }
                            }
                        } DrawMaterialSettings();
                    } else {
                        using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                            GUI.color = new Color(0.9f, 0.9f, 0.9f);
                            GUIStyle nopeStyle = new GUIStyle(UIStyles.CenteredLabelBold);
                            nopeStyle.fontSize--;
                            GUILayout.Label("No Additional Settings", nopeStyle);
                            GUI.color = Color.white;
                        }
                    } 
                } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                        GUI.color = UIColors.Blue;
                        if (GUILayout.Button("Reimport")) {
                            ReimportAsset();
                            Close();
                        } GUI.color = UIColors.Red;
                        if (GUILayout.Button("Cancel")) Close();
                        GUI.color = Color.white;
                    }
                }
            }
        }

        /// <summary>
        /// Choose a Material Override Mode and Draw the content for that mode accordingly;
        /// </summary>
        private void DrawMaterialSettings() {

            if (modelGO != null) {
                switch (Options.materialOverrideMode) {
                    case MaterialOverrideMode.Single:
                        using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                                GUILayout.Label("Preview", UIStyles.CenteredLabel);
                                DrawPreview(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                                if (GUILayout.Button("Expanded Preview")) {
                                    PreviewExpanded.ShowPreviewWindow(modelGO);
                                }
                            }
                        } DrawMaterialSlot(SingleKey, MaterialOverrideMap[SingleKey], 0);
                        break;
                    case MaterialOverrideMode.Multiple:
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                                using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                                    GUILayout.Label("Preview", UIStyles.CenteredLabel);
                                    DrawPreview(GUILayout.Width(96), GUILayout.Height(112));
                                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { richText = true }; 
                                    buttonStyle.fontSize--;
                                    if (GUILayout.Button("<b>Expand Preview</b>", buttonStyle)) {
                                        PreviewExpanded.ShowPreviewWindow(modelGO);
                                    }
                                } using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(96))) {
                                    EditorUtils.DrawSeparatorLines("New Materials", true);
                                    using (var view = new EditorGUILayout.ScrollViewScope(materialsNewScroll)) {
                                        materialsNewScroll = view.scrollPosition;
                                        DrawNewMaterials();
                                    }
                                }
                            } using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                                EditorUtils.DrawSeparatorLines("Available Slots", true);
                                using (var view = new EditorGUILayout.ScrollViewScope(materialSlotScroll)) {
                                    materialSlotScroll = view.scrollPosition;
                                    DrawAvailableSlots();
                                }
                            }
                        } break;
                }
            } else GUILayout.Label("Something went wrong here, ask Carlos or something;", UIStyles.CenteredLabelBold);
        }

        private void DrawPreview(params GUILayoutOption[] options) {
            if (preview is null) preview = GenericPreview.CreatePreview(modelGO);
            preview.DrawPreview(options);
        }

        /// <summary>
        /// Draws a 'list' of all materials that will be 'created' after a successful reimport;
        /// </summary>
        private void DrawNewMaterials() {
            if (PreservedMaterialMap.Count > 0) {
                foreach (KeyValuePair<string, Material> kvp in PreservedMaterialMap) {
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                        GUILayout.Label(kvp.Key, UIStyles.CenteredLabelBold);
                        EditorGUILayout.ObjectField(kvp.Value, typeof(Material), false);
                    }
                }
            } else {
                GUILayout.Label("- Empty -", UIStyles.CenteredLabelBold);
                EditorGUILayout.Separator();
            }
        }

        /// <summary>
        /// Draws all available material slots for the given model;
        /// </summary>
        private void DrawAvailableSlots() {
            int i = 1;
            foreach (KeyValuePair<string, MaterialData> kvp in MaterialOverrideMap) {
                if (string.IsNullOrWhiteSpace(kvp.Key)) continue;
                DrawMaterialSlot(kvp.Key, kvp.Value, i);
                EditorGUILayout.Separator();
                EditorUtils.DrawSeparatorLine(1);
                EditorGUILayout.Separator();
                i++;
            }
        }

        /// <summary>
        /// Draw a material slot given a slot key;
        /// </summary>
        /// <param name="key"> Key to the slot to draw; </param>
        /// <param name="data"> Material Data pertaining to the slot to draw; </param>
        /// <param name="i"> How far down the GUI material list are we; </param>
        private void DrawMaterialSlot(string key, MaterialData data, int i) {
            using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                if (string.IsNullOrEmpty(key)) {
                    GUILayout.Label("Global Material", UIStyles.CenteredLabelBold);
                } else GUILayout.Label(key, UIStyles.CenteredLabelBold);
            } bool containsTempKey = TempMaterialMap.ContainsKey(key);
            if ((data.isNew && containsTempKey)
                || (!data.isNew && data.materialRef != null)) GUI.color = UIColors.Green;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUI.color = Color.white;
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    GUILayout.Label("Origin:", UIStyles.ArrangedLabel, GUILayout.MinWidth(52), GUILayout.MaxWidth(52));
                    if (GUILayout.Button("New", data.isNew
                                                    ? UIStyles.ArrangedButtonSelected : GUI.skin.button, GUILayout.MinWidth(70))) {
                        if (!data.isNew) ToggleMaterialMap(key);
                    } if (GUILayout.Button("Remap", data.isNew
                                                        ? GUI.skin.button : UIStyles.ArrangedButtonSelected, GUILayout.MinWidth(70))) {
                        if (data.isNew) ToggleMaterialMap(key);
                    }
                } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                    if (data.isNew) {
                        bool validName = ValidateMaterialName(data.name);
                        bool validMaterial = ValidateTemporaryMaterial(key);
                        if (!validName) GUI.color = UIColors.DarkRed;
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                            GUI.color = Color.white;
                            GUILayout.Label("Name:", GUILayout.MinWidth(48), GUILayout.MaxWidth(48));
                            data.name = EditorGUILayout.TextField(data.name);
                            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { margin = new RectOffset(), padding = new RectOffset() };
                            if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d__Help")),
                                             buttonStyle, GUILayout.Width(18), GUILayout.Height(18))) {
                                /// Insert Documentation URL Opener here;
                            }
                        } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                            GUILayout.Label("Albedo:", GUILayout.MinWidth(48), GUILayout.MaxWidth(48));
                            data.albedoMap = (Texture2D) EditorGUILayout.ObjectField(data.albedoMap, typeof(Texture2D), false);
                        } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                            GUILayout.Label("Normal:", GUILayout.MinWidth(48), GUILayout.MaxWidth(48));
                            data.normalMap = (Texture2D) EditorGUILayout.ObjectField(data.normalMap, typeof(Texture2D), false);
                        } if (!Options.useSingleShader || Options.materialOverrideMode == MaterialOverrideMode.Single) {
                            bool noShader = data.shader == null;
                            if (noShader) GUI.color = UIColors.DarkRed;
                            using (var scope = new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                                GUI.color = Color.white;
                                GUILayout.Label("Shader:", GUILayout.MinWidth(48), GUILayout.MaxWidth(48));
                                GUIContent shaderContent = new GUIContent(noShader ? "No Shader Selected" : data.shader.name);
                                DrawShaderPopup(shaderContent, key);
                            }
                        } /*if (!validMaterial || !validName)*/ GUI.enabled = false;
                        if (containsTempKey) {
                            bool materialsAreEqual = ValidateMaterialEquality(key);
                            if (materialsAreEqual) GUI.enabled = false;
                            using (new EditorGUILayout.HorizontalScope()) {
                                GUI.color = UIColors.Blue;
                                if (GUILayout.Button("Replace")) GenerateTemporaryMaterial(key);
                                GUI.enabled = true;
                                GUI.color = UIColors.Red;
                                if (GUILayout.Button("Remove")) RemoveNewMaterial(key);
                            } GUI.color = Color.white;
                        } else {
                            GUI.color = UIColors.Green;
                            if (GUILayout.Button("Generate Material")) GenerateTemporaryMaterial(key);
                            GUI.color = Color.white;
                        } if (!validMaterial) GUI.enabled = true;
                    } else {
                        using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.Label("Material:", GUILayout.MaxWidth(52));
                            bool wasAssigned = tempMaterials[i] != null;
                            tempMaterials[i] = (Material) EditorGUILayout.ObjectField(tempMaterials[i], typeof(Material), false);
                            if ( (tempMaterials[i] != null && tempMaterials[i] != data.materialRef)
                                || (wasAssigned && tempMaterials[i] == null) ) UpdateMaterialRef(key, tempMaterials[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a Shader Popup... It literally says it in the header bruh;
        /// <br></br> Used the Advanced Dropdown extracted by the Material Manager;
        /// </summary>
        /// <param name="shaderContent"> Text to display in the popup header; </param>
        /// <param name="key"> Key where the shader value will be placed; </param>
        private void DrawShaderPopup(GUIContent shaderContent, string key) {
            shaderKey = key;
            Rect position = EditorGUILayout.GetControlRect(GUILayout.MinWidth(135));
            MADShaderUtils.DrawDefaultShaderPopup(position, shaderContent, ApplyShaderResult);
        }

        /// <summary>
        /// Draws a button to display the global Shader Popup History;
        /// </summary>
        /// <param name="key"> Key where the shader value will be placed; </param>
        private void DrawShaderHistoryPopup(string key) {
            shaderKey = key;
            Rect position = EditorGUILayout.GetControlRect(GUILayout.Width(36));
            MADShaderUtils.DrawShaderHistoryPopup(position, ApplyShaderResult);
        }

        /// <summary>
        /// Response method to the Shader Selection event;
        /// </summary>
        /// <param name="shader"> Shader returned by the Shader Popup; </param>
        private void ApplyShaderResult(Shader shader) {
            if (shaderKey == null) Options.shader = shader;
            else MaterialOverrideMap[shaderKey].shader = shader;
            shaderKey = null;
        }

        #endregion
    }
}