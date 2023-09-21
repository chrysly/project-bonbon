using System.Collections.Generic;
using UnityEditor;

public static class StringUtils {

    /// <summary>
    /// Insert spaces after uppercase letters (excluding start);
    /// </summary>
    /// <param name="str"> String to manipulate; </param>
    /// <returns> String with spaces after uppercase letters; </returns>
    public static string CamelSpace(this string str) {
        var nStr = str;
        int spaceCount = 0;
        for (int i = 1; i < str.Length; i++) {
            if (char.IsUpper(str[i])) {
                nStr = str.Insert(i + spaceCount, " ");
                spaceCount++;
            }
        } return nStr;
    }

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
        } var nArr = nStr.ToCharArray(); System.Array.Reverse(nArr);
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
    /// <param name="includeDefaultName"> Whether to replace the file name with the default Model name; </param>
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
    public static string ToPrefabPathWithGUID(this string modelPath, string guid) {
        string targetPath = modelPath.RemovePathEnd("\\/") + "/Prefabs";
        targetPath += "/" + AssetDatabase.GUIDToAssetPath(guid).IsolatePathEnd("\\/", true) + ".prefab";
        return targetPath;
    }
}
