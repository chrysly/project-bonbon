using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BattleUI {

    public enum UIAnimatorState {  Idle, Loading, Unloading }

    public abstract class UIAnimator : MonoBehaviour {

        protected UIStateAnimator stateAnimator;
        protected MaskableGraphic[] graphics;

        protected virtual void Awake() {
            graphics = GetComponentsInChildren<MaskableGraphic>();
            transform.DOScale(Vector2.zero, 0);
        }

        public virtual void Init(UIStateAnimator stateAnimator) {
            this.stateAnimator = stateAnimator;
        }

        public virtual void Toggle(bool toggle) {
            transform.DOScale(toggle ? Vector2.one : Vector2.zero, 1f).SetEase(Ease.OutBounce);
        }
    }
}