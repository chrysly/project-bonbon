using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Slider = UnityEngine.UI.Slider;

namespace BattleUI {
    public abstract class GenericBar : ScreenSpaceElement {

        [SerializeField] protected ActorData actorIdentifier;

        protected abstract int CurrPoints { get; }
        protected abstract int MaxPoints { get; }
        protected Actor actor;

        private Slider slider;
        private TextMeshProUGUI _text;

        protected float visualGauge;

        void Awake() {
            slider = GetComponent<Slider>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public override void Init(ScreenSpaceHandler handler) {
            base.Init(handler);
            actor = handler.FetchActor(actorIdentifier);
            if (actor == null) Destroy(gameObject);
            else {
                slider.value = CurrPoints;
                visualGauge = CurrPoints;
                if (_text != null) _text.text = visualGauge + " / " + MaxPoints;
            } RegisterInMachine();
        }

        protected void UpdateBar(float value, Actor actor) {
            if (actor.Data != actorIdentifier) return;
            visualGauge = value;
            float valueRatio = visualGauge / MaxPoints;
            DOTween.To(() => slider.value, x => slider.value = x, valueRatio, 0.5f);
            if (_text != null) _text.text = visualGauge + " / " + MaxPoints;
        }

        protected abstract void RegisterInMachine();
    }
}