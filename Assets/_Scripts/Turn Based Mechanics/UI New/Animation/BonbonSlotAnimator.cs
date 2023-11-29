using UnityEngine.UI;
using DG.Tweening;

namespace BattleUI {
    public class BonbonSlotAnimator : UIButtonAnimator {

        private BonbonObject Bonbon {
            get {
                if (Button == null) return null;
                if (Button is BonbonSlotButton) {
                    return (Button as BonbonSlotButton).Bonbon;
                } else if (Button is BonbonBakeSlotButton) {
                    return (Button as BonbonBakeSlotButton).Bonbon;
                } else return null;
            }
        } private BonbonSlotButton slotButton;
        private BonbonBakeSlotButton bakeButton;

        private RawImage icon;

        protected override void Awake() {
            base.Awake();
            icon = GetComponent<RawImage>();
        }

        protected override void LoadLogicButton() {
            slotButton = GetComponent<BonbonSlotButton>();
            slotButton.OnSelect += UIButton_OnSelect;
            slotButton.OnActivate += UIButton_OnActivate;
            bakeButton = GetComponent<BonbonBakeSlotButton>();
            bakeButton.OnSelect += UIButton_OnSelect;
            bakeButton.OnActivate += UIButton_OnActivate;
        }

        public override void Toggle(bool toggle) {
            /*if (Button != null) {
                Button.OnSelect -= UIButton_OnSelect;
                Button.OnActivate -= UIButton_OnActivate;
            } Button = stateAnimator.StateHandler is BonbonMainHandler ? slotButton : bakeButton;
            Button.OnSelect += UIButton_OnSelect;
            Button.OnActivate += UIButton_OnActivate;*/
            Button = stateAnimator.StateHandler is BonbonMainHandler ? slotButton : bakeButton;
            UpdateIcon();
            base.Toggle(toggle);
        }

        private void UpdateIcon() {
            icon.texture = Bonbon == null ? null : Bonbon.Texture;
            icon.DOFade(icon.texture == null ? 0 : 1, 0);
        }
    }
}