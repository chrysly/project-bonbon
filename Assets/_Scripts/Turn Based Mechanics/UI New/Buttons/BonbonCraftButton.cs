namespace BattleUI {
    public class BonbonCraftButton : UIButtonBase<BonbonCraftHandler> {

        public BonbonBlueprint Bonbon { get; private set; }

        public void Init(BonbonCraftHandler stateHandler, BonbonBlueprint bonbon) {
            StateHandler = stateHandler;
            Bonbon = bonbon;
        }

        public override bool IsAvailable() => StateHandler.CurrActor.Stamina >= Bonbon.craftStamina;

        public override void Activate() {
            base.Activate();
            StateHandler.CraftBonbon(Bonbon);
        }
    }
}