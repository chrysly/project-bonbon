using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using CJUtils;

namespace ModelAssetDatabase {

    /// <summary>
    /// Namespace managing the shader popup retrieval and shader history;
    /// </summary>
    namespace MADShaderUtility {

        public static class MADShaderUtils {

            /// <summary> Delegate for the local Shader Popup event set-up; </summary>
            public static System.Action<Shader> OnShaderResult;
            /// <summary> A list of recently selected shaders; </summary>
            public static List<Shader> shaderHistory;

            /// <summary>
            /// Fetches the internal Advanced Popup used for shader selection in the Material Editor;
            /// </summary>
            /// <param name="position"> Rect used to draw the popup button; </param>
            public static void ShowShaderSelectionAdvancedDropdown(Rect position) {
                System.Type type = typeof(Editor).Assembly.GetType("UnityEditor.MaterialEditor+ShaderSelectionDropdown");
                var dropDown = System.Activator.CreateInstance(type, args: new object[] { Shader.Find("Transparent/Diffuse"), (System.Action<object>) OnSelectedShaderPopup });
                MethodInfo method = type.GetMethod("Show");
                method.Invoke(dropDown, new object[] { position });
            }

            /// <summary>
            /// Output method for the Shader Selection event set-up;
            /// </summary>
            /// <param name="objShaderName"> Object output from the Shader Selection event containing a shader name; </param>
            private static void OnSelectedShaderPopup(object objShaderName) {
                var shaderName = (string) objShaderName;
                if (!string.IsNullOrEmpty(shaderName)) {
                    Shader shader = Shader.Find(shaderName);
                    OnShaderResult?.Invoke(shader);
                    if (shader != null) AddToShaderHistory(shader);
                }
            }

            /// <summary>
            /// Draws a standard shader popup at the given Rect;
            /// </summary>
            /// <param name="position"> Rect where the popup will be drawn; </param>
            /// <param name="shaderContent"> Text displayed on the popup button; </param>
            /// <param name="shaderCallback"> Callback subscribing to the OnShaderResult event;
            /// <br></br> NOTE: The callback should unsubscribe from the event to avoid unexpected behavior; </param>
            public static void DrawDefaultShaderPopup(Rect position, GUIContent shaderContent, System.Action<Shader> shaderCallback) {
                if (EditorGUI.DropdownButton(position, shaderContent, FocusType.Keyboard, EditorStyles.miniPullDown)) {
                    ShowShaderSelectionAdvancedDropdown(position);
                    OnShaderResult += shaderCallback;
                }
            }

            public static void DrawShaderHistoryPopup(Rect position, System.Action<Shader> callback) {
                if (EditorGUI.DropdownButton(position, new GUIContent(EditorUtils.FetchIcon("d_UnityEditor.AnimationWindow")),
                                             FocusType.Keyboard, EditorStyles.miniPullDown)) {
                    ShowShaderHistoryDropdown(position);
                    OnShaderResult += callback;
                }
            }

            public static void ShowShaderHistoryDropdown(Rect rect) {
                ShaderHistoryDropdown historyDropdown = new ShaderHistoryDropdown(shaderHistory, OnSelectedShaderPopup);
                historyDropdown.Show(rect);
            }

            /// <summary>
            /// Adds a shader to the shader history list;
            /// </summary>
            /// <param name="shader"> Shader added to the list; </param>
            public static void AddToShaderHistory(Shader shader) {
                if (shaderHistory == null) shaderHistory = new List<Shader>();
                if (shaderHistory.Contains(shader)) {
                    shaderHistory.Remove(shader);
                    shaderHistory.Insert(0, shader);
                } else {
                    int maxCount = 5;
                    if (shaderHistory.Count >= maxCount) shaderHistory.RemoveAt(maxCount - 1);
                    shaderHistory.Insert(0, shader);
                }
            }

            public class ShaderHistoryDropdown : AdvancedDropdown {

                private System.Action<object> onSelectedShaderPopup;
                private List<Shader> shaderList;

                public ShaderHistoryDropdown(List<Shader> shaderList, System.Action<object> onSelectedShaderPopup)
                        : base(new AdvancedDropdownState()) {
                    minimumSize = new Vector2(150, 0);
                    this.shaderList = shaderList;
                    this.onSelectedShaderPopup = onSelectedShaderPopup;
                }

                protected override AdvancedDropdownItem BuildRoot() {
                    AdvancedDropdownItem root;
                    if (shaderList == null || shaderList.Count == 0) {
                        root = new AdvancedDropdownItem("No Recent Shaders");
                    } else {
                        root = new AdvancedDropdownItem("Recent Shaders");
                        foreach (Shader shader in shaderList) {
                            root.AddChild(new AdvancedDropdownItem(shader.name));
                        }
                    } return root;
                }

                protected override void ItemSelected(AdvancedDropdownItem item) {
                    onSelectedShaderPopup(item.name);
                }
            }
        }
    }
}
