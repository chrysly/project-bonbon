using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BattleUI {
    public partial class BonbonSlotAnimator : UIButtonAnimator {

        private BonbonObject Bonbon {
            get {
                if (Button == null) return null;
                if (Button is BonbonSlotButton) {
                    return (Button as BonbonSlotButton).Bonbon;
                } else if (Button is BonbonBakeSlotButton) {
                    return (Button as BonbonBakeSlotButton).Bonbon;
                } else return null;
            }
        } private BonbonSlotButton slotButton;
        private BonbonBakeSlotButton bakeButton;
        private Coroutine specialAnimation;
        private Sprite defaultSprite;

        public int Slot => Button is BonbonSlotButton ? (Button as BonbonSlotButton).Slot
                                                      : (Button as BonbonBakeSlotButton).Slot;
        private SpriteRenderer icon;

        private bool destroyed;

        protected override void Awake() {
            base.Awake();
            icon = GetComponent<SpriteRenderer>();
            defaultSprite = icon.sprite;
        }

        protected override void LoadLogicButton() {
            slotButton = GetComponent<BonbonSlotButton>();
            slotButton.OnSelect += UIButton_OnSelect;
            slotButton.OnActivate += UIButton_OnActivate;
            bakeButton = GetComponent<BonbonBakeSlotButton>();
            bakeButton.OnSelect += UIButton_OnSelect;
            bakeButton.OnActivate += UIButton_OnActivate;
        }

        public override void Toggle(bool toggle) {
            bool isMain = stateAnimator.StateHandler is BonbonMainHandler;
            Button = isMain ? slotButton : bakeButton;
            UpdateIcon();
            ResolveAnimations();
            if (isMain) base.Toggle(toggle);
            else {
                if (!toggle) {
                    selected = false;
                    if (!destroyed) GetComponent<SpriteRenderer>().material.DOFloat(icon.sprite == null ? 1 : 1, "_Alpha" ,animationDuration / 2);
                } else ProcessAvailability();
            }
        }

        public void ToggleWithAnimation(BonbonFXInfo info) {
            ResolveAnimations();
            Button = stateAnimator.StateHandler is BonbonMainHandler ? slotButton : bakeButton;
            IEnumerator animation = info is BonbonCraftInfo
                                    ? CraftAnimation(info as BonbonCraftInfo)
                                    : BakeAnimation(info as BonbonBakeInfo);
            specialAnimation = StartCoroutine(animation);
            state = UIAnimatorState.Special;
            targetScale = 1;
        }

        private void UpdateIcon() {
            icon.sprite = Bonbon == null ? null : Bonbon.Texture;
        }

        protected override void ProcessAvailability() {
            if (!destroyed) GetComponent<SpriteRenderer>().material.DOFloat(icon.sprite == null ? 1 
                                             : Button.Available ? 1 : 0.5f, "_Alpha" ,0);
        }

        protected override IEnumerator Idle()
        {
            if (selected)
            {
                transform.DOScale(selectedScale, animationDuration);
                yield return new WaitForSeconds(animationDuration / 2);
                icon.sprite = Bonbon == null ? defaultSprite : Bonbon.Texture;
            }
            else
            {
                transform.DOScale(new Vector3(targetScale, targetScale, 1f), animationDuration);
                yield return new WaitForSeconds(animationDuration / 2);
                icon.sprite = Bonbon == null ? null : Bonbon.Texture;
            }
        }

        void OnDisable() => destroyed = true;
    }
}