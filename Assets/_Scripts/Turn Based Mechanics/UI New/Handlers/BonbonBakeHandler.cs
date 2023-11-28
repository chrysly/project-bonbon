using System.Linq;
using System.Collections.Generic;

namespace BattleUI {
    public class BonbonBakeHandler : BonbonDerivativeHandler {

        public BonbonBlueprint[] MatchingRecipes { get; private set; }

        public override void Init(UIBrain brain) {
            base.Init(brain);
            IEnumerable<BonbonBakeSlotButton> slots = buttonMatrix.Values.Select(button => button as BonbonBakeSlotButton);
            for (int i = 0; i < slots.Count(); i++) slots.ElementAt(i).Init(i);
        }

        public override UIInputPack Enable(BaseTransitionInfo info) {
            MatchingRecipes = BonbonHandler.FindRelativeRecipes(Inventory[Slot].Data).ToArray();
            IEnumerable<BonbonBakeSlotButton> slots = buttonMatrix.Values.Select(button => button as BonbonBakeSlotButton);
            for (int i = 0; i < slots.Count(); i++) slots.ElementAt(i).Enable();
            return base.Enable(info);
        }

        public override UIInputPack InputArrangement() {
            UIButton[][] buttonArr2D = buttonMatrix.To2DArray();
            for (int i = 0; i < buttonArr2D.Length; i++) {
                buttonArr2D[i] = buttonArr2D[i].Where(button => (button as BonbonBakeSlotButton).Slot != Slot).ToArray();
            } return new UIInputPack(buttonArr2D, TraversalMode.Cardinal);
        }

        public BonbonBlueprint FindValidRecipe(int slot) {
            return MatchingRecipes.FirstOrDefault(bonbon => bonbon.recipe.ToList()
                                                            .Contains(Inventory[slot].Data));
        }

        public void MatchAndBake(int slot, BonbonBlueprint bakeTarget) {
            bool[] recipeMask = new bool[4];
            for (int i = 0; i < recipeMask.Length; i++) recipeMask[i] = i == slot || i == Slot;
            BonbonObject freshBonbon = BonbonHandler.CreateBonbon(bakeTarget, CurrActor, recipeMask);
            CurrActor.AcceptBonbon(slot, freshBonbon);
            Brain.ReturnTo<BonbonMainHandler>();
        }

        public override void Revert() {
            MatchingRecipes = null;
            base.Revert();
        }
    }
}