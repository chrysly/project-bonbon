using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;

namespace ModelAssetDatabase {
    public class HierarchyTabFolders : HierarchyTab {

        /// <summary>
        /// Process the asset list to contain all identified model-containing folders for the search function;
        /// </summary>
        protected override void ProcessFolderMap() {
            base.ProcessFolderMap();
            foreach (ModelAssetDatabase.FolderData folderData in folderMap.Values) {
                bool hasModels = folderData.models.Count > 0;
                if (hasModels) assetList.AddRange(folderData.subfolders);
            } assetList.Sort((name1, name2) => SearchingUtils.AlnumSort(name1, name2));
        }

        /// <summary>
        /// Draws a folder hierarchy on the left-hand interface;
        /// </summary>
        /// <param name="path"> Path to the root folder where the hierarchy begins;
        /// <br></br> Note: The root folder path will be included in the hierarchy; </param>
        public override void LoadData(string path) {

            using (new EditorGUILayout.HorizontalScope()) {
                bool hasFiles = folderMap[path].models.Count > 0;
                bool hasSubfolders = folderMap[path].subfolders.Count > 0;
                GUIContent folderContent;
                if (hasFiles) {
                    folderContent = new GUIContent("");
                } else folderContent = new GUIContent(path.IsolatePathEnd("\\/"),
                                                      EditorUtils.FetchIcon(folderMap[path].foldout ? "d_FolderOpened Icon" : "d_Folder Icon"));
                bool worthShowing = hasSubfolders && PerformAssetSearch(path);
                if (worthShowing) {
                    Rect rect = GUILayoutUtility.GetRect(0, 18, GUILayout.Width(13));
                    folderMap[path].foldout = EditorGUI.Foldout(rect, folderMap[path].foldout, folderContent,
                                                                       new GUIStyle(EditorStyles.foldout) { stretchWidth = false });
                } if (hasFiles) DrawPrefabFolderButton(path, worthShowing && folderMap[path].foldout);
            } EditorGUI.indentLevel++;

            if (folderMap[path].foldout) {
                foreach (string subfolder in folderMap[path].subfolders) {
                    LoadData(subfolder);
                    EditorGUI.indentLevel--;
                } 
            } 
        }

        protected override void DrawHierarchyButton(string path) => DrawPrefabFolderButton(path, false);

        /// <summary>
        /// Draws a button corresponding to a relevant folder in the hierarchy;
        /// </summary>
        /// <param name="path"> Path to the folder; </param>
        /// <param name="folderOpened"> Whether the foldout is active, so the Folder icon can reflect it; </param>
        private void DrawPrefabFolderButton(string path, bool folderOpened) {
            GUIStyle buttonStyle = path == HierarchyBuilder.SelectedAssetPath ? UIStyles.HFButtonSelected : UIStyles.HFButton;
            GUIContent folderContent = new GUIContent(path.IsolatePathEnd("\\/"), EditorUtils.FetchIcon(folderOpened ? "d_FolderOpened Icon" : "d_Folder Icon"));
            float width = EditorUtils.MeasureTextWidth(folderContent.text, GUI.skin.font);
            if (GUILayout.Button(folderContent, buttonStyle, GUILayout.Width(width + 34), GUILayout.Height(20))) HierarchyBuilder.SetSelectedAsset(path);
        }
    }
}