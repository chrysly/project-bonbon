using CJUtils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModelAssetDatabase {

    /// <summary> Component class of the Model Asset Database;
    /// <br></br> Organizes Prefab & Model Data in the GUI for DnD and categorization functions; </summary>
    public class PrefabOrganizer : BaseTool {

        #region | Tool Core |

        #region | Variables |

        /// <summary> Data relevant to a folder category, namely, name and file contents; </summary>
        private class FolderData {
            /// <summary> Parsed name of this category; </summary>
            public string name;
            /// <summary> ID list of all prefabs filed under this category;
            /// <br></br> Note that prefab categories is based on model category; </summary>
            public List<string> prefabIDs;
            /// <summary> ID list of all models filed under this category; </summary>
            public List<string> modelIDs;

            public FolderData(string name) {
                this.name = name;
                prefabIDs = new List<string>();
                modelIDs = new List<string>();
            }
        } /// <summary> The folder path of the folder selected in the GUI; </summary>
        public string SelectedFolder { get; private set; }

        /// <summary> A dictionary of dictionaries mapping a folder name to it's folder dictionary;
        private Dictionary<string, FolderData> folderMap;

        /// <summary> List containing all prefab IDs loaded; </summary>
        private List<string> globalPrefabIDs;

        /// <summary> List containing all model IDs loaded; </summary>
        private List<string> globalModelIDs;

        /// <summary> Data used to draw prefab cards; </summary>
        private class PrefabCardData {
            /// <summary> Root gameObject atop the prefab file hierarchy; </summary>
            public GameObject rootObject;
            /// <summary> Asset preview of the prefab; </summary>
            public Texture2D preview;

            public PrefabCardData(GameObject rootObject, Texture2D preview) {
                this.rootObject = rootObject;
                this.preview = preview;
            }
        } /// <summary> Dictionary mapping path keys to the corresponding Prefab Card Data; </summary>
        private Dictionary<string, PrefabCardData> prefabCardMap;

        /// <summary> Dictionary mapping path keys to the root game object of a Model; </summary>
        private Dictionary<string, GameObject> modelCardMap;

        /// <summary> Selection group that may be passed to the DragAndDrop class; </summary>
        private List<Object> dragSelectionGroup;
        /// <summary> Whether the mouse hovered over a button in the current frame; </summary>
        private bool mouseOverButton;

        /// <summary> Constraint for the number of prefabs shown on a search result; </summary>
        private bool searchAll;

        /// <summary> Sorting Modes for the Prefab Organizer; </summary>
        private enum PrefabSortMode {
            /// <summary> The prefabs cards will be drawn in alphabetical order; </summary>
            Name,
            /// <summary> The prefab cards will be grouped by parent model; </summary>
            Model
        } /// <summary> Sorting Mode selected on the Prefab Organizer GUI; </summary>
        private PrefabSortMode sortMode;

        /// <summary> Search String obtained from the Prefab Organizer Search Bar; </summary>
        private string searchString;

        /// <summary> List mapping the name of each prefab to their IDs; </summary>
        /// <br></br> 1st string = Prefab Name; 2nd string = Prefab ID; 3rd string = Model ID;
        private List<(string, string)> prefabNameMapList;
        /// <summary> List holding IDs filtered using the current Search String, if any; </summary>
        private List<string> searchResultPrefabList;

        #endregion

        #region | Initialization & Cleanup |

        /// <summary>
        /// Get the tool ready to go!
        /// </summary>
        protected override void InitializeData() {
            prefabCardMap = new Dictionary<string, PrefabCardData>();
            modelCardMap = new Dictionary<string, GameObject>();
            globalPrefabIDs = new List<string>();
            globalModelIDs = new List<string>();
            BuildFolderMap();
            SetSearchScope(true);
        }

        /// <summary>
        /// The tool must be re-initialized upon reselection to prevent errors;
        /// </summary>
        public override void RefreshData() {
            base.RefreshData();
            InitializeData();
        }

        /// <summary>
        /// Unloads all static data contained in the tool;
        /// <br></br> There's none at the moment :|
        /// </summary>
        public override void FlushData() { }

        #endregion

        /// <summary>
        /// Creates a map of folders path and the data they contain that's relevant to the tool;
        /// </summary>
        private void BuildFolderMap() {
            Dictionary<string, ModelAssetDatabase.FolderData> extFolderMap = ModelAssetDatabase.FolderMap;
            if (extFolderMap == null) extFolderMap = ModelAssetDatabase.BuildFolderMap(ModelAssetDatabase.RootAssetPath);
            folderMap = new Dictionary<string, FolderData>();
            foreach (KeyValuePair<string, ModelAssetDatabase.FolderData> kvp in extFolderMap) {
                folderMap[kvp.Key] = new FolderData(kvp.Value.name);
                foreach (string modelPath in kvp.Value.models) {
                    string modelID = AssetDatabase.AssetPathToGUID(modelPath);
                    folderMap[kvp.Key].modelIDs.Add(modelID);
                    globalModelIDs.Add(modelID);
                }
                foreach (string modelID in folderMap[kvp.Key].modelIDs) {
                    List<string> prefabIDList = ModelAssetDatabase.ModelDataDict[modelID].prefabIDList;
                    folderMap[kvp.Key].prefabIDs.AddRange(prefabIDList);
                    globalPrefabIDs.AddRange(prefabIDList);
                }
                LoadFolderData(kvp.Key);
            }
        }

        /// <summary>
        /// Sets the selected folder path;
        /// </summary>
        /// <param name="path"> Folder path to select; </param>
        public override void SetSelectedAsset(string path) {
            base.SetSelectedAsset(path);
            if (!searchAll) SetSearchScope(!searchAll);
            dragSelectionGroup = new List<Object>();
            SelectedFolder = path;
        }

        /// <summary>
        /// Loads all relevant static data;
        /// </summary>
        /// <param name="path"> Category path to load; </param>
        private void LoadFolderData(string path) {
            foreach (string prefabID in folderMap[path].prefabIDs) {
                if (!prefabCardMap.ContainsKey(prefabID)) {
                    var prefabData = ModelAssetDatabase.PrefabDataDict[prefabID];
                    string assetPath = prefabData.path;
                    GameObject rootObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    Texture2D preview = AssetPreview.GetAssetPreview(rootObject);
                    while (preview == null) {
                        preview = AssetPreview.GetAssetPreview(rootObject);
                    } prefabCardMap[prefabID] = new PrefabCardData(rootObject, preview);
                }
            } dragSelectionGroup = new List<Object>();
            foreach (string modelID in folderMap[path].modelIDs) {
                string modelPath = ModelAssetDatabase.ModelDataDict[modelID].path;
                modelCardMap[modelID] = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            }
        }

        /// <summary>
        /// Sets the current prefab sort mode;
        /// <br></br> Honestly, why did I even write this method;
        /// </summary>
        /// <param name="sortMode"> New sort mode; </param>
        private void SetSortMode(PrefabSortMode sortMode) {
            /// Originally I did a thing or two here, but turns out I don't want to anymore;
            /// So I'll leave this method here anyways <3
            this.sortMode = sortMode;
        }

        /// <summary>
        /// Establish the search scope for potential searches;
        /// </summary>
        /// <param name="searchAll"> Whether the created scope should include all available prefabs or just the active folder; </param>
        private void SetSearchScope(bool searchAll) {
            prefabNameMapList = new List<(string, string)>();
            if (searchAll) {
                foreach (KeyValuePair<string, FolderData> kvp in folderMap) {
                    ExtractSearchNames(kvp.Value.prefabIDs, ref prefabNameMapList);
                }
            } else {
                ExtractSearchNames(folderMap[SelectedFolder].prefabIDs, ref prefabNameMapList);
            } this.searchAll = searchAll;
        }

        /// <summary>
        /// Append the names of a number of prefab IDs to a search list;
        /// </summary>
        /// <param name="prefabIDs"> The prefab IDs whose names must be processed; </param>
        /// <param name="targetList"> The list to add the extracted names to; </param>
        private void ExtractSearchNames(List<string> prefabIDs, ref List<(string, string)> targetList) {
            foreach (string prefabID in prefabIDs) {
                var prefabData = ModelAssetDatabase.PrefabDataDict[prefabID];
                targetList.Add((prefabData.name, prefabID));
            }
        }

        /// <summary>
        /// Updates the current Search String to a new value;
        /// <br></br> Discards the previous results, if any;
        /// </summary>
        /// <param name="searchString"> New search string; </param>
        public void SetSearchString(string searchString) {
            searchResultPrefabList = null;
            this.searchString = searchString;
        }

        /// <summary>
        /// Process the Search Results upon request;
        /// </summary>
        private void ProcessSearchList() {
            List<(string, string)> processList = prefabNameMapList.FindAll((tuple) => tuple.Item1.Contains(searchString));
            searchResultPrefabList = new List<string>();
            foreach ((string, string) ssTuple in processList) searchResultPrefabList.Add(ssTuple.Item2);
        }

        #endregion

        #region | Tool GUI |

        private static Vector2 modelSortScroll;

        /// <summary>
        /// Draws the toolbar for the Prefab Organizer;
        /// </summary>
        public override void DrawToolbar() {
            if (GUILayout.Button("Show All", EditorStyles.toolbarButton, GUILayout.MinWidth(80))) SelectedFolder = null;
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbarButton)) {
                GUILayout.Label("Sort By:", new GUIStyle(UIStyles.ToolbarText) { margin = new RectOffset(0, 20, 1, 0) }, GUILayout.Width(80));
            } if (GUILayout.Button("Name", sortMode == PrefabSortMode.Name
                                           ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton, GUILayout.MinWidth(105), GUILayout.ExpandWidth(true))) {
                SetSortMode(PrefabSortMode.Name);
            } if (GUILayout.Button("Model", sortMode == PrefabSortMode.Model
                                            ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton, GUILayout.MinWidth(105), GUILayout.ExpandWidth(true))) {
                SetSortMode(PrefabSortMode.Model);
            } GUILayout.FlexibleSpace();
            string impendingSearch = EditorGUILayout.TextField(searchString, EditorStyles.toolbarSearchField, GUILayout.MinWidth(175));
            if (searchString != impendingSearch) SetSearchString(impendingSearch);
            GUILayout.Label("in");
            if (GUILayout.Button(searchAll ? "All" : "Folder", EditorStyles.toolbarButton, GUILayout.MinWidth(50), GUILayout.ExpandWidth(true))) {
                SetSearchScope(!searchAll);
                searchResultPrefabList = null;
            }
        }

        /// <summary>
        /// Entry point to display the Prefab Organizer Data;
        /// </summary>
        public override void ShowGUI() {

            if (SelectedFolder == null) SetSelectedAsset(ModelAssetDatabase.RootAssetPath);

            bool searchStringInactive = string.IsNullOrWhiteSpace(searchString);
            string folderName = SelectedFolder.IsolatePathEnd("\\/");
            EditorUtils.WindowBoxLabel(searchStringInactive ? folderName 
                                           : "Search Results in \"" + ( searchAll ? "All" : folderName) + "\"");

            using (new EditorGUILayout.HorizontalScope()) {
                if (searchStringInactive) {
                    bool isRoot = SelectedFolder == ModelAssetDatabase.RootAssetPath;
                    switch (sortMode) {
                        case PrefabSortMode.Name:
                            if (isRoot) {
                                using (new EditorGUILayout.VerticalScope()) {
                                    DrawPrefabCards(globalPrefabIDs, "There are no prefab variants in this section of the project;");
                                }
                            } else {
                                DrawPrefabCards(folderMap[SelectedFolder].prefabIDs,
                                                "There are no prefabs under this folder;");
                            } break;
                        case PrefabSortMode.Model:
                            using (var view = new EditorGUILayout.ScrollViewScope(modelSortScroll,
                                                                                    GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true))) {
                                modelSortScroll = view.scrollPosition;
                                using (new EditorGUILayout.HorizontalScope()) {
                                    if (isRoot) foreach (string modelID in globalModelIDs) DrawModelColumn(modelID);
                                    else foreach (string modelID in folderMap[SelectedFolder].modelIDs) DrawModelColumn(modelID);
                                }
                            } break;
                    }
                } else DrawSearchCards();
            } 
        }

        /// <summary>
        /// Draws prefab cards whose name matches the Search Query in any way;
        /// </summary>
        private void DrawSearchCards() {
            if (searchResultPrefabList == null) ProcessSearchList();
            DrawPrefabCards(searchResultPrefabList, "No matching results were found;");
        }

        /// <summary>
        /// Draws all prefab cards as stipulated by the default sort mode;
        /// </summary>
        private void DrawPrefabCards(List<string> prefabIDs, string noPrefabsMessage) {
            if (prefabIDs.Count == 0) {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope()) {
                    EditorGUILayout.Separator(); EditorGUILayout.Separator();
                    GUILayout.Label(noPrefabsMessage, UIStyles.CenteredLabelBold);
                }
                GUILayout.FlexibleSpace();
            } else {
                int amountPerRow = Mathf.RoundToInt((MainGUI.position.xMax - MainGUI.position.xMin - 200) / 220);
                bool validCount = prefabIDs.Count >= amountPerRow;
                if (validCount) GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope()) {
                    for (int i = 0; i < Mathf.CeilToInt((float) prefabIDs.Count / amountPerRow); i++) {
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                            for (int j = i * amountPerRow; j < Mathf.Min((i + 1) * amountPerRow, prefabIDs.Count); j++) {
                                DrawPrefabCard(prefabIDs[j]);
                            }
                        }
                    } DeselectionCheck();
                } if (validCount) GUILayout.FlexibleSpace();
            }
        }

        /// <summary>
        /// Draws a Prefab Card containing buttons;
        /// </summary>
        /// <param name="prefabID"> ID of the prefab to draw the card for; </param>
        private void DrawPrefabCard(string prefabID) {
            PrefabCardData data = prefabCardMap[prefabID];
            bool objectInSelection = dragSelectionGroup.Contains(data.rootObject);
            if (objectInSelection) GUI.color = UIColors.DarkBlue;
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox, GUILayout.Width(200), GUILayout.Height(60))) {
                GUI.color = Color.white;
                DrawDragAndDropPreview(prefabID, objectInSelection);

                using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                    GUILayout.FlexibleSpace();
                    //GUIStyle labelStyle = new GUIStyle(UIStyles.CenteredLabel) { clipping = TextClipping.Clip };
                    EditorUtils.WindowBoxLabel(ModelAssetDatabase.PrefabDataDict[prefabID].name, GUILayout.Width(90));
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                        if (GUILayout.Button("Open Library", GUILayout.MaxHeight(24))) {
                            MainGUI.SwitchToLibrary(ModelAssetDatabase.PrefabDataDict[prefabID].modelID);
                        } using (new EditorGUILayout.HorizontalScope()) {
                            if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_PrefabModel On Icon")), GUILayout.MaxWidth(45), GUILayout.MaxHeight(24))) {
                                EditorUtils.PingObject(AssetImporter.GetAtPath(ModelAssetDatabase
                                                                                    .ModelDataDict[ModelAssetDatabase
                                                                                    .PrefabDataDict[prefabID].modelID].path)); ;
                            } if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_PrefabVariant On Icon")), GUILayout.MaxWidth(45), GUILayout.MaxHeight(24))) {
                                EditorUtils.PingObject(data.rootObject);
                            }
                        }
                    } GUILayout.FlexibleSpace();
                }
            }
        }

        /// <summary>
        /// Creates a Drag & Drop button for a given prefab;
        /// </summary>
        /// <param name="prefabID"> ID of the prefab; </param>
        /// <param name="objectInSelection"> Whether the prefab is in the current Drag & Drop Selection Group; </param>
        private void DrawDragAndDropPreview(string prefabID, bool objectInSelection) {
            PrefabCardData data = prefabCardMap[prefabID];
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                Rect buttonRect = GUILayoutUtility.GetRect(80, 80, GUILayout.ExpandWidth(false));
                if (buttonRect.Contains(Event.current.mousePosition)) {
                    mouseOverButton = true;
                    bool mouseDown = Event.current.type == EventType.MouseDown;
                    bool mouseDrag = Event.current.type == EventType.MouseDrag;
                    bool leftClick = Event.current.button == 0;
                    bool rightClick = Event.current.button == 1;
                    if (Event.current.shift) {
                        if (objectInSelection) {
                            if (mouseDown || (mouseDrag && rightClick)) dragSelectionGroup.Remove(data.rootObject);
                        } else if (mouseDown || (mouseDrag && leftClick)) dragSelectionGroup.Add(data.rootObject);
                    } else if (mouseDown && leftClick) {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.StartDrag("Dragging");
                        DragAndDrop.objectReferences = dragSelectionGroup.Count > 1
                                                       ? dragSelectionGroup.ToArray() : new Object[] { data.rootObject };
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                } GUI.Label(buttonRect, prefabCardMap[prefabID].preview, GUI.skin.button);
            }
        }

        private void DrawDragAndDropPreviewModel(string modelID) {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                Rect buttonRect = GUILayoutUtility.GetRect(50, 50, GUILayout.ExpandWidth(false));
                if (buttonRect.Contains(Event.current.mousePosition)) {
                    bool mouseDown = Event.current.type == EventType.MouseDown;
                    bool leftClick = Event.current.button == 0;
                    if (mouseDown && leftClick) {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.StartDrag("Dragging");
                        DragAndDrop.objectReferences = new Object[] { modelCardMap[modelID] };
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                } GUI.Label(buttonRect, new GUIContent(EditorUtils.FetchIcon("d_PrefabModel Icon")), GUI.skin.button);
            }
        }

        /// <summary>
        /// Whether a Drag & Drop Selection Group wipe may happen at the end of the frame;
        /// </summary>
        private void DeselectionCheck() {
            if (!mouseOverButton && Event.current.type == EventType.MouseDown && !Event.current.shift
                && Event.current.button == 0 && dragSelectionGroup.Count > 0) dragSelectionGroup.Clear();
            mouseOverButton = false;
        }

        /// <summary>
        /// Draw a List of Prefab Cards under a Model Header as stipulated by the model sort mode;
        /// </summary>
        /// <param name="modelID"> ID of the model owning the column; </param>
        private void DrawModelColumn(string modelID) {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(210))) {
                using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox, GUILayout.Width(210))) {
                        DrawDragAndDropPreviewModel(modelID);
                        using (new EditorGUILayout.VerticalScope()) {
                            EditorUtils.WindowBoxLabel(ModelAssetDatabase.ModelDataDict[modelID].name, GUILayout.Height(14));
                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.Height(24))) {
                                GUI.color = UIColors.Blue;
                                if (GUILayout.Button("Reimport", EditorStyles.miniButton)) {
                                    ModelImporter model = AssetImporter.GetAtPath(ModelAssetDatabase.ModelDataDict[modelID].path) as ModelImporter;
                                    AssetPreprocessorGUI.LibraryReimport(model);
                                } GUI.color = Color.white;
                            }
                        }
                    }
                } GUIStyle boxStyle = new GUIStyle(GUI.skin.box) {
                    margin = new RectOffset(), stretchWidth = true, stretchHeight = true };
                using (new EditorGUILayout.VerticalScope(boxStyle)) {
                    List<string> modelIDList = ModelAssetDatabase.ModelDataDict[modelID].prefabIDList;
                    if (modelIDList.Count == 0) {
                        EditorGUILayout.Separator();
                        using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.FlexibleSpace();
                            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                GUILayout.Label("No Prefab Variants", UIStyles.CenteredLabelBold);
                                if (GUILayout.Button("Open Library")) MainGUI.SwitchToLibrary(modelID);
                            } GUILayout.FlexibleSpace();
                        }
                    } foreach (string prefabID in modelIDList) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.FlexibleSpace();
                            DrawPrefabCard(prefabID);
                            GUILayout.FlexibleSpace();
                        }
                    }
                }
            }
        }

        #endregion
    }
}