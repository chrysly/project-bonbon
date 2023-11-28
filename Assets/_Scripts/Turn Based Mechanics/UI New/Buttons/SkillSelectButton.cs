namespace BattleUI {
    public class SkillSelectButton : UIButtonBase<SkillSelectHandler> {

        private SkillAction skill;

        public void Init(SkillSelectHandler stateHandler, SkillAction skill) {
            this.stateHandler = stateHandler;
            this.skill = skill;
        }

        public override bool IsAvailable() => stateHandler.CurrActor.Stamina >= skill.SkillData.staminaCost;

        public override void Activate() {
            base.Activate();
            stateHandler.Transition<TargetSelectHandler>(skill);
        }
    }
}