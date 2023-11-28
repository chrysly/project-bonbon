using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {

    public abstract class UIButtonBase<T> : UIButton where T : UIStateHandler {

        public T StateHandler { get; protected set; }

        public override void Init<E>(E stateHandler) {
            if (stateHandler is T) {
                StateHandler = stateHandler as T;
            } else Debug.LogError("Invalid State Handler");
        }
    }
}