using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class BonbonOptionsHandler : BonbonDerivativeHandler {

        [SerializeField] private int shareStaminaCost;
        public int ShareStaminaCost => shareStaminaCost;

        public override void Transition<T>() {
            Brain.Transition<T>(new SkillTransitionInfo(Inventory[Slot]));
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
            Brain.ExitUI(true);
        }
    }
}