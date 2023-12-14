using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class ScreenSpaceHandler : StateMachineHandler {

        private ScreenSpaceElement[] elements;

        public override void Initialize(BattleStateInput input) {
            base.Initialize(input);
            elements = GetComponentsInChildren<ScreenSpaceElement>();
            foreach (ScreenSpaceElement element in elements) element.Init(this);
        }
    }
}

