using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class ScreenSpaceHandler : MonoBehaviour {

        public UIBrain Brain { get; private set; }
        private ScreenSpaceElement[] elements;

        public void Init(UIBrain brain) {
            Brain = brain;
            elements = GetComponentsInChildren<ScreenSpaceElement>();
            foreach (ScreenSpaceElement element in elements) element.Init(this);
        }
    }
}

