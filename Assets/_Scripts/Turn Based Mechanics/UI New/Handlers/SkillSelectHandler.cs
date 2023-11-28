using UnityEngine;
using UnityEngine.UI;

namespace BattleUI {
    public class SkillSelectHandler : FluidStateHandler<SkillTransitionInfo> {

        [SerializeField] private Transform skillView;
        [SerializeField] private GameObject skillButtonPrefab;
        public SkillSelectButton[] ButtonArr { get; private set; }

        private void Awake() { Type = UIStateType.Skill; }

        public override UIInputPack InputArrangement() {
            int skillCount = Brain.CurrActor.SkillList.Count;
            ButtonArr = new SkillSelectButton[skillCount];
            for (int i = 0; i < skillCount; i++) {
                SkillAction skill = Brain.CurrActor.SkillList[i];
                GameObject go = Instantiate(skillButtonPrefab, skillView);
                go.name = $"Skill: {skill}";
                ButtonArr[i] = go.GetComponent<SkillSelectButton>();
                ButtonArr[i].Init(this, skill);
            } return new UIInputPack(new[] { ButtonArr }, TraversalMode.Vertical);
        }

        public void Transition<T>(SkillAction skill) where T : UIStateHandler {
            Brain.Transition<T>(TransitionInfo.ExpandWith(skill));
        }

        public override void Revert() {
            ButtonArr = null;
            base.Revert();
        }
    }
}