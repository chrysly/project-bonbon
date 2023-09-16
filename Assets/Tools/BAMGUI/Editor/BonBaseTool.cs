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

        private Vector2 recipeScroll;

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
                        DrawRecipeDropSlots();
                        DrawRecipePreview();
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

        private void DrawRecipeDropSlots() {
            EditorUtils.WindowBoxLabel("Recipe");
            GUI.enabled = false;
            using (var scope = new EditorGUILayout.ScrollViewScope(recipeScroll, EditorStyles.textField)) {
                GUI.enabled = true;
                recipeScroll = scope.scrollPosition;
            }
        }

        private void DrawRecipePreview() {
            EditorUtils.WindowBoxLabel("Recipe Preview");
        }
    }

    public abstract class ActorManager : BonBaseTool {

    }
}