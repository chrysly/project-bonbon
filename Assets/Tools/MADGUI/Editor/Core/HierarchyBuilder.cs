using UnityEditor;
using CJUtils;

namespace ModelAssetDatabase {

    /// <summary> Core class of the Model Asset Database;
    /// <br></br> Reads the folder directory and generates interactable Hierarchy Previews; </summary>
    public class HierarchyBuilder : BaseTool {

        #region | Variables |

        private HierarchyTab[] tabs;

        /// <summary> Asset path selected in the hierarchy; </summary>
        public string SelectedAssetPath { get { return MainGUI.SelectedAssetPath; } }

        /// GUI variables;
        private string searchString;

        #endregion

        #region | Initialization & Cleanup |

        /// <summary>
        /// Loads the data required to generate Hierarchy Previews;
        /// </summary>
        protected override void InitializeData() {
            tabs = new HierarchyTab[] {
                BaseTab.CreateTab<HierarchyTabModels>(this),
                BaseTab.CreateTab<HierarchyTabFolders>(this),
                BaseTab.CreateTab<HierarchyTabMaterials>(this),
            };
        }

        /// <summary>
        /// Propagates the asset selection to the GUI, which in turn propagates it to the Active Tool;
        /// </summary>
        /// <param name="path"> Path of the asset to select; </param>
        public override void SetSelectedAsset(string path) => MainGUI.SetSelectedAsset(path);

        /// <summary>
        /// There's nothing to flush in the Hierarchy so far;
        /// </summary>
        public override void FlushData() { }

        #endregion

        #region | Hierarchy GUI |

        /// <summary>
        /// Draws the Search Bar atop the Hierarchy Preview;
        /// </summary>
        public override void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(UIStyles.PaddedToolbar)) {
                searchString = EditorGUILayout.TextField(searchString, EditorStyles.toolbarSearchField);
            }
        }

        /// <summary>
        /// Show a Hierarchy Preview applicable to the current tool;
        /// </summary>
        public override void ShowGUI() {
            HierarchyTab activeTab = tabs[(int) MainGUI.ActiveTool];
            if (string.IsNullOrWhiteSpace(searchString)) {
                activeTab.LoadData(ModelAssetDatabase.RootAssetPath);
            } else activeTab.DrawSearchQuery(searchString);
        }

        #endregion
    }
}