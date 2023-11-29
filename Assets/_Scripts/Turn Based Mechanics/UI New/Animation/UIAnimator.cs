using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace BattleUI {

    public enum UIAnimatorState {  Idle, Loading, Unloading }

    public abstract class UIAnimator : MonoBehaviour {

        protected float animationDuration = 0.15f;

        protected UIStateAnimator stateAnimator;

        protected UIAnimatorState state;

        protected virtual void Awake() {
            transform.DOScale(Vector2.zero, 0);
            StartCoroutine(CoreCoroutine());
        }

        public virtual void Init(UIStateAnimator stateAnimator) {
            this.stateAnimator = stateAnimator;
        }

        protected IEnumerator CoreCoroutine() {
            while (true) {
                switch (state) {
                    case UIAnimatorState.Loading:
                        IEnumerator loadCoroutine = Load();
                        while (loadCoroutine.MoveNext())
                            yield return loadCoroutine.Current;
                        state = UIAnimatorState.Idle;
                        break;
                    case UIAnimatorState.Idle:
                        IEnumerator idleCoroutine = Idle();
                        while (idleCoroutine.MoveNext())
                            yield return idleCoroutine.Current;
                        break;
                    case UIAnimatorState.Unloading:
                        IEnumerator unloadCoroutine = Unload();
                        while (unloadCoroutine.MoveNext())
                            yield return unloadCoroutine.Current;
                        state = UIAnimatorState.Idle;
                        break;
                } yield return null;
            }
        }

        protected virtual IEnumerator Load() {
            transform.DOScale(Vector2.one, animationDuration);
            yield return new WaitForSeconds(animationDuration);
        }

        protected virtual IEnumerator Idle() {
            yield return null;
        }

        protected virtual IEnumerator Unload() {
            transform.DOScale(Vector2.zero, animationDuration);
            yield return new WaitForSeconds(animationDuration);
        }

        public virtual void Toggle(bool toggle) {
            state = toggle ? UIAnimatorState.Loading : UIAnimatorState.Unloading;
        }
    }
}