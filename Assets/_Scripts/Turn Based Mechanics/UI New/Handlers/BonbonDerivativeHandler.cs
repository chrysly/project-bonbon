namespace BattleUI {
    /// <summary>
    /// Base class with QoL accessors for derivate bonbon handlers;
    /// <br></br> Inherits essential transition info as a FluidStateHandler;
    /// </summary>
    public abstract class BonbonDerivativeHandler : FluidStateHandler<BonbonTransitionInfo> {
        public int Slot => TransitionInfo.slot;
        public BonbonObject[] Inventory => Brain.CurrActor.BonbonInventory;
        public BonbonHandler BonbonHandler => Brain.BattleStateMachine.CurrInput.BonbonHandler;
        protected virtual void Awake() { Type = UIStateType.Bonbon; }
    }
}