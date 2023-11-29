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
                        state = UIAnimatorState.Idle;
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

        protected abstract IEnumerator Load();
        protected abstract IEnumerator Unload();
    }
}