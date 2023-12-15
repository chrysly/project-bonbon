using UnityEngine;

namespace BattleUI {
    public class Healthbar : GenericBar {
        protected override int CurrPoints => actor.Hitpoints;
        protected override int MaxPoints => actor.Data.MaxHitpoints;

        protected override void RegisterInMachine() {
            handler.OnDamage += (value, target) => UpdateBar(Mathf.Clamp(visualGauge - value, 0, MaxPoints), target);
            handler.OnHeal += (value, target) => UpdateBar(Mathf.Clamp(visualGauge + value, 0, MaxPoints), target);
        }

        /*
        private Slider slider;
        private TextMeshProUGUI _text;
        private BattleStateMachine stateMachine => BattleStateMachine.Instance;
        [SerializeField] private ActorData actorIdentifier;
        [SerializeField] private Actor actor;
        void Start() {
            slider = GetComponent<Slider>();
            stateMachine.OnStateTransition += UpdateHealthBar;
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void UpdateHealthBar(BattleStateMachine.BattleState state, BattleStateInput input) {
            float currHealth = actor.Hitpoints;
            float maxHealth = actor.Data.MaxHitpoints;

            float healthRatio = currHealth / maxHealth;
            DOTween.To(() => slider.value, x => slider.value = x, healthRatio, 0.5f);
            _text.text = currHealth + " / " + maxHealth;
        }

        #if UNITY_EDITOR

        public ActorData ActorIdentifier => actorIdentifier;
        public Actor Actor { get => actor; set => actor = value; }

        #endif*/
    }
}