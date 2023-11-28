using UnityEngine;

namespace BattleUI {
    public class TargetSelectButton : UIButtonBase<TargetSelectHandler> {

        public Transform Anchor { get; private set; }
        private Actor target;

        public void Init(Actor target, Transform anchor) {
            Anchor = anchor;
            this.target = target;
        }

        public override void Activate() {
            base.Activate();
            stateHandler.Attack(target);
        }
    }
}