using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class ScreenSpaceHandler : StateMachineHandler {

        private ScreenSpaceElement[] elements;
        private event System.Action<float> OnDamage;
        private event System.Action<float> OnHeal;
        private event System.Action<EffectBlueprint> OnEffect;
        private event System.Action<float> OnStamina;

        public override void Initialize(BattleStateInput input) {
            base.Initialize(input);
            elements = GetComponentsInChildren<ScreenSpaceElement>();
            foreach (ScreenSpaceElement element in elements) element.Init(this);
        }
    }
}

