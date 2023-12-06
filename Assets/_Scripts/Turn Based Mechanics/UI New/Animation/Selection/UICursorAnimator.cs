using System.Collections;
using UnityEngine;

namespace BattleUI {
    public abstract class UICursorAnimator : MonoBehaviour {

        protected UIButtonAnimator target;
        protected UIAnimatorState state;

        protected virtual void Awake() {
            StartCoroutine(CoreCoroutine());
        }

        public abstract void Init();

        public virtual void Refocus(UIButtonAnimator target) {
            if (this.target != null) Detach(this.target);
            this.target = target;
            Attach(target);
            state = UIAnimatorState.Loading;
        }

        protected virtual void Attach(UIButtonAnimator target) {
            transform.SetParent(target.transform, false);
            target.OnToggle += UIButtonAnimator_OnToggle;
            target.OnActivate += UIButtonAnimator_OnActivate;
        }

        protected virtual void Detach(UIButtonAnimator target) {
            target.OnToggle -= UIButtonAnimator_OnToggle;
            target.OnActivate -= UIButtonAnimator_OnActivate;
        }

        public virtual void UIButtonAnimator_OnToggle(bool toggle) {
            if (!toggle) state = UIAnimatorState.Unloading;
        }

        public virtual void UIButtonAnimator_OnActivate(bool toggle) { }

        protected IEnumerator CoreCoroutine() {
            while (true) {
                switch (state) {
                    case UIAnimatorState.Loading:
                        IEnumerator loadCoroutine = Load();
                        while (loadCoroutine.MoveNext())
                            yield return loadCoroutine.Current;
                        if (state == UIAnimatorState.Loading) state = UIAnimatorState.Idle;
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
                        break;
                } yield return null;
            }
        }

        protected abstract IEnumerator Load();
        protected abstract IEnumerator Idle();
        protected abstract IEnumerator Unload();
    }
}