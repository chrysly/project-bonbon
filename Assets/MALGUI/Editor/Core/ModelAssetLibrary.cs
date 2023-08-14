using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExtManager = ModelAssetLibraryExtManager;

/// <summary> Core class for the Model Asset Library; 
/// <br></br> Reads and catalogs data from the Asset Database and the target File Structure;
/// </summary>
public static class ModelAssetLibrary {

    /// Relevant Tool Information ///
    /// The Model Asset Library runs on several static classes, all of which are unable to store data 
    /// persistently by conventional means. In a regular Editor session, static data will remain in memory 
    /// until the Editor is restarted. The classes will have access to static dictionaries that might be updated 
    /// through calls from the GUI; any persistent data generated through the library, by contrast, is saved 
    /// on persistent scriptable objects, each linked to a potential reference by GUID;
    /// 
    /// For safety reasons, the Model Asset Library operates discreetly on the designated Hierarchy. Most
    /// operations performed on the imported files themselves are read-only, save for certain Unity-exclusive
    /// asset maps that might remain serialized in the Model Importer. The Tool does not access any project
    /// data outside of the designated folders, unless otherwise specified by a manual GUI operation;

    #region | Configuration variables |

    /// <summary> Path to the root folder of the hierarchy to search; </summary>
    public static string RootAssetPath { get { return ModelAssetLibraryConfigurationCore.RootAssetPath; } }

    /// <summary> File extensions to contemplate in the search; </summary>
    public static string[] ModelFileExtensions { 
        get {
            if (ModelAssetLibraryConfigurationCore.ModelFileExtensions != null) {
                return ModelAssetLibraryConfigurationCore.ModelFileExtensions.Split(" ");
            } return new string[0];
        }
    }

    #endregion

    #region | Asset Library Data |

    /// <summary>
    /// A baby class containing a model path and its associated prefab IDs. Halleluleh ;-;
    /// </summary>
    public class ModelData {
        public string name;
        public string path;
        public List<string> prefabIDList;
        public ModelAssetLibraryExtData extData;

        public ModelData(string name, string path, List<string> prefabIDList, ModelAssetLibraryExtData extData) {
            this.name = name;
            this.path = path;
            this.prefabIDList = prefabIDList;
            this.extData = extData;
        }
    } /// <summary> Maps a Model GUID to its file path and associated prefabs; </summary>
    public static Dictionary<string, ModelData> ModelDataDict { get; private set; }

    /// <summary>
    /// A baby class containing a prefab path and its associated model ID. Superb ;-;
    /// </summary>
    public class PrefabData {
        public string name;
        public string path;
        public string modelID;

        public PrefabData(string name, string path, string modelID) {
            this.name = name;
            this.path = path;
            this.modelID = modelID;
        }
    } /// <summary> Maps a Prefab GUID to its file path and associated model </summary>
    public static Dictionary<string, PrefabData> PrefabDataDict { get; private set; }

    /// <summary> Registry options for the FindAssets() method; </summary>
    public enum RegistryMode {
        /// <summary> No assets will be registered while searching the folders; </summary>
        None,
        /// <summary> Register assets to the Model Dictionary; </summary>
        Model,
        /// <summary> Register assets to the Prefab Dictionary; </summary>
        Prefab
    }

    #endregion

    #region | Initialization & Update |

    /// <summary>
    /// Reload all static library data;
    /// </summary>
    public static void Refresh() {
        LoadDictionaries();
        ExtManager.Refresh();
        if (!string.IsNullOrWhiteSpace(RootAssetPath)
            && ModelFileExtensions != null 
            && ModelFileExtensions.Length > 0) {
            ReloadModelEntries();
            ReloadPrefabEntries();
        }
    }

    /// <summary>
    /// Replace static dictionary data with the persistent data;
    /// </summary>
    public static void LoadDictionaries() {
        ModelDataDict = new Dictionary<string, ModelData>();
        PrefabDataDict = new Dictionary<string, PrefabData>();
    }

    /// <summary>
    /// Flushes all the data collected by the library, so it doesn't sit in memory;
    /// <br></br> Called if the corresponding option is selected in the Configuration GUI;
    /// </summary>
    public static void UnloadDictionaries() {
        ModelDataDict = null;
        PrefabDataDict = null;
    }

    /// <summary>
    /// Iterates recursively across the Model Hierarchy to read relevant model files;
    /// </summary>
    /// <param name="path"> Recursive paramater overriden by the Root path on the first call; </param>
    public static void ReloadModelEntries(string path = null) {
        if (path == null) {
            string[] modelDataKeys = new string[ModelDataDict.Count];
            ModelDataDict.Keys.CopyTo(modelDataKeys, 0);
            foreach (string modelID in modelDataKeys) {
                ValidateModelEntry(modelID);
            } path = RootAssetPath;
        } string[] subfolders = Directory.GetDirectories(path);
        FindAssets(path, ModelFileExtensions, RegistryMode.Model);
        foreach (string subfolder in subfolders) {
            ReloadModelEntries(subfolder);
        } 
    }

    /// <summary>
    /// Iterates over the registered prefab data to remove deprecated keys and relocate misplaced assets;
    /// </summary>
    public static void ReloadPrefabEntries(string path = null) {
        if (path == null) {
            string[] prefabDataKeys = new string[PrefabDataDict.Count];
            PrefabDataDict.Keys.CopyTo(prefabDataKeys, 0);
            foreach (string prefabID in prefabDataKeys) {
                ValidatePrefabEntry(prefabID);
            } path = RootAssetPath;
        } string[] subfolders = Directory.GetDirectories(path);
        FindAssets(path, new string[] { "prefab" }, RegistryMode.Prefab);
        foreach (string subfolder in subfolders) {
            ReloadPrefabEntries(subfolder);
        }
    }

    #endregion

    #region | Data Validation & Registration |

    /// <summary>
    /// Checks whether the Model asset exists and whether the associated paths are up-to-date;
    /// <br></br> Updates deprecated paths and calls for the relocation of linked prefabs accordingly;
    /// </summary>
    /// <param name="modelID"> ID of the model to check; </param>
    /// <param name="databasePath"> Optional file path <b>from the asset database</b> if the string is already accessible; </param>
    /// <returns> Whether the model asset exists in the Asset Database </returns>
    private static bool ValidateModelEntry(string modelID) {
        string databasePath = AssetDatabase.GUIDToAssetPath(modelID);
        if (NoAssetAtPath(databasePath)) {
            ModelDataDict.Remove(modelID);
            return false;
        } return true;
    }

    /// <summary>
    /// Checks whether the Prefab asset exists or should exist, and whether the associated paths are up-to-date;
    /// <br></br> Updates deprecated paths and calls for self-relocation accordingly;
    /// </summary>
    /// <param name="prefabID"> ID of the prefab to check; </param>
    /// <returns> Whether the prefab asset is linked to a model <b>and</b> exists in the Asset Database </returns>
    private static bool ValidatePrefabEntry(string prefabID) {
        string databasePath = AssetDatabase.GUIDToAssetPath(prefabID);
        if (NoAssetAtPath(databasePath)) {
            string associatedModelID = PrefabDataDict[prefabID].modelID;
            ModelDataDict[associatedModelID].prefabIDList.Remove(prefabID);
            PrefabDataDict.Remove(prefabID);
            return false;
        } if (!ModelDataDict.ContainsKey(PrefabDataDict[prefabID].modelID)) {
            AssetDatabase.MoveAssetToTrash(PrefabDataDict[prefabID].path);
            PrefabDataDict.Remove(prefabID);
            AssetDatabase.Refresh();
            return false;
        } return true;
    }

    /// <summary>
    /// Search a folder for paths to all assets with a given file extension;
    /// </summary>
    /// <param name="path"> Path to the folder to search; </param>
    /// <param name="registryMode"> How to register read entries in the Model Data Dictionary </param>
    /// <returns> Array of relevant file paths; </returns>
    public static string[] FindAssets(string path, string[] fileExtensions, RegistryMode registryMode = 0) {

        List<string> extensions = new List<string>();
        foreach (string extension in fileExtensions) extensions.Add(extension.ToLower());

        List<string> matchingFiles = new List<string>();
        string[] files = Directory.GetFiles(path);

        foreach (string file in files) {
            if ( extensions.Contains(file.IsolatePathEnd(".").ToLower()) ) {
                string parsedPath = file.Replace('\\', '/');
                matchingFiles.Add(parsedPath);
                switch(registryMode) {
                    case RegistryMode.Model:
                        RegisterModel(AssetDatabase.AssetPathToGUID(parsedPath));
                        break;
                    case RegistryMode.Prefab:
                        RegisterPrefabConditionally(parsedPath);
                        break;
                }
            }
        } return matchingFiles.ToArray();
    }

    /// <summary>
    /// Registers a Model ID key in the Model Data Dictionary if the Key does not exist;
    /// </summary>
    /// <param name="modelID"> ID of the model whose entries must be added or updated; </param>
    private static void RegisterModel(string modelID) {
        if (!ModelDataDict.ContainsKey(modelID)) {
            string modelPath = AssetDatabase.GUIDToAssetPath(modelID).Replace('\\', '/');
            string name = modelPath.IsolatePathEnd("/").RemovePathEnd(".");
            var extData = ExtManager.CreateExtData(modelID);
            ModelDataDict[modelID] = new ModelData(name, modelPath, new List<string>(), extData);
        } else ValidateModelEntry(modelID);
    }

    /// <summary>
    /// Verifies if the target prefab is a Prefab Variant;
    /// <br></br> If it is, registers the variant as Prefab of the base model;
    /// </summary>
    /// <param name="path"> Path of the file to register; </param>
    private static void RegisterPrefabConditionally(string path) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        string prefabID = AssetDatabase.AssetPathToGUID(path);
        if (PrefabDataDict.ContainsKey(prefabID)) return;
        if (prefab != null && PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Variant) {
            GameObject modelPrefab = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
            if (modelPrefab != null) {
                string modelID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(modelPrefab));
                if (!ModelDataDict.ContainsKey(modelID)) RegisterModel(modelID);
                if (!ModelDataDict[modelID].prefabIDList.Contains(prefabID)) {
                    ModelDataDict[modelID].prefabIDList.Add(prefabID);
                } string name = path.IsolatePathEnd("\\/").RemovePathEnd(".");
                PrefabDataDict[prefabID] = new PrefabData(name, path, modelID);
            }
        }
    }

    /// <summary>
    /// Registers a new Prefab Variant to Model;
    /// <br></br> Calls the Prefab and updates associated dictionary entries;
    /// </summary>
    /// <param name="modelID"> ID Key to register/update; </param>
    public static void RegisterNewPrefab(string modelID, string fileName = null) {
        string prefabPath = CreatePrefabFromModel(AssetDatabase.GUIDToAssetPath(modelID), fileName);
        string name = prefabPath.IsolatePathEnd("\\/").RemovePathEnd(".");
        string prefabID = AssetDatabase.AssetPathToGUID(prefabPath);
        PrefabDataDict[prefabID] = new PrefabData(name, prefabPath, modelID);
        if (ModelDataDict.ContainsKey(modelID) && !ModelDataDict[modelID].prefabIDList.Contains(prefabID)) {
            ModelDataDict[modelID].prefabIDList.Add(prefabID);
        }
    }

    #endregion

    #region | Prefab Creation & Directory Management |

    /// <summary>
    /// Creates a Prefab Variant from a Model asset;
    /// <br></br> Note: This method WILL overwrite an existing prefab if one exists at the target path;
    /// </summary>
    /// <param name="modelPath"> Path to the asset to create a prefab from; </param>
    /// <param name="fileName"> File name of the new prefab, <b>without the extension</b> ; </param>
    /// <returns> Path to the created prefab; </returns>
    public static string CreatePrefabFromModel(string modelPath, string fileName) {

        /// Instantiate prefab from model;
        GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(modelPath));

        /// Rearrange model path to create prefab path;
        string targetPath = modelPath.ToPrefabPath();
        Directory.CreateDirectory(targetPath);

        /// Save prefab in path and destroy temporary scene instance;
        targetPath = fileName == null ? modelPath.ToPrefabPath(true) : modelPath.ToPrefabPathWithName(fileName);
        PrefabUtility.SaveAsPrefabAsset(go, targetPath);
        AssetDatabase.Refresh();
        UnityEngine.Object.DestroyImmediate(go);

        return targetPath;
    }

    /// <summary>
    /// Relocates all the prefabs associated with a model to a neighboring folder;
    /// </summary>
    /// <param name="modelID"> Model whose prefabs should be relocated; </param>
    public static void RelocatePrefabsWithModel(string modelID) {
        foreach (string prefabID in ModelDataDict[modelID].prefabIDList) {
            RelocatePrefab(prefabID);
        }
    }

    /// <summary>
    /// Relocates a prefab to mirror the file structure of the model folders;
    /// <br></br> Calls for a conditional clean-up of the original folder;
    /// </summary>
    /// <param name="prefabID"> ID of the Prefab to Move; </param>
    public static void RelocatePrefab(string prefabID) {
        string modelID = PrefabDataDict[prefabID].modelID;
        string originalFilePath = AssetDatabase.GUIDToAssetPath(prefabID).Replace('\\', '/');
        if (NoAssetAtPath(originalFilePath)) {
            ModelDataDict[modelID].prefabIDList.Remove(prefabID);
            ModelDataDict.Remove(prefabID);
            return;
        }

        string modelPath = AssetDatabase.GUIDToAssetPath(modelID).Replace('\\', '/');
        string targetFilePath = modelPath.ToPrefabPathWithGUID(prefabID);
        string targetFolderPath = modelPath.ToPrefabPath();

        if (originalFilePath != targetFilePath) {
            if (!AssetDatabase.IsValidFolder(targetFolderPath)) {
                AssetDatabase.CreateFolder(targetFolderPath.RemovePathEnd("/"), targetFolderPath.IsolatePathEnd("/"));
            }
            AssetDatabase.MoveAsset(originalFilePath, targetFilePath);
            AssetDatabase.Refresh();
            PrefabDataDict[prefabID].path = targetFilePath;
            CleanEmptyFolders();
        }
    }

    /// <summary>
    /// Deletes a prefab asset and unregisters all references to it;
    /// </summary>
    /// <param name="prefabID"> ID of the prefab to delete; </param>
    public static void DeletePrefab(string prefabID) {
        if (PrefabDataDict.ContainsKey(prefabID)) {
            string modelID = PrefabDataDict[prefabID].modelID;
            if (ModelDataDict.ContainsKey(modelID)) {
                ModelDataDict[modelID].prefabIDList.Remove(prefabID);
            } string path = PrefabDataDict[prefabID].path;
            PrefabDataDict.Remove(prefabID);
            AssetDatabase.MoveAssetToTrash(path);
            CleanEmptyFolders();
        }
    }

    /// <summary>
    /// Iterates over all the subfolders of the root directory and removes any empty folders;
    /// </summary>
    private static void CleanEmptyFolders() {
        DirectoryInfo directoryInfo = new DirectoryInfo(RootAssetPath);
        foreach (var directory in directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories)) {
            if (directory.Exists) {
                var files = directory.GetFiles("*.*", SearchOption.AllDirectories);
                if (files.Length == 0) {
                    var deletedFolderName = directory.FullName.IsolatePathEnd("\\/").TrimEnd(new char[] { '\\', '/' });
                    try {
                        directory.Delete();
                    } catch (IOException) {
                        return;
                    }
                    string metaFilePath = directory.Parent + "\\" + deletedFolderName + ".meta";
                    if (File.Exists(metaFilePath)) {
                        File.Delete(metaFilePath);
                        AssetDatabase.Refresh();
                    } else Debug.LogWarning("Failed to delete .meta file at: " + metaFilePath);
                }
            }
        }
    }

    /// <summary>
    /// Checks if there's no asset at the given path;
    /// </summary>
    /// <param name="path"> Path to check; </param>
    /// <returns> True if the asset <b>doesn't</b> exist; </returns>
    public static bool NoAssetAtPath(string path) => string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));

    #endregion

    #region | String Utils |

    /// <summary>
    /// Remove the ending of a string up to a given delimeter;<br></br>
    /// Used in the script to remove substrings pertaining to file pathing;
    /// </summary>
    /// <param name="str"> String to manipulate; </param>
    /// <param name="delimiters"> Use "\\/" to remove the file part;
    /// <br></br> Use "." to remove the file extension; </param>
    /// <returns> String representing the full path without the final part (ex: without the file name); </returns>
    public static string RemovePathEnd(this string str, string delimiters) {
        var index = 0;
        for (int i = str.Length - 1; i >= 0; i--) {
            if (delimiters.Contains(str[i])) {
                index = i;
                break;
            }
        } var nArr = new List<char>(str.ToCharArray()).GetRange(0, index).ToArray();
        return new string(nArr);
    }

    /// <summary>
    /// Isolate the ending of a string up to a given delimeter;
    /// <br></br> Used in the script to retrieve file and folder names with our without file extensions, if applicable;
    /// </summary>
    /// <param name="str"> String to manipulate </param>
    /// <param name="delimiters"> Use "\\/" to isolate the final part of the path, either a folder or a file name;
    /// <br></br> Use "." to isolate the file extension of a file, if applicable; </param>
    /// <param name="trimExtension"> Whether or not to trim the file extension from the isolated file name; </param>
    /// <returns> String representing the end of the path only; </returns>
    public static string IsolatePathEnd(this string str, string delimiters, bool trimExtension = false) {
        var nStr = "";
        for (int i = str.Length - 1; i >= 0; i--) {
            if (delimiters.Contains(str[i])) break;
            else nStr += str[i];
        }
        var nArr = nStr.ToCharArray(); Array.Reverse(nArr);
        if (trimExtension) {
            for (int i = nArr.Length - 1; i >= 0; i--) {
                if (nArr[i] == '.') {
                    nArr = new List<char>(nArr).GetRange(0, i).ToArray();
                    break;
                }
            }
        } return new string(nArr);
    }

    /// <summary>
    /// Transforms a Model path into a Prefab path by changing the name of the root;
    /// <br></br> May trim the file name or replace it by the default Model name if indicated;
    /// </summary>
    /// <param name="modelPath"> Model path to modify; </param>
    /// <param name="includeFileName"> Whether to replace the file name with the default Model name; </param>
    /// <returns> Path string pointing towards the Prefab file hierarchy; </returns>
    public static string ToPrefabPath(this string modelPath, bool includeDefaultName = false) {
        string targetPath = modelPath.RemovePathEnd("\\/") + "/Prefabs";
        if (includeDefaultName) targetPath += "/" + modelPath.IsolatePathEnd("\\/", true) + ".prefab";
        return targetPath;
    }

    /// <summary>
    /// Transforms a Model path into a Prefab path by changing the name of the root;
    /// <br></br> Replaces the file name by the given name;
    /// </summary>
    /// <param name="modelPath"> Model path to modify; </param>
    /// <param name="fileName"> Name that will replace the default file name; </param>
    /// <returns> Path string pointing towards the Prefab file hierarchy; </returns>
    public static string ToPrefabPathWithName(this string modelPath, string fileName) {
        string targetPath = modelPath.RemovePathEnd("\\/") + "/Prefabs";
        targetPath += "/" + fileName + ".prefab";
        return targetPath;
    }

    /// <summary>
    /// Transforms a Model path into a Prefab path by changing the name of the root;
    /// <br></br> Replaces the file name by the name of an existing prefab whose ID is known;
    /// </summary>
    /// <param name="modelPath"> Model path to modify; </param>
    /// <param name="fileName"> GUID of the object whose name will replace the default file name; </param>
    /// <returns> Path string pointing towards the Prefab file hierarchy; </returns>
    private static string ToPrefabPathWithGUID(this string modelPath, string guid) {
        string targetPath = modelPath.RemovePathEnd("\\/") + "/Prefabs";
        targetPath += "/" + AssetDatabase.GUIDToAssetPath(guid).IsolatePathEnd("\\/", true) + ".prefab";
        return targetPath;
    }

    #endregion
}