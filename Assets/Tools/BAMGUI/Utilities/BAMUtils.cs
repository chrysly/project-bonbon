using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using CJUtils;
using PseudoDataStructures;

#endif

namespace BonbonAssetManager {

    #if UNITY_EDITOR

    public class ModalAssetDeletion : EditorWindow {

        /// <summary>
        /// Confirms whether a Prefab Deletion Action should be performed;
        /// </summary>
        /// <param name="prefabName"> Name to display in the confirmation window; </param>
        /// <returns> True if the asset should be deleted, false otherwise; </returns>
        public static bool ConfirmAssetDeletion(string prefabName) {
            fileName = prefabName;
            var window = GetWindow<ModalAssetDeletion>("Confirm Asset Deletion");
            window.maxSize = new Vector2(350, 105);
            window.minSize = window.maxSize;
            window.ShowModal();
            return result;
        }

        /// <summary> Name to display in the confirmation window; </summary>
        private static string fileName;
        /// <summary> Result to return from the modal window; </summary>
        private static bool result;

        void OnGUI() {
            using (new EditorGUILayout.VerticalScope(UIStyles.MorePaddingScrollView)) {
                GUILayout.Label("Are you sure you want to delete the following asset?", UIStyles.CenteredLabel);
                EditorGUILayout.Separator();
                GUI.color = UIColors.Red;
                GUILayout.Label(fileName, UIStyles.CenteredLabel);
                EditorGUILayout.Separator();
                using (new EditorGUILayout.HorizontalScope()) {
                    if (GUILayout.Button("Delete")) {
                        result = true;
                        Close();
                    } GUI.color = Color.white;
                    if (GUILayout.Button("Cancel")) {
                        result = false;
                        Close();
                    }
                }
            }
        }
    }

    public static class BAMUtils {

        #region | Asset Manipulation |

        public static void ResetHotControl() {
            GUIUtility.keyboardControl = 0;
            GUIUtility.hotControl = 0;
        }

        public static string ToFilePath(string path, string name) => path + "/" + name + ".asset";

        public static void DeleteAsset(string folderPath, string assetName) {
            AssetDatabase.MoveAssetToTrash(ToFilePath(folderPath, assetName));
        }

        #endregion

        #region | Initialization Helpers |

        public static List<T> InitializeList<T>() where T : Object {
            List<T> list = new List<T>();
            string typeName = typeof(T).FullName;
            var genericGUIDs = AssetDatabase.FindAssets($"t:{typeName}");
            for (int i = 0; i < genericGUIDs.Length; i++) {
                list.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(genericGUIDs[i])));
            } return list;
        }

        #endregion

        #region | Fields |

        public static void PassiveModFields() {

        }

        #endregion

        #region | Drag N' Drop |

        public static void DrawAssetDragButton<T>(T draggedObject, System.Func<object, GUIContent> contentFunc, Vector2 buttonSize) where T : ScriptableObject {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                Rect buttonRect = GUILayoutUtility.GetRect(buttonSize.x, buttonSize.y, GUILayout.ExpandWidth(false));
                if (buttonRect.Contains(Event.current.mousePosition)) {
                    bool mouseDown = Event.current.type == EventType.MouseDown;
                    bool leftClick = Event.current.button == 0;
                    if (mouseDown && leftClick) {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.StartDrag("Dragging");
                        DragAndDrop.objectReferences = new Object[] { draggedObject };
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                } GUI.Label(buttonRect, contentFunc.Invoke(draggedObject), GUI.skin.button);
            }
        }

        public static void DrawAssetDragButton<T>(T draggedObject, System.Func<object, GUIContent> contentFunc, float buttonSize) where T : Object {
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
                } GUI.Label(buttonRect, contentFunc.Invoke(draggedObject), GUI.skin.button);
            }
        }

        public static T DrawDragAcceptButton<T>(FieldUtils.DnDFieldType fieldType, CJToolAssets.DnDFieldAssets assets,
                                                params GUILayoutOption[] options) where T : Object {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                T obj = null;
                obj = FieldUtils.DnDField(typeof(T), fieldType, assets, options) as T;
                return obj;
            }
        }

        
        public static void DrawMap<T>(List<T>[] listArr, ref Vector2[] scrollGroup, System.Func<object, GUIContent> contentFunc,
                                      float buttonSize, CJToolAssets.DnDFieldAssets dndFieldAssets, System.Action saveCallback) where T : ScriptableObject {
            if (scrollGroup == null || scrollGroup.Length < listArr.Length) scrollGroup = new Vector2[listArr.Length];
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                EditorUtils.WindowBoxLabel($"Actor {typeof(T).FullName.CamelSpace().RemovePathEnd(" ")} Map");
                for (int i = 0; i < listArr.Length; i++) {
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        EditorUtils.WindowBoxLabel($"Level {i + 1}", GUILayout.Width(48), GUILayout.Height(45));
                        if (DrawLevelDropdown(listArr[i], ref scrollGroup[i], contentFunc, buttonSize, dndFieldAssets)) saveCallback?.Invoke();
                    }
                }
            }
        }

        public static bool DrawLevelDropdown<T>(List<T> list, ref Vector2 scrollValue, System.Func<object, GUIContent> contentFunc,
                                                float buttonSize, CJToolAssets.DnDFieldAssets dndFieldAssets) where T : ScriptableObject {
            bool changed = false;
            T acceptedElement = DrawDragAcceptButton<T>(FieldUtils.DnDFieldType.Add,
                                                        dndFieldAssets, GUILayout.Width(buttonSize),
                                                        GUILayout.Height(buttonSize));
            if (acceptedElement != null && !list.Contains(acceptedElement)) {
                changed = true;
                list.Add(acceptedElement);
            }

            using (var scope = new EditorGUILayout.HorizontalScope(UIStyles.WindowBox, GUILayout.ExpandWidth(true), GUILayout.Height(buttonSize * 1.575f))) {
                using (var scrollScope = new EditorGUILayout.ScrollViewScope(scrollValue, GUILayout.Width(scope.rect.width))) {
                    scrollValue = scrollScope.scrollPosition;
                    
                    using (new EditorGUILayout.HorizontalScope()) {
                        for (int i = 0; i < list.Count; i++) {
                            GUIContent content = contentFunc.Invoke(list[i]);
                            if (content.image == null) {
                                DrawAssetDragButton(list[i], contentFunc,
                                                    new Vector2(EditorUtils.MeasureTextWidth(content.text, GUI.skin.font) + 15, buttonSize));
                            } else {
                                DrawAssetDragButton(list[i], contentFunc, buttonSize);
                            }

                        }
                    }
                }
            } T removedElement = DrawDragAcceptButton<T>(FieldUtils.DnDFieldType.Remove,
                                                                dndFieldAssets, GUILayout.Width(buttonSize),
                                                                GUILayout.Height(buttonSize));
            if (removedElement != null) {
                changed = true;
                list.Remove(removedElement);
            } return changed;
        }

        public static void DrawAssetGroup<T>(List<T> assetList, Vector2 scrollVar, System.Func<object, GUIContent> contentFunc,
                                             Rect position, float buttonSize) where T : ScriptableObject {
            EditorUtils.WindowBoxLabel($"Available {typeof(T).FullName.CamelSpace().RemovePathEnd(" ")}s");

            using (var vscope = new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true))) {
                using (var scope = new EditorGUILayout.ScrollViewScope(scrollVar, GUILayout.Height(buttonSize + 25))) {
                    scrollVar = scope.scrollPosition;
                    DrawWrappedSequence(assetList, contentFunc, position, buttonSize);
                }
            }
        }

        public static void DrawWrappedSequence<T>(List<T> assetList, System.Func<object, GUIContent> contentFunc,
                                                  Rect position, float buttonSize) where T : ScriptableObject {

            int amountPerRow = Mathf.RoundToInt((position.xMax - position.xMin - 200) / buttonSize);
            using (new EditorGUILayout.VerticalScope()) {
                for (int i = 0; i < Mathf.CeilToInt((float) assetList.Count / amountPerRow); i++) {
                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                        for (int j = i * amountPerRow; j < Mathf.Min((i + 1) * amountPerRow, assetList.Count); j++) {
                            GUIContent content = contentFunc.Invoke(assetList[j]);
                            if (content.image == null) {
                                DrawAssetDragButton(assetList[j], contentFunc,
                                                    new Vector2(EditorUtils.MeasureTextWidth(content.text, GUI.skin.font) + 15, buttonSize));
                            } else DrawAssetDragButton(assetList[j], contentFunc, buttonSize);
                        }
                    }
                }
            }
        }

        public static ArrayArray<T> VerifyMapSize<T>(ArrayArray<T> arrArr) {
            int size = 8;
            /// If the list is null, return a new one with the set amount of slots;
            if (arrArr == null) return new ArrayArray<T>(new T[size][]);
            /// Else, create a clean array and parse the ArrayArray into a manageable list array;
            List<T>[] outputArr = new List<T>[8];
            List<T>[] listArr = arrArr.ToListArray();
            /// If the structure does not meet the size criteria, copy the pre-existing one into a properly sized one;
            if (listArr.Length < size) System.Array.Copy(listArr, outputArr, listArr.Length);
            else outputArr = listArr; /// Else, the passed list is the result;
            return new ArrayArray<T>(outputArr);
        }

        #endregion

        #region | Action Helpers |

        public static void InsertAction<T>(this List<T> actionList, System.Type actionType) where T : ImmediateAction {
            List<System.Type> typeList = new List<System.Type>();
            foreach (ImmediateAction action in actionList) typeList.Add(action.GetType());
            if (typeList.Contains(actionType)) Debug.LogWarning("The action is already included in the list;");
            else {
                ImmediateAction actionInstance = System.Activator.CreateInstance(actionType, new object[0]) as ImmediateAction;
                actionList.Add(actionInstance as T);
            }
        }

        public static void RemoveAction<T>(this List<T> actionList, System.Type actionType) where T : ImmediateAction {
            for (int i = 0; i < actionList.Count; i++) {
                if (actionList[i].GetType() == actionType) {
                    actionList.RemoveAt(i);
                    return;
                }
            } Debug.LogWarning("There was no such action in the list");
        }

        public static ImmediateAction FindAction<T>(this List<T> actionList, System.Type actionType) where T : ImmediateAction {
            foreach (ImmediateAction action in actionList) {
                if (action.GetType() == actionType) return action;
            } return null;
        }

        #endregion
    }

    /// <summary>
    /// Static utilities to manipulate Immediate Actions;
    /// </summary>
    public static class ActionUtils {

        /// <summary>
        /// Fetch all inheritors of the given supertypes;
        /// </summary>
        /// <param name="superTypes"> Types to fetch the inheriting classes for; </param>
        /// <returns> An array of types that inherit from the superclasses; </returns>
        public static System.Type[] FetchAssemblyChildren(System.Type[] superTypes) {
            List<System.Type> typeList = new List<System.Type>();
            foreach (System.Type superType in superTypes) {
                System.Type[] assemblies = System.Reflection.Assembly.GetAssembly(superType).GetTypes();
                foreach (System.Type type in assemblies) {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(superType)) typeList.Add(type);
                }
            } return typeList.ToArray();
        }

        /// <summary>
        /// Construct an Array mask from a List of Immediate Actions matching the configuration of an Array of types;
        /// </summary>
        /// <param name="actionBank"> List of Actions to process; </param>
        /// <param name="actionTypes"> Arrays to search for; </param>
        /// <returns> An array of Immediate Actions in a specific format; </returns>
        public static ImmediateAction[] FetchAvailableActions<T>(List<T> actionBank, System.Type[] actionTypes) where T : ImmediateAction{
            ImmediateAction[] foundActions = new ImmediateAction[actionTypes.Length];
            for (int i = 0; i < actionTypes.Length; i++) {
                var action = actionBank.FindAction(actionTypes[i]);
                if (action != null) foundActions[i] = action;
            } return foundActions;
        }

        /// <summary>
        /// Draw the Immediate Actions contained in a list and update the target asset as requested;
        /// <br></br> Operates on a predefined array of found actions to detect changes;
        /// </summary>
        /// <param name="actionList"> List of Actions to process; </param>
        /// <param name="foundActions"> Array of found actions to process; </param>
        /// <param name="actionTypes"> Action types to draw; </param>
        /// <returns> Whether the Object was updated in the GUI; </returns>
        public static bool DrawAvailableActions<T>(ref List<T> actionList,
                                                   ref ImmediateAction[] foundActions, System.Type[] actionTypes) where T : ImmediateAction {
            bool updateObject = false;
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                for (int i = 0; i < actionTypes.Length; i++) {
                    bool actionExists = foundActions[i] != null;

                    using (var scope = new EditorGUILayout.ToggleGroupScope(actionTypes[i].FullName, actionExists)) {
                        if (actionExists != scope.enabled) {
                            if (foundActions[i] == null) {
                                actionList.InsertAction(actionTypes[i]);
                            } else {
                                actionList.RemoveAction(actionTypes[i]);
                            } foundActions = FetchAvailableActions(actionList, actionTypes);
                            updateObject = true;
                        } if (foundActions[i] != null) foundActions[i].OnGUI();
                    }
                }
            } return updateObject;
        }
    }
    #endif
}
