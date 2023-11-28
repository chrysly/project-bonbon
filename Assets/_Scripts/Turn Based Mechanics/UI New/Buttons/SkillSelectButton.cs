namespace BattleUI {
    public class SkillSelectButton : UIButtonBase<SkillSelectHandler> {

        private SkillAction skill;

        public void Init(SkillSelectHandler stateHandler, SkillAction skill) {
            this.StateHandler = stateHandler;
            this.skill = skill;
        }

        public override bool IsAvailable() => StateHandler.CurrActor.Stamina >= skill.SkillData.staminaCost;

        public override void Activate() {
            base.Activate();
            StateHandler.Transition<TargetSelectHandler>(skill);
        }
    }
}