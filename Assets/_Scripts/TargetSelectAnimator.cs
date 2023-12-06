using System.Collections;
using System.Collections.Generic;
using BattleUI;
using UnityEngine;

namespace BattleUI {
    public class TargetSelectAnimator : UIStateAnimator {

        private List<TargetButtonAnimator> runtimeAnimators = new();

        protected override void Awake() {
            base.Awake();
            (StateHandler as TargetSelectHandler).OnButtonCreated += Init;
        }

        private void Init(UIButton button) {
            TargetButtonAnimator animator = button.gameObject.AddComponent<TargetButtonAnimator>();
            animator.Init(this);
            runtimeAnimators.Add(animator);
        }

        protected override void UIStateHandler_OnHandlerToggle(bool toggle) {
            foreach (TargetButtonAnimator animator in runtimeAnimators) animator.Toggle(toggle);
            if (!toggle) runtimeAnimators.Clear();
        }
    }
}
