using UnityEditor;
using UnityEngine;
using CJUtils;
using static ModelAssetDatabase.ModelAssetDatabaseGUI;

namespace ModelAssetDatabase {
    public class ReaderTabAnimations : ReaderTab {

        /// <summary> Internal editor used to embed the Animation Clip Editor from the Model Importer; </summary>
        private Editor AnimationEditor;

        private static Vector2 animationScroll;

        public override void ResetData() => CleanAnimationEditor();

        /// <summary>
        /// Fetches a reference to the Animation Editor class;
        /// </summary>
        private void FetchAnimationEditor() {
            /// Fetch a reference to the base Model Importer Editor class;
            var editorType = typeof(Editor).Assembly.GetType("UnityEditor.ModelImporterEditor");
            /// Perform a clean reconstruction of the Model Importer Editor;
            DestroyImmediate(AnimationEditor);
            AnimationEditor = Editor.CreateEditor(Reader.Model, editorType);
        }

        /// <summary>
        /// Cleans the Animation Editor, if it exists;
        /// </summary>
        private void CleanAnimationEditor() {
            DestroyImmediate(AnimationEditor);
        }

        /// <summary> GUI Display for the Animations Section </summary>
        public override void ShowGUI() {
            if (AnimationEditor == null) FetchAnimationEditor();

            int panelWidth = 620;
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.Width(panelWidth / 2))) {
                    EditorUtils.WindowBoxLabel("Animation Editor");
                    EditorGUILayout.Separator();
                    using (var scope = new EditorGUILayout.ScrollViewScope(animationScroll)) {
                        animationScroll = scope.scrollPosition;
                        DrawAnimationEditor();
                    }
                } using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.Width(panelWidth / 2))) {
                    EditorUtils.WindowBoxLabel("Animation Preview");
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                        if (AnimationEditor.HasPreviewGUI()) {
                            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                                GUILayout.Label("Preview Settings:", new GUIStyle(GUI.skin.label) { contentOffset = new Vector2(0, -1) });
                                AnimationEditor.OnPreviewSettings();
                            } using (new EditorGUILayout.HorizontalScope()) {
                                GUILayout.FlexibleSpace();
                                using (new EditorGUILayout.VerticalScope()) {
                                    Rect rect = GUILayoutUtility.GetRect(panelWidth / 2 + 20, 0, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));
                                    AnimationEditor.OnInteractivePreviewGUI(rect, GUIStyle.none);
                                } GUILayout.FlexibleSpace();
                            }
                        } else EditorUtils.DrawScopeCenteredText("No animation to preview;");
                    } MainGUI.SetHighRepaintFrequency(true);
                } 
            }
        }

        /// <summary>
        /// Draws the Animation Clip Editor tab from the internal Model Importer Editor;
        /// </summary>
        private void DrawAnimationEditor() {
            if (AnimationEditor == null) return;
            /// Fetch a reference to the parent Asset Importer Editor, which contains the tabs array field;
            var baseType = typeof(Editor).Assembly.GetType("UnityEditor.AssetImporterTabbedEditor");
            /// Fetch a reference to the Model Importer Clip Editor tab class;
            var tabType = typeof(Editor).Assembly.GetType("UnityEditor.ModelImporterClipEditor");
            /// Fetch a reference to the field containing a tab array;
            var tabField = baseType.GetField("m_Tabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            /// Fetch a referebce to the OnInspectorGUI method fo the tab;
            var tabGUI = tabType.GetMethod("OnInspectorGUI");
            /// Cast the field value to an array of objects;
            object[] tabArray = (object[]) tabField.GetValue(AnimationEditor);
            /// Access the Animation Clip Editor tab, residing in index 2;
            var animationTab = tabArray[2];
            /// Invoke the method on the Animation Clip Editor tab;
            tabGUI.Invoke(animationTab, null);
        }
    }
}