using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Reader = ModelAssetLibraryModelReader;
using HierarchyBuilder = ModelAssetLibraryHierarchyBuilder;
using TempManager = ModelAssetLibraryTempMaterialManager;
using ExtManager = ModelAssetLibraryExtManager;
using ExtData = ModelAssetLibraryExtData;

/// <summary>
/// Processes some features of an imported model and allows for further customization of such Model;
/// </summary>
public static class ModelAssetLibraryAssetPreprocessor {

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
    public static ImportOverrideOptions Options { get; set; }

    /// <summary> An array of paths to pick from for the Model Relocation method; </summary>
    public static string[] FolderPaths { get; private set; }

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
    public static Dictionary<string, MaterialData> MaterialOverrideMap { get; set; }

    /// <summary> External key reserved for the Single Material Override Mode; </summary>
    public static string SingleKey { get { return ""; } private set { SingleKey = value; } }

    /// <summary> Map of identifier keys to newly generated materials; </summary>
    public static Dictionary<string, Material> TempMaterialMap { get; private set; }

    /// <summary> Map of identifier keys to New Materials that will ultimately remain in use; </summary>
    public static Dictionary<string, Material> PreservedMaterialMap { get; private set; }

    /// <summary> Original material map used by the Model to reimport; </summary>
    private static Dictionary<string, Material> originalInternalMap;

    /// <summary> Delegate for the local Shader Popup event set-up; </summary>
    public static System.Action<Shader> OnShaderResult;

    #endregion

    #region | Core Import Functions |

    /// <summary>
    /// Creates an Import Override Options object with publish-ready default values;
    /// </summary>
    /// <param name="modelImporter"> Model to create the Import Options object for; </param>
    public static void LoadBasicOptions(ModelImporter modelImporter) {
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
    private static void FetchFolderPaths() {
        var folderMap = HierarchyBuilder.BuildFolderMap(ModelAssetLibrary.RootAssetPath);
        FolderPaths = new string[folderMap.Keys.Count + 1];
        int i = 1;
        foreach (KeyValuePair<string, HierarchyBuilder.FolderData> kvp in folderMap) {
            FolderPaths[i] = kvp.Key;
            i++;
        } FolderPaths[0] = "None";
    }

    /// <summary>
    /// Mark the asset as reimported;
    /// </summary>
    public static void SignExtData() {
        ExtManager.Refresh();
        ExtData extData = ExtManager.CreateExtData(Options.modelID);
        extData.isReimported = true;
    }

    /// <summary>
    /// Moves a model asset and the selected dependencies to a given folder;
    /// </summary>
    public static void RelocateModelAsset() {
        if (Options.folder != 0) {
            string oldPath = Options.model.assetPath;
            string newPath = FolderPaths[Options.folder] + "/" + oldPath.IsolatePathEnd("\\/");
            string moveMessage = AssetDatabase.ValidateMoveAsset(oldPath, newPath);
            if (string.IsNullOrEmpty(moveMessage)) {
                AssetDatabase.MoveAsset(Options.model.assetPath, newPath);
            } else Debug.LogWarning(moveMessage);
        } if (Options.relocatePrefabs) ModelAssetLibrary.RelocatePrefabsWithModel(Options.modelID);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Apply the Import Overrides designated in the GUI;
    /// </summary>
    public static void ReimportAsset() {
        RelocateModelAsset();
        foreach (KeyValuePair<string, Material> kvp in PreservedMaterialMap) {
            TempMaterialMap.Remove(kvp.Key);
            TempManager.ReleaseMaterial(kvp.Value);
        } SignExtData();
        Options.model.SaveAndReimport();
        FlushImportData(false);
        Options = null;
    }

    /// <summary>
    /// Flushes all data held by this static class;
    /// </summary>
    public static void FlushImportData(bool restore = true) {
        if (TempMaterialMap != null) {
            foreach (KeyValuePair<string, Material> kvp in TempMaterialMap) {
                if (kvp.Value != null) Object.DestroyImmediate(kvp.Value, true);
            } TempManager.CleanAllMaterials();
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
    public static void ProcessLibraryMaterialData(ModelImporter model) {
        originalInternalMap = Reader.LoadInternalMaterialMap(model);
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
    private static bool IsSingleKey(string key) => string.IsNullOrWhiteSpace(key);

    /// <summary>
    /// A call to replace a material binding through the Reader;
    /// <br></br> Accounts for whether the passed materials should be preserved;
    /// </summary>
    /// <param name="key"> Key of the entry to remap; </param>
    /// <param name="material"> Material to place in the map; </param>
    /// <param name="reimport"> Whether the model should be reimported within this call; </param> 
    private static void ReplacePersistentMaterial(string key, Material material, bool reimport = true) {

        /// If a new material is incoming to the slot, place a reserve in the slot;
        /// Else, if the slot was preserving a material but the replacement isn't new, remove the reserve;
        bool isNewMaterial = TempMaterialMap.ContainsValue(material);
        bool hadNewMaterial = PreservedMaterialMap.ContainsKey(key);
        if (isNewMaterial) PreservedMaterialMap[key] = material;
        else if (hadNewMaterial && !isNewMaterial) PreservedMaterialMap.Remove(key);
        /// Note that any key can reserve a single material at any given time;
        /// Multiple reserves are not an issue;

        Reader.ReplacePersistentMaterial(key, material, Options.model);
        if (reimport) {
            Options.model.SaveAndReimport();
            Reader.CleanObjectPreview();
        }
    }

    /// <summary>
    /// Switch between None, Single, and Multiple Material Override modes;
    /// <br></br> Updates the model and preview accordingly; values are preserved statically;
    /// </summary>
    /// <param name="mom"> Material Override Mode; </param>
    public static void SetMaterialOverrideMode(MaterialOverrideMode mom) {
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
                Reader.CleanObjectPreview();
                break;
        } Options.materialOverrideMode = mom;
    }

    /// <summary>
    /// Update a material reference in a slot data;
    /// </summary>
    /// <param name="key"> Slot key to override; </param>
    /// <param name="material"> Material to place in the slot;
    /// <br></br> If set to null, restores the original material assigned in this slot; </param>
    public static void UpdateMaterialRef(string key, Material material) {
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
    public static bool ValidateTemporaryMaterial(string key) {
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
    public static bool ValidateMaterialEquality(string key) {
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
    public static bool ValidateMaterialName(string name) {
        return Reader.ValidateFilename(TempManager.TempMaterialPath + "/" + name + ".mat", name) == 0;
    }

    /// <summary>
    /// Generate a Temporary material reference, as well as an external asset through an External Manager;
    /// </summary>
    /// <param name="key"> Slot key where the new material will be placed; </param>
    public static void GenerateTemporaryMaterial(string key) {
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
        TempManager.CreateTemporaryMaterialAsset(newMaterial);
        TempMaterialMap[key] = newMaterial;
        if (IsSingleKey(key)) ReplaceGlobalMapping(newMaterial);
        else ReplacePersistentMaterial(key, newMaterial);
    }

    /// <summary>
    /// Switches between the New Material and Reference Material modes;
    /// <br></br> If a material replacement has not been defined, it restores the original mapping;
    /// </summary>
    /// <param name="key"> Key where the material origin will be toggled; </param>
    public static void ToggleMaterialMap(string key) {
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
    public static void ReplaceGlobalMapping(Material newMaterial = null) {
        foreach (KeyValuePair<string, Material> kvp in originalInternalMap) {
            if (newMaterial == null) ReplacePersistentMaterial(kvp.Key, kvp.Value, false);
            else ReplacePersistentMaterial(kvp.Key, newMaterial, false);
        } Options.model.SaveAndReimport();
        Reader.CleanObjectPreview();
    }

    /// <summary>
    /// Remove a temporary material both from this and the external manager;
    /// </summary>
    /// <param name="key"> Key where the material to remove resides; </param>
    public static void RemoveNewMaterial(string key) {
        TempManager.CleanMaterial(TempMaterialMap[key]);
        TempMaterialMap.Remove(key);
        if (originalInternalMap.ContainsKey(key)) ReplacePersistentMaterial(key, originalInternalMap[key]);
        else ReplaceGlobalMapping();
        PreservedMaterialMap.Remove(key);
    }

    #endregion

    #region | Shader Selection Shenanigans |

    /// <summary>
    /// Fetches the internal Advanced Popup used for shader selection in the Material Editor;
    /// </summary>
    /// <param name="position"> Rect used to draw the popup button; </param>
    public static void ShowShaderSelectionMagic(Rect position) {
        System.Type type = typeof(Editor).Assembly.GetType("UnityEditor.MaterialEditor+ShaderSelectionDropdown");
        var dropDown = System.Activator.CreateInstance(type, args: new object[] { Shader.Find("Transparent/Diffuse"), (System.Action<object>) OnSelectedShaderPopup });
        MethodInfo method = type.GetMethod("Show");
        method.Invoke(dropDown, new object[] { position });
    }

    /// <summary>
    /// Output method for the Shader Selection event set-up;
    /// </summary>
    /// <param name="objShaderName"> Object output from the Shader Selection event containing a shader name; </param>
    private static void OnSelectedShaderPopup(object objShaderName) {
        var shaderName = (string) objShaderName;
        if (!string.IsNullOrEmpty(shaderName)) {
            OnShaderResult?.Invoke(Shader.Find(shaderName));
        }
    }

    #endregion
}