using System.Linq;
using System.Collections.Generic;

namespace BattleUI {
    public class BonbonBakeHandler : BonbonDerivativeHandler {

        public BonbonBlueprint[] MatchingRecipes { get; private set; }

        public event System.Action<BonbonFXInfo> OnBonbonModification;

        public override UIInputPack Enable(BaseTransitionInfo info) {
            UIInputPack input = base.Enable(info);
            MatchingRecipes = BonbonHandler.FindRelativeRecipes(Inventory[Slot].Data).ToArray();
            IEnumerable<BonbonBakeSlotButton> slots = buttonMatrix.Values.Select(button => button as BonbonBakeSlotButton);
            for (int i = 0; i < slots.Count(); i++) slots.ElementAt(i).Enable();
            OnHandlerToggle?.Invoke(true);
            return input;
        }

        public BonbonBlueprint FindValidRecipe(int slot) {
            if (Inventory[slot].Data == Inventory[Slot].Data) return null;
            return MatchingRecipes.FirstOrDefault(bonbon => bonbon.recipe.ToList()
                                                            .Contains(Inventory[slot].Data));
        }

        public void MatchAndBake(int slot, BonbonBlueprint bakeTarget) {
            bool[] recipeMask = new bool[4];
            for (int i = 0; i < recipeMask.Length; i++) recipeMask[i] = i == slot || i == Slot;
            BonbonObject freshBonbon = BonbonHandler.CreateBonbon(bakeTarget, CurrActor, recipeMask);
            CurrActor.AcceptBonbon(Slot, freshBonbon);
            OnBonbonModification?.Invoke(new BonbonBakeInfo(Slot, new BonbonObject[] { Inventory[slot],
                                                                                       Inventory[Slot] }, bakeTarget));
            Brain.ReturnTo<BonbonMainHandler>();
        }

        public override void Revert() {
            MatchingRecipes = null;
            base.Revert();
        }
    }
}