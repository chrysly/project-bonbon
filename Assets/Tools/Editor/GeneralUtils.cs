using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// General utilities for the model AssetLibrary;
/// </summary>
public static class GeneralUtils {

    /// <summary>
    /// Open the file explorer and return a folder path;
    /// </summary>
    /// <returns> Folder path; </returns>
    public static string OpenAndParseFolder() {
        string res = EditorUtility.OpenFolderPanel("Set Root Path", "Assets", "");
        if (res != null && res.StartsWith(Application.dataPath)) {
            res = "Assets" + res.Substring(Application.dataPath.Length);
            return res;
        } else return null;
    }

    /// <summary>
    /// Checks if there's no asset at the given path;
    /// </summary>
    /// <param name="path"> Path to check; </param>
    /// <returns> True if the asset <b>doesn't</b> exist; </returns>
    public static bool NoAssetAtPath(string path) => string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));

    /// <summary> Potential results for the name validation process; </summary>
    public enum InvalidNameCondition {
        None,
        Empty,
        Overwrite,
        Symbol,
        Convention,
        Success
    }

    /// <summary>
    /// Validate a filename in terms of content, convention, and File I/O availability;
    /// </summary>
    /// <returns> True if the name is valid, false otherwise; </returns>
    public static InvalidNameCondition ValidateFilename(string path, string name) {
        if (string.IsNullOrWhiteSpace(name)) {
            return InvalidNameCondition.Empty;
        }
        if (!NoAssetAtPath(path)) {
            return InvalidNameCondition.Overwrite;
        }
        if (NameViolatesConvention(name)) {
            return InvalidNameCondition.Convention;
        }
        List<char> invalidChars = new List<char>(Path.GetInvalidFileNameChars());
        foreach (char character in name) {
            if (invalidChars.Contains(character)) {
                return InvalidNameCondition.Symbol;
            }
        }
        return InvalidNameCondition.None;
    }

    private static bool NameViolatesConvention(string fileName) {
        if (string.IsNullOrWhiteSpace(fileName)) return true;
        if (!char.IsUpper(fileName[0])) return true;
        if (fileName.Contains(" ")) return true;
        return false;
    }
}