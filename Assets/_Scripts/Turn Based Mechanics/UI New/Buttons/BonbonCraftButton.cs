namespace BattleUI {
    public class BonbonCraftButton : UIButtonBase<BonbonCraftHandler> {

        private BonbonBlueprint bonbon;

        public void Init(BonbonCraftHandler stateHandler, BonbonBlueprint bonbon) {
            this.stateHandler = stateHandler;
            this.bonbon = bonbon;
        }

        public override bool IsAvailable() => stateHandler.CurrActor.Stamina >= bonbon.craftStamina;

        public override void Activate() {
            base.Activate();
            stateHandler.CraftBonbon(bonbon);
        }
    }
}