using System.Collections;
using UnityEngine;
using DG.Tweening;

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

    public class BonbonMenuAnimator : HandsOnStateAnimator {

        [SerializeField] private Transform[] trays;
        private BonbonMainHandler mainHandler;
        private BonbonBakeHandler bakeHandler;

        protected override void Awake() {
            mainHandler = GetComponent<BonbonMainHandler>();
            bakeHandler = GetComponent<BonbonBakeHandler>();

            foreach (UIAnimator animator in animators) animator.Init(this);
            
            mainHandler.OnHandlerToggle += OnMainHandlerToggle;
            bakeHandler.OnHandlerToggle += OnBakeHandlerToggle;
        }

        private void OnMainHandlerToggle(bool toggle) {
            StateHandler = mainHandler;
            base.UIStateHandler_OnHandlerToggle(toggle);
        }
        
        private void OnBakeHandlerToggle(bool toggle) {
            StateHandler = bakeHandler;
            base.UIStateHandler_OnHandlerToggle(toggle);
        }

        protected override IEnumerator Load() {
            foreach (Transform transform in trays) {
                
            } yield return null;
        }

        protected override IEnumerator Unload() {
            yield return null;
        }
    }

    public class BonbonSlotAnimator : UIButtonAnimator {
        
    }
}