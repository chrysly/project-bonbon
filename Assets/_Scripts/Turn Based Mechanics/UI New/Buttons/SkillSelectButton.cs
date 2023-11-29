namespace BattleUI {
    public class SkillSelectButton : UIButtonBase<SkillSelectHandler> {

        public SkillAction Skill { get; private set; }

        public void Init(SkillSelectHandler stateHandler, SkillAction skill) {
            StateHandler = stateHandler;
            Skill = skill;
        }

        public override bool IsAvailable() => StateHandler.CurrActor.Stamina >= Skill.SkillData.staminaCost;

        public override void Activate() {
            base.Activate();
            StateHandler.Transition<TargetSelectHandler>(Skill);
        }
    }
}