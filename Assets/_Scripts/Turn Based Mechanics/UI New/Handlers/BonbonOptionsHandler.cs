using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class BonbonOptionsHandler : BonbonDerivativeHandler {

        [SerializeField] private int shareStaminaCost;
        public int ShareStaminaCost => shareStaminaCost;

        private BonbonBlueprint[] matchingRecipes;
        private BonbonBlueprint[] MatchingRecipes {
            get {
                if (matchingRecipes == null) matchingRecipes = BonbonHandler.FindRelativeRecipes(Inventory[Slot].Data).ToArray();
                return matchingRecipes;
            }
        }

        public bool CanBake {
            get {
                if (MatchingRecipes == null || MatchingRecipes.Length == 0) return false;
                for (int i = 0; i < Inventory.Length; i++) {
                    if (MatchingRecipes.Any(bonbon => Inventory[i] != null && Inventory[i].Data != Inventory[Slot].Data
                                                      && bonbon.recipe.Contains(Inventory[i].Data))) return true;
                } return false;
            }
        }

        public override void Transition<T>() {
            if (typeof(T) == typeof(SkillSelectHandler)) {
                Brain.Transition<T>(new SkillTransitionInfo(Inventory[Slot]));
            } if (typeof(T) == typeof(BonbonBakeHandler)) {
                Brain.Transition<T>(new BonbonTransitionInfo(Slot));
            }
        }

        public Actor FetchPassTarget() {
            IEnumerable<Actor> actors = Brain.BattleStateMachine.FilterActors<CharacterActor>()
                                             .Where(actor => actor != Brain.CurrActor);
            return actors.Count() > 0 ? actors.ElementAt(0) : null;
        }

        public void ConsumeBonbon() {
            Inventory.ConsumeBonbon(CurrActor, Slot);
            Brain.ReturnTo<BonbonMainHandler>();
        }

        public void PassBonbon() {
            Actor target = FetchPassTarget();
            Brain.CurrActor.PassBonbon(Slot, target);
            Brain.CurrActor.ConsumeStamina(shareStaminaCost);
            Brain.ExitUI(true);
        }

        public override void Revert() {
            base.Revert();
            matchingRecipes = null;
        }
    }
}