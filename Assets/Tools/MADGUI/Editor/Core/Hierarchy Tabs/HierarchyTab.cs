using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using ModelAssetDatabase.MADUtils;

namespace ModelAssetDatabase {

    /// <summary>
    /// Base class for all 'tabbed' variants of the Hierarchy Builder;
    /// </summary>
    public abstract class HierarchyTab : BaseTab {

        #region | Base Tab Overrides |

        /// <summary> The parent Hierarchy Builder of this tab; </summary>
        protected HierarchyBuilder HierarchyBuilder;

        /// <summary> A getter for the folder map contained in the Model Asset Database; </summary>
        protected Dictionary<string, ModelAssetDatabase.FolderData> folderMap { get { return ModelAssetDatabase.FolderMap; } }

        /// <summary> A list of the assets contemplated by this tab; </summary>
        protected List<string> assetList;

        protected override void InitializeData() {
            if (Tool is HierarchyBuilder) {
                HierarchyBuilder = Tool as HierarchyBuilder;
            } else Debug.LogError(INVALID_MANAGER);
            ProcessFolderMap();
        }

        /// <summary>
        /// Override to draw the results of a search query;
        /// </summary>
        /// <param name="searchString"> Search string currently passed in the GUI; </param>
        public void DrawSearchQuery(string searchString) {
            List<string> resultList = SearchingUtils.GetSearchQuery(searchString, assetList);
            foreach (string path in resultList) DrawHierarchyButton(path);
        }

        /// <summary>
        /// Override to draw a button for the given Hierarchy Tab;
        /// </summary>
        protected abstract void DrawHierarchyButton(string path);

        /// <summary>
        /// Override to process the folder map for relevant search names;
        /// </summary>
        protected virtual void ProcessFolderMap() => assetList = new List<string>();

        #endregion

        #region | Helpers |

        /// <summary>
        /// Perform a search through subfolders to identify any folder containing models or materials;
        /// <br></br> Used to determine whether a folder/foldout is worth drawing;
        /// </summary>
        /// <param name="path"> Path to begin the subfolder search in; </param>
        /// <param name="searchModels"> Whether to search for models or materials; </param>
        /// <returns> Whether a folder containing models or materials was found; </returns>
        protected bool PerformAssetSearch(string path, bool searchModels = true) {
            foreach (string folder in folderMap[path].subfolders) {
                int countParameter = searchModels ? folderMap[folder].models.Count : folderMap[folder].materials.Count;
                if (countParameter > 0) return true;
                else if (folderMap[folder].subfolders.Count > 0) return PerformAssetSearch(folder);
            } return false;
        }

        /// <summary>
        /// Draw a conditional foldout based on folder data;
        /// </summary>
        /// <param name="path"> Path to the foldout folder to draw; </param>
        /// <param name="data"> Data pertaining to the folder to draw; </param>
        /// <param name="marginCondition"> Whether the folder will fold out to show materials; </param>
        /// <returns></returns>
        protected bool DrawConditionalFoldout(string path, ModelAssetDatabase.FolderData data, bool marginCondition) {
            GUIContent foldoutContent = new GUIContent(" " + path.IsolatePathEnd("/\\"),
                                            EditorUtils.FetchIcon(data.foldout ? "d_FolderOpened Icon" : "d_Folder Icon"));
            float width = EditorUtils.MeasureTextWidth(data.name, GUI.skin.font);
            return EditorGUILayout.Foldout(data.foldout, foldoutContent,
                                                         new GUIStyle(EditorStyles.foldoutHeader) {
                                                             fixedWidth = width + 48,
                                                             fixedHeight = 19,
                                                             margin = new RectOffset(0, 0, 0,
                                                             marginCondition && data.foldout ? 2 : 0)
                                                         });
        }

        #endregion
    }
}