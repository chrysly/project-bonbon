using System.Collections;
using UnityEngine;

namespace BattleUI {
    public abstract class HandsOnStateAnimator : UIStateAnimator {

        [SerializeField] protected float animationDuration;

        protected UIAnimatorState state;

        protected override void Awake() {
            base.Awake();
            StartCoroutine(CoreCoroutine());
        }

        protected IEnumerator CoreCoroutine() {
            while (true) {
                switch (state) {
                    case UIAnimatorState.Loading:
                        IEnumerator loadCoroutine = Load();
                        while (loadCoroutine.MoveNext())
                            yield return loadCoroutine.Current;
                        if (state == UIAnimatorState.Loading) state = UIAnimatorState.Idle;
                        break;
                    case UIAnimatorState.Unloading:
                        IEnumerator unloadCoroutine = Unload();
                        while (unloadCoroutine.MoveNext())
                            yield return unloadCoroutine.Current;
                        break;
                } yield return null;
            }
        }

        protected override void UIStateHandler_OnHandlerToggle(bool toggle) {
            base.UIStateHandler_OnHandlerToggle(toggle);
            state = toggle ? UIAnimatorState.Loading : UIAnimatorState.Unloading;
        }

        protected abstract IEnumerator Load();
        protected abstract IEnumerator Unload();
    }
}