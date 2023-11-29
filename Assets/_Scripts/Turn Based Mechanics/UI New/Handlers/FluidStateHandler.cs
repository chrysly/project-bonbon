namespace BattleUI {
    public class FluidStateHandler<T> : UIStateHandler where T : BaseTransitionInfo, new() {

        private T transitionInfo;
        protected T TransitionInfo {
            get => transitionInfo == null ? new() : transitionInfo;
            set => transitionInfo = value;
        }

        public override UIInputPack Enable(BaseTransitionInfo info) {
            if (info is SkillTransitionInfo) TransitionInfo = info as T;
            if (info is BonbonTransitionInfo) {
                TransitionInfo = info as T;
            } return base.Enable(info);
        }

        public override void Revert() {
            TransitionInfo = null;
            base.Revert();
        }
    }
}