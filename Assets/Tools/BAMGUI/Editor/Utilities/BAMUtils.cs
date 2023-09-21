using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

namespace BonbonAssetManager {
    public static class BAMUtils {
        public static void DrawBonbonDragButton<T>(T draggedObject, GUIContent content, float buttonSize) where T : Object {
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

        public static T DrawDragAcceptButton<T>(params GUILayoutOption[] options) where T : Object {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                T obj = null;
                obj = EditorGUILayout.ObjectField(obj, typeof(T), false, options) as T;
                return obj;
            }
        }

        public static void InsertAction(this List<ImmediateAction> actionList, System.Type actionType) {
            List<System.Type> typeList = new List<System.Type>();
            foreach (ImmediateAction action in actionList) typeList.Add(action.GetType());
            if (typeList.Contains(actionType)) Debug.LogWarning("The action is already included in the list;");
            else {
                ImmediateAction actionInstance = System.Activator.CreateInstance(actionType, new object[0]) as ImmediateAction;
                actionList.Add(actionInstance);
            }
        }

        public static void RemoveAction(this List<ImmediateAction> actionList, System.Type actionType) {
            for (int i = 0; i < actionList.Count; i++) {
                if (actionList[i].GetType() == actionType) {
                    actionList.RemoveAt(i);
                    return;
                }
            } Debug.LogWarning("There was no such action in the list");
        }

        public static ImmediateAction FindAction(this List<ImmediateAction> actionList, System.Type actionType) {
            foreach (ImmediateAction action in actionList) {
                if (action.GetType() == actionType) return action;
            } return null;
        }
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
        public static ImmediateAction[] FetchAvailableActions(List<ImmediateAction> actionBank, System.Type[] actionTypes) {
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
        public static bool DrawAvailableActions(ref List<ImmediateAction> actionList, 
                                                ref ImmediateAction[] foundActions, System.Type[] actionTypes) {
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
}
