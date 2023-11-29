namespace BattleUI {
    public class Main2SkillButton : UIButtonBase<MainStateHandler> {
        
        public override void Activate() {
            base.Activate();
            StateHandler.Transition<SkillSelectHandler>();
        }
    }
}