using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BattleUI {
    public class BonbonCraftAnimator : HandsOnStateAnimator {
        
        [SerializeField] private Transform backdrop;
        [SerializeField] private Transform horizontalView;
        [SerializeField] private Transform ingredientDisplayPoint;
        [SerializeField] private float rollInOutSpeed = 0.3f;
        
        private Vector3 _originalPos;

        protected override void Awake() {
            base.Awake();
            (StateHandler as BonbonCraftHandler)
                .OnButtonArrange += BonbonCraftHandler_OnButtonArrange;
            _originalPos = backdrop.position;
        }

        protected override void UIStateHandler_OnHandlerToggle(bool toggle) {
            base.UIStateHandler_OnHandlerToggle(toggle);
            state = toggle ? UIAnimatorState.Loading : UIAnimatorState.Unloading;
        }
        
        private void BonbonCraftHandler_OnButtonArrange() {
            animators = GetComponentsInChildren<UIAnimator>();
            foreach (UIAnimator animator in animators) {
                animator.Init(this);
                animator.Toggle(true);
            }
        }

        protected override IEnumerator Load() {
            backdrop.DOMove(ingredientDisplayPoint.position, rollInOutSpeed);
            yield return null;
        }

        protected override IEnumerator Unload() {
            backdrop.DOMove(_originalPos, rollInOutSpeed);
            yield return null;
        }
    }
}
