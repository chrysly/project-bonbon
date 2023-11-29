using System.Linq;
using UnityEngine;

namespace BattleUI {
    public class UIAnimationBrain : MonoBehaviour {

        private UIBrain logicBrain;

        private UIStateAnimator[] baseAnimators;
        private UIStateAnimator[] stateAnimators;

        public event System.Action<UIButtonAnimator> OnSelectionChange;

        void Awake() {
            logicBrain = GetComponent<UIBrain>();
            logicBrain.OnUIRefresh += UIBrain_OnUIRefresh;
            baseAnimators = GetComponentsInChildren<UIStateAnimator>(true);
        }

        private void UIBrain_OnUIRefresh(Actor actor) {
            stateAnimators = baseAnimators.Concat(actor.gameObject
                                          .GetComponentsInChildren<UIStateAnimator>(true)).ToArray();
            foreach (UIStateAnimator animator in stateAnimators) animator.Init(this);
        }

        public void UpdateSelection(UIButtonAnimator button) => OnSelectionChange?.Invoke(button);
    }
}