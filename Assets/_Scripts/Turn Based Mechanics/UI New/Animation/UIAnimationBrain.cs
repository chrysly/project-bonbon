using System.Linq;
using UnityEngine;

namespace BattleUI {
    public class UIAnimationBrain : MonoBehaviour {

        public UIBrain LogicBrain { get; private set; }

        private UIStateAnimator[] baseAnimators;
        private UIStateAnimator[] stateAnimators;

        public event System.Action<UIButtonAnimator> OnSelectionChange;

        void Awake() {
            LogicBrain = GetComponent<UIBrain>();
            LogicBrain.OnUIRefresh += UIBrain_OnUIRefresh;
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