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
            foreach (ImmediateAction action in actionList) {
                if (action.GetType() == actionType) actionList.Remove(action);
                return;
            } Debug.LogWarning("There was no such action in the list");
        }

        public static ImmediateAction FindAction(this List<ImmediateAction> actionList, System.Type actionType) {
            foreach (ImmediateAction action in actionList) {
                if (action.GetType() == actionType) return action;
            } return null;
        }
    }
}
