﻿using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace BattleUI {

    public enum UIAnimatorState {  Idle, Loading, Unloading, Special }

    public abstract class UIAnimator : MonoBehaviour {

        protected float animationDuration = 0.15f;

        protected UIStateAnimator stateAnimator;

        protected UIAnimatorState state;

        protected virtual void Awake() {
            transform.DOScale(new Vector3(0f, 0f, 1f), 0);
            StartCoroutine(CoreCoroutine());
        }

        public virtual void Init(UIStateAnimator stateAnimator) {
            this.stateAnimator = stateAnimator;
            this.stateAnimator.Brain.OnGlobalSoftToggle += Toggle;
            if (stateAnimator.Brain.CinematicMode) Destroy(this);
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

        protected virtual IEnumerator Load() {
            transform.DOScale(Vector3.one, animationDuration);
            yield return new WaitForSeconds(animationDuration);
        }

        protected virtual IEnumerator Idle() {
            yield return null;
        }

        protected virtual IEnumerator Unload() {
            transform.DOScale(new Vector3(0f, 0f, 1f), animationDuration);
            yield return new WaitForSeconds(animationDuration);
        }

        public virtual void Toggle(bool toggle) {
            state = toggle ? UIAnimatorState.Loading : UIAnimatorState.Unloading;
        }
    }
}