using UnityEngine;

namespace BattleUI {
    public class UINavigationManager : MonoBehaviour {
        
        public UIButtonAnimator SelectedButton { get; private set; }
        private UIAnimationBrain brain;

        void Start() {
            brain = GetComponent<UIAnimationBrain>();
            brain.OnSelectionChange += UIAnimationBrain_UpdateSelection;
        }

        private void UIAnimationBrain_UpdateSelection(UIButtonAnimator buttonAnim) {
            if (SelectedButton != null) SelectedButton.OverrideSelect(false);
            SelectedButton = buttonAnim;
            SelectedButton.OverrideSelect(true);
        }
    }
}