using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class ScreenSpaceElement : MonoBehaviour {

        protected ScreenSpaceHandler handler;

        public virtual void Init(ScreenSpaceHandler handler) {
            this.handler = handler;
        }
    }
}