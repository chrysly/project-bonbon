using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace BattleUI {
    public class BonbonMenuAnimator : HandsOnStateAnimator {

        [SerializeField] private Transform[] decorations;
        private BonbonMainHandler mainHandler;
        private BonbonBakeHandler bakeHandler;

        protected override void Awake() {
            base.Awake();
            mainHandler = GetComponent<BonbonMainHandler>();
            bakeHandler = GetComponent<BonbonBakeHandler>();

            foreach (UIAnimator animator in animators) animator.Init(this);
            foreach (Transform transform in decorations) transform.DOScale(0, 0);
            
            mainHandler.OnHandlerToggle += OnMainHandlerToggle;
            bakeHandler.OnHandlerToggle += OnBakeHandlerToggle;
        }

        private void OnMainHandlerToggle(bool toggle) {
            StateHandler = mainHandler;
            state = toggle ? UIAnimatorState.Loading : UIAnimatorState.Unloading;
            base.UIStateHandler_OnHandlerToggle(toggle);
        }
        
        private void OnBakeHandlerToggle(bool toggle) {
            StateHandler = bakeHandler;
        }

        protected override IEnumerator Load() {
            foreach (Transform transform in decorations) {
                transform.DOScale(Vector2.one, animationDuration).SetEase(Ease.InOutCirc);
                yield return new WaitForSeconds(animationDuration / 2);
            } yield return null;
        }

        protected override IEnumerator Unload() {
            foreach (Transform transform in decorations) {
                transform.DOScale(Vector2.zero, animationDuration).SetEase(Ease.InOutQuart);
                yield return new WaitForSeconds(animationDuration / 2);
            } yield return null;
        }
    }
}