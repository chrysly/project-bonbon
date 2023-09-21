using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;

namespace ModelAssetDatabase {
    public class HierarchyTabModels : HierarchyTab {

        /// Asset list contains all identified models for the search function;

        protected override void ProcessFolderMap() {
            base.ProcessFolderMap();
            foreach (ModelAssetDatabase.FolderData folderData in folderMap.Values) {
                assetList.AddRange(folderData.models);
            } assetList.Sort((name1, name2) => SearchingUtils.AlnumSort(name1, name2));
        }

        /// <summary>
        /// Draws a folder + model hierarchy on the left-hand interface;
        /// </summary>
        /// <param name="path"> Path to the root folder where the hierarchy begins;
        /// <br></br> Note: The root folder path will be included in the hierarchy; </param>
        public override void LoadData(string path) {
            ModelAssetDatabase.FolderData data = folderMap[path];
            bool hasModels = data.models.Count > 0;
            if (hasModels || (data.subfolders.Count > 0 && PerformAssetSearch(path))) {
                data.foldout = DrawConditionalFoldout(path, data, hasModels);
            } EditorGUI.indentLevel++;

            if (folderMap[path].foldout) {
                foreach (string subfolder in folderMap[path].subfolders) {
                    LoadData(subfolder);
                    EditorGUI.indentLevel--;
                } foreach (string file in folderMap[path].models) DrawHierarchyButton(file);
            }
        }

        /// <summary>
        /// Draws a button corresponding to model file in the hierarchy;
        /// </summary>
        /// <param name="path"> Path to the file; </param>
        protected override void DrawHierarchyButton(string path) {
            bool selected = path == HierarchyBuilder.SelectedAssetPath;
            GUIStyle buttonStyle = selected ? UIStyles.HButtonSelected : UIStyles.HButton;
            string extension = path.IsolatePathEnd(".");
            string fileName = path.IsolatePathEnd("\\/").Replace(extension, extension.ToUpper());
            float width = EditorUtils.MeasureTextWidth(fileName, GUI.skin.font);
            var data = ExtManager.FetchExtData(AssetDatabase.AssetPathToGUID(path));
            GUIContent modelContent;
            Texture2D icon;
            if (data != null) {
                if (selected) icon = EditorUtils.FetchIcon(data.isModel ? "d_PrefabModel Icon" : "AvatarSelector");
                else icon = EditorUtils.FetchIcon(data.isModel ? "d_PrefabModel On Icon" : "AvatarMask On Icon");
            } else {
                if (selected) icon = EditorUtils.FetchIcon("d_ScriptableObject Icon");
                else icon = EditorUtils.FetchIcon("d_ScriptableObject On Icon");
            } modelContent = new GUIContent(fileName, icon);
            if (GUILayout.Button(modelContent, buttonStyle, 
                                 GUILayout.Width(width + 29), GUILayout.Height(20))) HierarchyBuilder.SetSelectedAsset(path);
        }

    }
}