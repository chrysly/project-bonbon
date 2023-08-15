using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using MainGUI = ModelAssetLibraryGUI;
using ModelReader = ModelAssetLibraryModelReader;
using static ModelAssetLibraryPrefabOrganizer;

/// <summary> 
/// Displays the interface of the Model Asset Library Reader; </summary>
/// </summary>
public static class ModelAssetLibraryPrefabOrganizerGUI {

    private static Vector2 modelSortScroll;

    /// <summary>
    /// Draws the toolbar for the Prefab Organizer;
    /// </summary>
    public static void DrawPrefabOrganizerToolbar() {
        GUI.enabled = false;
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbarButton)) {
            if (SelectedCategory != null) GUI.enabled = true;
            GUILayout.Label("Sort By:", new GUIStyle(UIStyles.ToolbarText) { margin = new RectOffset(0, 20, 1, 0) }, GUILayout.Width(110));
        } if (GUILayout.Button("Name", SortMode == PrefabSortMode.Name
                                       ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton, GUILayout.MinWidth(140), GUILayout.ExpandWidth(true))) {
            SetSortMode(PrefabSortMode.Name);
        } if (GUILayout.Button("Model", SortMode == PrefabSortMode.Model
                                        ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton, GUILayout.MinWidth(140), GUILayout.ExpandWidth(true))) {
            SetSortMode(PrefabSortMode.Model);
        } GUILayout.FlexibleSpace();
        string impendingSearch = EditorGUILayout.TextField(SearchString, EditorStyles.toolbarSearchField, GUILayout.MinWidth(140));
        if (SearchString != impendingSearch) SetSearchString(impendingSearch);
        GUI.enabled = true;
    }

    /// <summary>
    /// Entry point to display the Prefab Organizer Data;
    /// </summary>
    public static void ShowSelectedCategory() {

        if (SelectedCategory == null) {
            EditorUtils.DrawScopeCenteredText("Prefabs stored in the Selected Category will be displayed here;");
            return;
        }

        using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
            GUILayout.Label(SelectedCategory.IsolatePathEnd("\\/"), UIStyles.CenteredLabelBold);
        }

        using (new EditorGUILayout.HorizontalScope()) {
            if (string.IsNullOrWhiteSpace(SearchString)) {
                switch (SortMode) {
                    case PrefabSortMode.Name:
                        DrawPrefabCards(CategoryMap[SelectedCategory].prefabIDs, 
                                        "There are no prefabs under this category;");
                        break;
                    case PrefabSortMode.Model:
                        using (var view = new EditorGUILayout.ScrollViewScope(modelSortScroll,
                                                                                GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true))) {
                            modelSortScroll = view.scrollPosition;
                            using (new EditorGUILayout.HorizontalScope()) {
                                foreach (string modelID in CategoryMap[SelectedCategory].modelIDs) DrawModelColumn(modelID);
                            }
                        } break;
                }
            } else DrawSearchCards();
        } 
    }

    /// <summary>
    /// Draws prefab cards whose name matches the Search Query in any way;
    /// </summary>
    private static void DrawSearchCards() {
        if (SearchResultList == null) ProcessSearchList();
        DrawPrefabCards(SearchResultList, "No matching results were found;");
    }

    /// <summary>
    /// Draws all prefab cards as stipulated by the default sort mode;
    /// </summary>
    private static void DrawPrefabCards(List<string> prefabIDs, string noPrefabsMessage) {
        if (prefabIDs.Count == 0) {
            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.VerticalScope()) {
                EditorGUILayout.Separator(); EditorGUILayout.Separator();
                GUILayout.Label(noPrefabsMessage, UIStyles.CenteredLabelBold);
            }
            GUILayout.FlexibleSpace();
        } else {
            bool validCount = prefabIDs.Count >= 3;
            if (validCount) GUILayout.FlexibleSpace();
            using (new EditorGUILayout.VerticalScope()) {
                int amountPerRow = 3;
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
    private static void DrawPrefabCard(string prefabID) {
        PrefabCardData data = PrefabCardMap[prefabID];
        bool objectInSelection = DragSelectionGroup.Contains(data.rootObject);
        if (objectInSelection) GUI.color = UIColors.DarkBlue;
        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox, GUILayout.MaxWidth(200), GUILayout.Height(60))) {
            GUI.color = Color.white;
            DrawDragAndDropPreview(prefabID, objectInSelection);

            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.ExpandHeight(true))) {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    string name = ModelAssetLibrary.PrefabDataDict[prefabID].name;
                    GUILayout.Label(name, UIStyles.CenteredLabelBold);
                } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                    if (GUILayout.Button("Open Library", GUILayout.MaxHeight(24))) {
                        SwitchToLibrary(ModelAssetLibrary.PrefabDataDict[prefabID].modelID);
                    } using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d_PrefabModel On Icon")), GUILayout.MaxWidth(45), GUILayout.MaxHeight(24))) {
                            EditorUtils.PingObject(AssetImporter.GetAtPath(ModelAssetLibrary
                                                                                .ModelDataDict[ModelAssetLibrary
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
    private static void DrawDragAndDropPreview(string prefabID, bool objectInSelection) {
        PrefabCardData data = PrefabCardMap[prefabID];
        using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
            Rect buttonRect = GUILayoutUtility.GetRect(80, 80, GUILayout.ExpandWidth(false));
            if (buttonRect.Contains(Event.current.mousePosition)) {
                MouseOverButton = true;
                bool mouseDown = Event.current.type == EventType.MouseDown;
                bool mouseDrag = Event.current.type == EventType.MouseDrag;
                bool leftClick = Event.current.button == 0;
                bool rightClick = Event.current.button == 1;
                if (Event.current.shift) {
                    if (objectInSelection) {
                        if (mouseDown || (mouseDrag && rightClick)) DragSelectionGroup.Remove(data.rootObject);
                    } else if (mouseDown || (mouseDrag && leftClick)) DragSelectionGroup.Add(data.rootObject);
                } else if (mouseDown && leftClick) {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.StartDrag("Dragging");
                    DragAndDrop.objectReferences = DragSelectionGroup.Count > 1
                                                   ? DragSelectionGroup.ToArray() : new Object[] { data.rootObject };
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                }
            } GUI.Label(buttonRect, PrefabCardMap[prefabID].preview, GUI.skin.button);
        }
    }

    /// <summary>
    /// Whether a Drag & Drop Selection Group wipe may happen at the end of the frame;
    /// </summary>
    private static void DeselectionCheck() {
        if (!MouseOverButton && Event.current.type == EventType.MouseDown && !Event.current.shift
            && Event.current.button == 0 && DragSelectionGroup.Count > 0) DragSelectionGroup.Clear();
        MouseOverButton = false;
    }

    /// <summary>
    /// Draw a List of Prefab Cards under a Model Header as stipulated by the model sort mode;
    /// </summary>
    /// <param name="modelID"> ID of the model owning the column; </param>
    private static void DrawModelColumn(string modelID) {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(210))) {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox, GUILayout.Width(210))) {
                    GUILayout.Label(new GUIContent(EditorUtils.FetchIcon("d_PrefabModel Icon")),
                                    GUI.skin.button, GUILayout.Width(50), GUILayout.Height(50));
                    using (new EditorGUILayout.VerticalScope()) {
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            GUILayout.Label(ModelAssetLibrary.ModelDataDict[modelID].name, UIStyles.CenteredLabelBold, GUILayout.Height(14));
                        } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.Height(24))) {
                            GUI.color = UIColors.Blue;
                            if (GUILayout.Button("Reimport", EditorStyles.miniButton)) {
                                ModelImporter model = AssetImporter.GetAtPath(ModelAssetLibrary.ModelDataDict[modelID].path) as ModelImporter;
                                ModelAssetLibraryAssetPreprocessorGUI.LibraryReimport(model);
                            } GUI.color = Color.white;
                        }
                    }
                }
            } GUIStyle boxStyle = new GUIStyle(GUI.skin.box) {
                margin = new RectOffset(), stretchWidth = true, stretchHeight = true };
            using (new EditorGUILayout.VerticalScope(boxStyle)) {
                List<string> modelIDList = ModelAssetLibrary.ModelDataDict[modelID].prefabIDList;
                if (modelIDList.Count == 0) {
                    EditorGUILayout.Separator();
                    using (new EditorGUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                            GUILayout.Label("No Prefab Variants", UIStyles.CenteredLabelBold);
                            if (GUILayout.Button("Open Library")) SwitchToLibrary(modelID);
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

    /// <summary>
    /// Switch to the Prefabs Section of the Model Reader on the corresponding model;
    /// </summary>
    /// <param name="modelID"> ID of the model used to redirect the GUI; </param>
    private static void SwitchToLibrary(string modelID) {
        MainGUI.SwitchActiveTool(MainGUI.ToolMode.ModelReader);
        ModelReader.SetSelectedModel(ModelAssetLibrary.ModelDataDict[modelID].path);
        ModelReader.SetSelectedSection(ModelReader.SectionType.Prefabs);
        GUIUtility.ExitGUI();
    }
}