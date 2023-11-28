using UnityEngine;
using UnityEngine.UI;

namespace BattleUI {
    public class SkillSelectHandler : FluidStateHandler<SkillTransitionInfo> {

        [SerializeField] private Transform skillView;
        [SerializeField] private GameObject skillButtonPrefab;
        private SkillSelectButton[] buttonArr;

        private void Awake() { Type = UIStateType.Skill; }

        public override UIInputPack InputArrangement() {
            int skillCount = Brain.CurrActor.SkillList.Count;
            buttonArr = new SkillSelectButton[skillCount];
            for (int i = 0; i < skillCount; i++) {
                SkillAction skill = Brain.CurrActor.SkillList[i];
                GameObject go = Instantiate(skillButtonPrefab, skillView);
                go.name = $"Skill: {skill}";
                buttonArr[i] = go.GetComponent<SkillSelectButton>();
                buttonArr[i].Init(this, skill);
            } return new UIInputPack(new[] { buttonArr }, TraversalMode.Vertical);
        }

        public void Transition<T>(SkillAction skill) where T : UIStateHandler {
            Brain.Transition<T>(TransitionInfo.ExpandWith(skill));
        }

        public override void Revert() {
            buttonArr.Dispose();
            buttonArr = null;
            base.Revert();
        }
    }
}