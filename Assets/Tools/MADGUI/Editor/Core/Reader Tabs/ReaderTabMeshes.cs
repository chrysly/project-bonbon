using UnityEditor;
using UnityEngine;
using CJUtils;
using static ModelAssetDatabase.Reader;

namespace ModelAssetDatabase {
    public class ReaderTabMeshes : ReaderTab {

        private MeshPreview meshPreview;

        /// <summary> Class that bundles properties relevant to the selected mesh for quick handling and disposal; </summary>
        private class SelectedMeshProperties {
            /// <summary> Mesh selected in the Editor Window; </summary>
            public Mesh mesh;
            /// <summary> Gameobject holding the mesh selected in the Editor Window </summary>
            public GameObject gameObject;
            /// <summary> Type of the renderer holding the mesh; </summary>
            public Renderer renderer;
            public Texture2D preview;

            public SelectedMeshProperties(Mesh mesh, GameObject gameObject, Renderer renderer) {
                this.mesh = mesh;
                this.gameObject = gameObject;
                this.renderer = renderer;
            }
        } /// <summary> Relevant properties of the Mesh selected in the GUI; </summary>
        private SelectedMeshProperties selectedMesh;
        /// <summary> Index of the selected SubMesh in the GUI (+1); </summary>
        private int selectedSubmeshIndex;

        /// <summary> Vertex count of a single mesh; </summary>
        private int localVertexCount;

        /// <summary> Triangle count of a single mesh; </summary>
        private int localTriangleCount;

        private static Vector2 meshUpperScroll;
        private static Vector2 meshLowerScroll;

        public override void ResetData() {
            CleanMeshPreview();
            selectedMesh = null;
            selectedSubmeshIndex = 0;
        }

        public void SetSelectedMesh(Mesh mesh, Renderer renderer) {
            ResetData();
            selectedMesh = new SelectedMeshProperties(mesh, renderer.gameObject, renderer);
            localVertexCount = mesh.vertexCount;
            localTriangleCount = mesh.triangles.Length;
        }

        private void SetSelectedSubMesh(int index) {
            Reader.CleanObjectPreview();
            if (index > 0) {
                Reader.CreateDummyGameObject(selectedMesh.gameObject);
                Renderer renderer = Reader.DummyGameObject.GetComponent<Renderer>();
                Material[] arr = renderer.sharedMaterials;
                for (int i = 0; i < arr.Length; i++) {
                    if (i != index - 1) arr[i] = Reader.CustomTextures.defaultMaterial;
                } arr[index - 1] = Reader.CustomTextures.highlight;
                renderer.sharedMaterials = arr;
            } selectedSubmeshIndex = index;
        }

        /// <summary>
        /// Draw a mesh preview of the currently selected mesh;
        /// </summary>
        /// <param name="mesh"> Mesh to draw the preview for; </param>
        /// <param name="width"> Width of the Preview's Rect; </param>
        /// <param name="height"> Height of the Preview's Rect; </param>
        private void DrawMeshPreview(Mesh mesh, params GUILayoutOption[] options) {
            Rect rect = EditorGUILayout.GetControlRect(options);
            if (meshPreview == null) {
                meshPreview = new MeshPreview(mesh);
            } else {
                GUIStyle style = new GUIStyle();
                style.normal.background = Reader.CustomTextures.meshPreviewBackground;
                meshPreview.OnPreviewGUI(rect, style);
            }
        }

        /// <summary>
        /// Dispose of the contents of the current Mesh Preview;
        /// </summary>
        private void CleanMeshPreview() {
            try {
                if (meshPreview != null) {
                    meshPreview.Dispose();
                    meshPreview = null;
                }
            } catch (System.NullReferenceException) {
                Debug.LogWarning("Nice Assembly Reload! Please disregard this message...");
            }
        }

        /// <summary> GUI Display for the Meshes Section </summary>
        public override void ShowGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                /// Mesh Preview + Mesh Details;
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(200), GUILayout.Height(200))) {
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(192), GUILayout.Height(192))) {
                        GUILayout.Label("Mesh Preview", UIStyles.CenteredLabel);
                        if (selectedMesh != null) {
                            GUIContent settingsContent = new GUIContent(" Preview Settings", EditorUtils.FetchIcon("d_Mesh Icon"));
                            if (selectedSubmeshIndex == 0) {
                                DrawMeshPreview(selectedMesh.mesh, GUILayout.ExpandWidth(true), GUILayout.Height(192));

                                if (GUILayout.Button(settingsContent, GUILayout.MaxHeight(19))) {
                                    ExtraMeshPreview
                                        .ShowPreviewSettings(meshPreview,
                                                             GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect()));
                                }
                            } else {
                                Reader.DrawObjectPreviewEditor(Reader.DummyGameObject, GUILayout.ExpandWidth(true), GUILayout.Height(192));
                                GUI.enabled = false;
                                GUILayout.Button(settingsContent, GUILayout.MaxHeight(19));
                                GUI.enabled = true;
                            } using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                                if (GUILayout.Button("Open In Materials")) Reader.SwitchToMaterials(selectedMesh.renderer);
                            }
                        } else EditorUtils.DrawTexture(Reader.CustomTextures.noMeshPreview, 206, 206);

                        using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.FlexibleSpace();
                            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(206), GUILayout.Height(60))) {
                                if (selectedMesh != null) {
                                    GUILayout.Label("Mesh Details", UIStyles.CenteredLabel);
                                    GUILayout.FlexibleSpace();
                                    EditorUtils.DrawLabelPair("Vertex Count:", localVertexCount.ToString());
                                    EditorUtils.DrawLabelPair("Triangle Count: ", localTriangleCount.ToString());
                                } else {
                                    EditorUtils.DrawScopeCenteredText("No Mesh Selected");
                                }
                            } GUILayout.FlexibleSpace();
                        }
                    }
                }

                using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(PANEL_WIDTH))) {
                    EditorUtils.DrawSeparatorLines("Renderer Details", true);
                    if (selectedMesh != null) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorUtils.DrawLabelPair("Skinned Mesh:", selectedMesh.renderer is SkinnedMeshRenderer ? "Yes" : "No");
                            GUILayout.FlexibleSpace();
                            EditorUtils.DrawLabelPair("No. Of Submeshes:", selectedMesh.mesh.subMeshCount.ToString());
                            GUILayout.FlexibleSpace();
                            EditorUtils.DrawLabelPair("Materials Assigned:", selectedMesh.renderer.sharedMaterials.Length.ToString());
                        } GUIContent propertiesContent = new GUIContent(" Open Mesh Properties", EditorUtils.FetchIcon("Settings"));
                        if (GUILayout.Button(propertiesContent, GUILayout.MaxHeight(19))) EditorUtils.OpenAssetProperties(selectedMesh.mesh);
                    } else {
                        EditorGUILayout.Separator();
                        GUILayout.Label("No Mesh Selected", UIStyles.CenteredLabelBold);
                        EditorGUILayout.Separator();
                    }

                    EditorUtils.DrawSeparatorLines("Submeshes", true);
                    if (selectedMesh != null) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            using (var view = new EditorGUILayout.ScrollViewScope(meshUpperScroll, true, false,
                                                                                GUI.skin.horizontalScrollbar, GUIStyle.none,
                                                                                GUI.skin.box, GUILayout.MaxWidth(PANEL_WIDTH), GUILayout.MaxHeight(56))) {
                                meshUpperScroll = view.scrollPosition;
                                using (new EditorGUILayout.HorizontalScope()) {
                                    for (int i = 0; i < selectedMesh.mesh.subMeshCount; i++) DrawSubMeshSelectionButton(i + 1);
                                }
                            } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(54))) {
                                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                                    GUILayout.Box("Highlight Color", UIStyles.CenteredLabelBold);
                                } GUILayout.FlexibleSpace();
                                Color color = EditorGUILayout.ColorField(Reader.CustomTextures.highlight.color, GUILayout.MaxWidth(80));
                                if (color != Reader.CustomTextures.highlight.color) {
                                    Reader.CustomTextures.highlight.color = color;
                                    SetSelectedSubMesh(selectedSubmeshIndex);
                                } GUILayout.FlexibleSpace();
                            }
                        }
                    } else {
                        EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
                        GUILayout.Label("No Mesh Selected", UIStyles.CenteredLabelBold);
                        EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
                    } DrawMeshSearchArea();
                }
            }
        }

        /// <summary>
        /// Displays a horizontal scrollview with all the meshes available in the model to select from;
        /// </summary>
        /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
        private void DrawMeshSearchArea(float scaleMultiplier = 1f) {

            EditorUtils.DrawSeparatorLines("All Meshes", true);

            using (var view = new EditorGUILayout.ScrollViewScope(meshLowerScroll, true, false,
                                                                  GUI.skin.horizontalScrollbar, GUIStyle.none,
                                                                  GUI.skin.box, GUILayout.MaxWidth(PANEL_WIDTH), GUILayout.MaxHeight(scaleMultiplier == 1 ? 130 : 110))) {
                meshLowerScroll = view.scrollPosition;
                using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(PANEL_WIDTH), GUILayout.MaxHeight(scaleMultiplier == 1 ? 130 : 110))) {
                    foreach (MeshRendererPair mrp in Reader.MeshRenderers) {
                        if (mrp.renderer is SkinnedMeshRenderer) {
                            DrawMeshSelectionButton((mrp.renderer as SkinnedMeshRenderer).sharedMesh,
                                                     mrp.renderer, scaleMultiplier);
                        } else if (mrp.renderer is MeshRenderer) {
                            DrawMeshSelectionButton(mrp.filter.sharedMesh, mrp.renderer, scaleMultiplier);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw a button to select a given submesh;
        /// </summary>
        /// <param name="mesh"> Mesh that the button will select; </param>
        /// <param name="renderer"> Renderer containing the mesh; </param>
        /// <param name="scaleMultiplier"> Lazy scale multiplier; </param>
        private void DrawMeshSelectionButton(Mesh mesh, Renderer renderer, float scaleMultiplier) {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxWidth(1))) {
                EditorUtils.DrawTexture(Reader.MeshPreviewDict[renderer], 80 * scaleMultiplier, 80 * scaleMultiplier);
                if (selectedMesh != null && selectedMesh.mesh == mesh) {
                    GUILayout.Label("Selected", new GUIStyle(UIStyles.CenteredLabelBold) { contentOffset = new Vector2(0, -2) },
                                    GUILayout.Width(80 * scaleMultiplier), GUILayout.Height(14 * scaleMultiplier));
                } else if (GUILayout.Button("Open", GUILayout.Width(80 * scaleMultiplier), 
                                            GUILayout.Height(18 * scaleMultiplier))) SetSelectedMesh(mesh, renderer);
            }
        }

        /// <summary>
        /// Update the selected submesh index and update the preview to reflect it;
        /// </summary>
        /// <param name="index"> Index of the submesh to select; </param>
        private void DrawSubMeshSelectionButton(int index) {
            bool isSelected = index == selectedSubmeshIndex;
            GUIStyle buttonStyle = isSelected ? EditorStyles.helpBox : GUI.skin.box;
            using (new EditorGUILayout.VerticalScope(buttonStyle, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35))) {
                if (GUILayout.Button(index.ToString(), UIStyles.TextureButton, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35))) {
                    if (isSelected) index = 0;
                    SetSelectedSubMesh(index);
                }
            }
        }
    }
}