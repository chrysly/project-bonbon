using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {

    public abstract class UIButtonBase<T> : UIButton where T : UIStateHandler {

        protected T stateHandler;

        public override void Init<E>(E stateHandler) {
            if (stateHandler is T) {
                this.stateHandler = stateHandler as T;
            } else Debug.LogError("Invalid State Handler");
        }
    }
}