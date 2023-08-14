using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Handles the creation and disposal of potential material assets;
/// </summary>
public static class ModelAssetLibraryTempMaterialManager {

    /// <summary> A variable to store the path so it doesn't have to be 'found' as often; </summary>
    private static string tempPath;
    /// <summary> String path where new temporary files will be created; </summary>
    public static string TempMaterialPath {
        get {
            if (tempPath == null) {
                string[] guids = AssetDatabase.FindAssets($"t:Script {nameof(ModelAssetLibraryTempMaterialManager)}");
                tempPath = AssetDatabase.GUIDToAssetPath(guids[0]).RemovePathEnd("\\/");
            } return tempPath;
        }
    }

    /// <summary> Maps the GUID of a created material to its asset path; </summary>
    private static Dictionary<Material, string> tempMaterialDict;

    /// <summary>
    /// Creates a temporary (persistent for a while) asset from a given material object;
    /// </summary>
    /// <param name="material"> Material object to create an asset for; </param>
    public static void CreateTemporaryMaterialAsset(Material material) {
        if (tempMaterialDict == null) tempMaterialDict = new Dictionary<Material, string>();
        string path = TempMaterialPath + "/" + material.name + ".mat";
        AssetDatabase.CreateAsset(material, path);
        tempMaterialDict[material] = path;
    }

    /// <summary>
    /// Publish a material persistenly;
    /// </summary>
    /// <param name="material"> Material to release into the wilderness; </param>
    /// <param name="path"> Folder where the material will be placed; 
    /// <br></br> If null, the material will be placed in a default folder at the Root Path of the Library; </param>
    /// <returns> True if the asset was moved successfully, false otherwise; </returns>
    public static void ReleaseMaterial(Material material, string path = null) {
        if (!tempMaterialDict.ContainsKey(material)) return;
        if (path != null) {
            string pathValidation = AssetDatabase.ValidateMoveAsset(tempMaterialDict[material], path);
            if (string.IsNullOrEmpty(pathValidation)) {
                AssetDatabase.MoveAsset(tempMaterialDict[material], path);
            } else Debug.LogWarning(pathValidation);
        } else {
            /*
            string rootLocation = ModelAssetLibrary.RootAssetPath + "/Materials/" + tempMaterialDict[material].IsolatePathEnd("\\/");
            string pathValidation = AssetDatabase.ValidateMoveAsset(tempMaterialDict[material], rootLocation);
            if (string.IsNullOrEmpty(pathValidation)) {
                AssetDatabase.MoveAsset(tempMaterialDict[material], rootLocation);
            } else Debug.LogWarning(pathValidation);
            */
        } tempMaterialDict.Remove(material);
    }

    /// <summary>
    /// Destroy a material asset from the Manager's dictionary;
    /// <br></br> Quite safe to use, actually;
    /// </summary>
    /// <param name="material"> Material object whose asset must be deleted; </param>
    public static void CleanMaterial(Material material) {
        if (tempMaterialDict.ContainsKey(material) && File.Exists(tempMaterialDict[material])) {
            AssetDatabase.DeleteAsset(tempMaterialDict[material]);
            tempMaterialDict.Remove(material);
        }
    }

    /// <summary>
    /// Destroy all material assets with a static reference on this manager;
    /// <br></br> Nulls the dictionary at the end;
    /// </summary>
    public static void CleanAllMaterials() {
        if (tempMaterialDict == null) return;
        foreach (KeyValuePair<Material, string> kvp in tempMaterialDict) {
            if (File.Exists(kvp.Value)) AssetDatabase.DeleteAsset(kvp.Value);
        } tempMaterialDict = null;
    }
}