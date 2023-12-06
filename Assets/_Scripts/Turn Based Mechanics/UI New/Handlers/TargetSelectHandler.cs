using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class TargetSelectHandler : FluidStateHandler<SkillTransitionInfo> {

        private TargetSelectButton[] buttonArr;

        public event System.Action<UIButton> OnButtonCreated;

        private void Awake() { Type = UIStateType.TargetSelect; }

        public override UIInputPack InputArrangement() {
            List<Actor> targets = Brain.BattleStateMachine
                                       .FilterActorsBySkill(TransitionInfo.Skill);
            buttonArr = new TargetSelectButton[targets.Count];
            for (int i = 0; i < targets.Count; i++) {
                Transform anchor = targets[i].GetComponentInChildren<CursorIdentifier>(true).transform;
                GameObject go = new GameObject($"Target: {targets[i]}");
                go.transform.parent = anchor;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                buttonArr[i] = go.AddComponent<TargetSelectButton>();
                OnButtonCreated?.Invoke(buttonArr[i]);
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
            buttonArr = null;
            base.Revert();
        }
    }
}