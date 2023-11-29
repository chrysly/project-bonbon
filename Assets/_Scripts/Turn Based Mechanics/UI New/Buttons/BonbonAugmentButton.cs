namespace BattleUI {
    public class BonbonAugmentButton : UIButtonBase<BonbonOptionsHandler> {

        public override void Activate() {
            base.Activate();
            StateHandler.Transition<SkillSelectHandler>();
        }
    }
}