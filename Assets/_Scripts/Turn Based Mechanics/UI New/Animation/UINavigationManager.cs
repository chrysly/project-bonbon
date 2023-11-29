using UnityEngine;

namespace BattleUI {
    public class UINavigationManager : MonoBehaviour {
        
        public UIButtonAnimator SelectedButton { get; private set; }
        private UIAnimationBrain brain;
        
        void Awake() {
            brain = GetComponent<UIAnimationBrain>();
            brain.OnSelectionChange += UIAnimationBrain_UpdateCursor;
        }

        private void UIAnimationBrain_UpdateCursor(UIButtonAnimator buttonAnim) {
            bool isDifferent = SelectedButton != null && SelectedButton.Button == buttonAnim.Button;
            if (SelectedButton != null) SelectedButton.OverrideSelect(false);
            SelectedButton = buttonAnim;
            SelectedButton.OverrideSelect(true);
            ///buttonAnim.Button is TargetSelectButton;
        }
    }
}