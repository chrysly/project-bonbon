using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;

namespace ModelAssetDatabase {
    public class HierarchyTabMaterials : HierarchyTab {

        /// <summary>
        /// Process the asset list to contain all identfied materials for the search function;
        /// </summary>
        protected override void ProcessFolderMap() {
            base.ProcessFolderMap();
            foreach (ModelAssetDatabase.FolderData folderData in folderMap.Values) {
                assetList.AddRange(folderData.materials);
            } assetList.Sort((name1, name2) => SearchingUtils.AlnumSort(name1, name2));
        }

        public override void LoadData(string path) {
            ModelAssetDatabase.FolderData data = folderMap[path];
            bool hasMaterials = data.materials.Count > 0;
            if (hasMaterials || (data.subfolders.Count > 0 && PerformAssetSearch(path, false))) {
                folderMap[path].foldout = DrawConditionalFoldout(path, data, hasMaterials);
            } EditorGUI.indentLevel++;

            if (folderMap[path].foldout) {
                foreach (string subfolder in folderMap[path].subfolders) {
                    LoadData(subfolder);
                    EditorGUI.indentLevel--;
                } foreach (string materialPath in folderMap[path].materials) DrawHierarchyButton(materialPath);
            }
        }

        protected override void DrawHierarchyButton(string path) {
            bool selected = path == HierarchyBuilder.SelectedAssetPath;
            GUIStyle buttonStyle = selected ? UIStyles.HButtonSelected : UIStyles.HButton;
            string pathName = path.IsolatePathEnd("\\/").RemovePathEnd(".");
            float width = EditorUtils.MeasureTextWidth(pathName, GUI.skin.font);
            GUIContent materialContent = new GUIContent(pathName, 
                                                        EditorUtils.FetchIcon(selected ? "d_Material Icon" : "d_Material On Icon"));
            if (GUILayout.Button(materialContent, buttonStyle, GUILayout.Width(width + 29), GUILayout.Height(20))) HierarchyBuilder.SetSelectedAsset(path);
        }
    }
}