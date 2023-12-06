using System.Linq;
using UnityEngine;

namespace BattleUI {
    public class UIAnimationBrain : MonoBehaviour {

        private UIBrain logicBrain;

        private UIStateAnimator[] baseAnimators;
        private UIStateAnimator[] stateAnimators;

        public event System.Action<bool> OnGlobalSoftToggle;
        public event System.Action<BonbonFXInfo> OnBonbonAnimationCall;
        public void PropagateAnimationCall(BonbonFXInfo info) => OnBonbonAnimationCall?.Invoke(info);

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
    }
}