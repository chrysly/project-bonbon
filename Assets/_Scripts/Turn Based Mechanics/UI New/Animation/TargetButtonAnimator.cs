using DG.Tweening;

namespace BattleUI {
    public class TargetButtonAnimator : UIButtonAnimator {

        protected override void Awake() {
            cursorTarget = transform;
            LoadLogicButton();
        }

        protected override void UIButton_OnActivate() {
            transform.DOScale(transform.localScale * 2, animationDuration).SetEase(Ease.OutBack);
        }

        protected override void ProcessAvailability() { }
    }
}