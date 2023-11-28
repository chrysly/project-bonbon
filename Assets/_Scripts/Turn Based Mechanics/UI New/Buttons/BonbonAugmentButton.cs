namespace BattleUI {
    public class BonbonAugmentButton : UIButtonBase<BonbonOptionsHandler> {

        public override void Activate() {
            base.Activate();
            stateHandler.Transition<SkillSelectHandler>();
        }
    }
}