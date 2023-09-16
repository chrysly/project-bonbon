using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

/// <summary>
/// Because the structure I devised for the MADGUI has been quite comfortable to work with, I'm building 
/// this tool using a similar structure. This tool has different needs and less complexity than the MAD,
/// thus I'm creating a new tool altogether rather than inheriting from the former's abstract base;
/// </summary>
namespace BonbonAssetManager {
    public abstract class BonBaseTool : ScriptableObject {

        protected BAMGUI MainGUI;

        public static T CreateTool<T>(BAMGUI mainGUI) where T : BonBaseTool {
            T tool = CreateInstance<T>();
            tool.MainGUI = mainGUI;
            return tool;
        }

        public virtual void ShowGUI() { }
    }

    public class BonbonManager : BonBaseTool {

        private BonbonHierarchy bonbonHierarchy;
        public string SelectedPath { get; private set; }
        private BonbonObject selectedBonbon;
        private List<BonbonObject> bonbonList => MainGUI.GlobalBonbonList;

        private int buttonSize = 50;

        private Vector2 bonbonScroll;
        private Vector2 recipeScroll;
        private Vector2 previewScroll;

        void OnEnable() {
            bonbonHierarchy = BaseHierarchy<BonbonObject>.CreateHierarchy<BonbonHierarchy>(this);
            bonbonHierarchy.OnPathSelection += BonbonManager_OnPathSelection;
        }

        public override void ShowGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                bonbonHierarchy.ShowGUI();
                using (new EditorGUILayout.VerticalScope()) {
                    MainGUI.DrawToolbar();

                    if (selectedBonbon != null) {
                        DrawBonbonGroup();
                        DrawRecipeDropSlots();
                        DrawRecipePreview(selectedBonbon);
                    } else {
                        EditorUtils.DrawScopeCenteredText("Select a Bonbon to edit it here;");
                    }
                }
            }
        }

        private void BonbonManager_OnPathSelection(string path) {
            SelectedPath = path;
            selectedBonbon = AssetDatabase.LoadAssetAtPath<BonbonObject>(path);
        }

        private void DrawBonbonGroup() {
            EditorUtils.WindowBoxLabel("Available Bonbons");

            using (var vscope = new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true))) {
                using (var scope = new EditorGUILayout.ScrollViewScope(bonbonScroll, GUILayout.Height(buttonSize * 2.5f))) {
                    bonbonScroll = scope.scrollPosition;
                    DrawWrappedSequence(500);
                }
            }
        }

        private void DrawWrappedSequence(int maxWidth) {

            int perRow = maxWidth / buttonSize;
            int rows = bonbonList.Count / perRow;
            for (int i = 0; i < rows + 1; i++) {
                using (new EditorGUILayout.HorizontalScope()) {
                    for (int j = i * perRow; j < Mathf.Min((i + 1) * perRow, bonbonList.Count); j++) {
                        DrawBonbonDragButton(bonbonList[j], new GUIContent(bonbonList[j].texture));
                    }
                }
            }
        }

        private void DrawRecipeDropSlots() {
            EditorUtils.WindowBoxLabel("Recipe");
            GUI.enabled = false;
            using (var scope = new EditorGUILayout.ScrollViewScope(recipeScroll, EditorStyles.textField, GUILayout.ExpandHeight(false))) {
                GUI.enabled = true;
                recipeScroll = scope.scrollPosition;

                using (new EditorGUILayout.HorizontalScope()) {
                    BonbonObject acceptedBonbon = DrawDragAcceptButton<BonbonObject>(GUILayout.Width(buttonSize * 2),
                                                                                     GUILayout.Height(buttonSize * 2));
                    UpdateBonbonRecipe(selectedBonbon, acceptedBonbon);

                    using (new EditorGUILayout.VerticalScope()) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            foreach (BonbonObject bonbon in selectedBonbon.recipe) {
                                if (bonbon) DrawBonbonDragButton(bonbon, new GUIContent(bonbon.texture));
                                else DrawEmptyBox();
                            }
                        } BonbonObject removeObject = DrawDragAcceptButton<BonbonObject>(GUILayout.Width(buttonSize * 4.6f),
                                                                                         GUILayout.Height(buttonSize * 0.8f));
                        if (removeObject != null) RemoveFromRecipe(selectedBonbon, removeObject);
                    }
                } 
            }
        }

        private void UpdateBonbonRecipe(BonbonObject recipeBonbon, BonbonObject ingredientBonbon) {
            recipeBonbon.AddRecipeSlot(ingredientBonbon);
        }

        private void RemoveFromRecipe(BonbonObject recipeBonbon, BonbonObject ingredientBonbon) {
            recipeBonbon.RemoveRecipeSlot(ingredientBonbon);
        }

        private void DrawEmptyBox() {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                GUILayout.Box("", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
            }
        }

        private void DrawBonbonDragButton<T>(T draggedObject, GUIContent content) where T : Object {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                Rect buttonRect = GUILayoutUtility.GetRect(buttonSize, buttonSize, GUILayout.ExpandWidth(false));
                if (buttonRect.Contains(Event.current.mousePosition)) {
                    bool mouseDown = Event.current.type == EventType.MouseDown;
                    bool leftClick = Event.current.button == 0;
                    if (mouseDown && leftClick) {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.StartDrag("Dragging");
                        DragAndDrop.objectReferences = new Object[] { draggedObject };
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                }
                GUI.Label(buttonRect, content, GUI.skin.button);
            }
        }

        private T DrawDragAcceptButton<T>(params GUILayoutOption[] options) where T : Object {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                T obj = null;
                obj = EditorGUILayout.ObjectField(obj, typeof(T), false, options) as T;
                return obj;
            }
        }

        private void DrawRecipePreview(BonbonObject bonbonObject) {
            EditorUtils.WindowBoxLabel("Recipe Preview");
            using (var scope = new EditorGUILayout.ScrollViewScope(previewScroll)) {
                previewScroll = scope.scrollPosition;
                DrawPreviewLevel(bonbonObject);
            }
        }

        private const float HANDLE_HEIGHT = 20;

        private void DrawPreviewLevel(BonbonObject bonbonObject) {
            if (bonbonObject == null) return;
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                var levelRect = DrawPaddedButton(bonbonObject);
                if (bonbonObject.recipe == null) bonbonObject.UpdateRecipeSize();
                /*float levelStart = 0;
                float levelWidth = 0;
                int buttonCount = 0;*/
                
                using (new EditorGUILayout.HorizontalScope()) {
                    foreach (BonbonObject ingredient in bonbonObject.recipe) {
                        if (ingredient) DrawPreviewLevel(ingredient);
                        //if (buttonCount == 0) levelStart = (rect.xMax - rect.x) / 2;
                        //levelWidth = (rect.xMax - rect.x) / 2 - levelStart;
                    }
                } /*var lineHeight = levelRect.y - 2;
                Handles.DrawLine(new Vector2(levelStart, lineHeight), new Vector2(levelStart + levelWidth, lineHeight));*/
            }
        }

        private Rect DrawPaddedButton(BonbonObject ingredient) {
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.ExpandWidth(false))) {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(ingredient.texture, UIStyles.CenteredLabel, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
                    GUILayout.FlexibleSpace();
                } GUILayout.FlexibleSpace();
                return new Rect();
            }
        }
    }

    public abstract class ActorManager : BonBaseTool {

    }
}