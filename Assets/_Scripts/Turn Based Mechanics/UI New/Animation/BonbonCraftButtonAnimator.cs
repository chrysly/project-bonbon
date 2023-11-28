using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleUI {
    public class BonbonCraftButtonAnimator : UIButtonAnimator {
        
        [SerializeField] private float selectDuration = 0.3f;
        
        public override void Init(UIStateAnimator stateAnimator) {
            base.Init(stateAnimator);
            this.stateAnimator.StateHandler.OnHandlerRevert += PseudoDestroy;
        }

        public void PseudoDestroy() {
            stateAnimator.StateHandler.OnHandlerRevert -= PseudoDestroy;
            Destroy(gameObject);
        }
        
        public override void Toggle(bool toggle) {
            base.Toggle(toggle);
            if (!toggle) return;
            BonbonCraftButton bonbonButton = Button as BonbonCraftButton;
            GetComponentInChildren<BonobnIconIdentifier>().GetComponent<RawImage>().texture =
                bonbonButton.Bonbon.texture;
            GetComponentInChildren<BonbonTextIdentifier>().GetComponent<TextMeshProUGUI>().text =
                bonbonButton.Bonbon.name;
        }
        
        protected override IEnumerator Idle() {
            if (selected) {
                transform.DOScale(1.1f, selectDuration);
                yield return new WaitForSeconds(selectDuration);
            } else {
                transform.DOScale(targetScale, selectDuration);
                yield return new WaitForSeconds(selectDuration);
            }
        }
        
        protected override IEnumerator Unload() {
            transform.DOScale(Vector2.zero, animationDuration);
            yield return new WaitForSeconds(animationDuration);
        }
    }
}
