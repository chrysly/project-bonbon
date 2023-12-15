using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
 
namespace BattleUI {
    public class BonbonTrayAnimator : UIAnimator {

        [SerializeField] private List<MaskableGraphic> graphics;

        protected override void Awake() {
            SetGraphicAlpha(0, 0);
            StartCoroutine(CoreCoroutine());
        }

        protected override IEnumerator Load() {
            SetGraphicAlpha(1, animationDuration);
            yield return new WaitForSeconds(animationDuration);
        }

        protected override IEnumerator Idle() {
            yield return null;
        }

        protected override IEnumerator Unload() {
            SetGraphicAlpha(0, animationDuration);
            yield return new WaitForSeconds(animationDuration);
        }

        private void SetGraphicAlpha(float value, float duration) {
            graphics.ForEach(graphic => graphic.DOFade(value, duration));
        }
    }
}