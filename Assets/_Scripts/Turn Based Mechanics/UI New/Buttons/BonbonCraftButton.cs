namespace BattleUI {
    public class BonbonCraftButton : UIButtonBase<BonbonCraftHandler> {

        private BonbonBlueprint bonbon;

        public void Init(BonbonCraftHandler stateHandler, BonbonBlueprint bonbon) {
            this.StateHandler = stateHandler;
            this.bonbon = bonbon;
        }

        public override bool IsAvailable() => StateHandler.CurrActor.Stamina >= bonbon.craftStamina;

        public override void Activate() {
            base.Activate();
            StateHandler.CraftBonbon(bonbon);
        }
    }
}