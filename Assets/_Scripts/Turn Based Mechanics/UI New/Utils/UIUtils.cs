using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {

    public static class UILogicUtils {

        public static void LoadHandlers<T>(this Dictionary<System.Type, T> handlerMap, 
                                           UIBrain brain, GameObject gameObject) where T : UIStateHandler {
            T[] handlers = gameObject.GetComponentsInChildren<T>();
            foreach (T handler in handlers) {
                handlerMap[handler.GetType()] = handler;
                handler.Init(brain);
            }
        }

        public static void Dispose(this UIButton[] buttonArr) {
            foreach (UIButton button in buttonArr) {
                if (button) Object.Destroy(button.gameObject);
            }
        }
    }
}