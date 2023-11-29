using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class TargetSelectHandler : FluidStateHandler<SkillTransitionInfo> {

        private TargetAnimator ta;
        private TargetSelectButton[] buttonArr;

        private void Awake() { Type = UIStateType.TargetSelect;
            ta = GetComponent<TargetAnimator>();
        }

        public override UIInputPack InputArrangement() {
            List<Actor> targets = Brain.BattleStateMachine
                                       .FilterActorsBySkill(TransitionInfo.Skill);
            buttonArr = new TargetSelectButton[targets.Count];
            for (int i = 0; i < targets.Count; i++) {
                Transform anchor = targets[i].GetComponentInChildren<CursorIdentifier>(true).transform;
                GameObject go = new GameObject($"Target: {targets[i]}");
                go.transform.parent = anchor;
                go.transform.localPosition = Vector3.zero;
                buttonArr[i] = go.AddComponent<TargetSelectButton>();
                var aaaa = go.AddComponent<UIButtonAnimator>();
                ta.Init(aaaa);
                buttonArr[i].Init(this);
                buttonArr[i].Init(targets[i], anchor);
            } return new UIInputPack(new[] { buttonArr }, TraversalMode.Horizontal);
        }

        public void Attack(Actor target) {
            TransitionInfo.SkillPrep.targets = new[] { target };
            Brain.BattleStateMachine.CurrInput
                 .SkillHandler.SkillActivate(TransitionInfo.SkillPrep);
            Brain.ExitUI(false);
        }

        public override void Revert() {
            if (buttonArr != null) {
                buttonArr.Dispose();
                buttonArr = null;
            } base.Revert();
        }
    }
}