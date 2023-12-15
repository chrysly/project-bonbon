using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Slider = UnityEngine.UI.Slider;

namespace BattleUI {
    public abstract class GenericBar : ScreenSpaceElement {

        [SerializeField] protected ActorData actorIdentifier;
        protected BattleStateMachine BattleStateMachine => BattleStateMachine.Instance;
        protected abstract int MaxPoints { get; }
        protected Actor actor;

        private Slider slider;
        private TextMeshProUGUI _text;

        private float visualGauge;

        void Awake() {
            slider = GetComponent<Slider>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void UpdateBar(float value) {
            float currHealth = visualGauge;
            float maxHealth = actor.Data.MaxHitpoints;

            float healthRatio = currHealth / maxHealth;
            DOTween.To(() => slider.value, x => slider.value = x, healthRatio, 0.5f);
            _text.text = currHealth + " / " + maxHealth;
        }
    }
}
