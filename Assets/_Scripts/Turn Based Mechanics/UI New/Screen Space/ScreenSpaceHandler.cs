using System.Linq;
using UnityEngine;

namespace BattleUI {
    public class ScreenSpaceHandler : StateMachineHandler {

        [SerializeField] private GameObject characterAnchor;

        private ScreenSpaceElement[] elements;
        public event System.Action<float, Actor> OnDamage;
        public event System.Action<float, Actor> OnHeal;
        public event System.Action<float, Actor> OnStamina;

        public override void Initialize(BattleStateInput input) {
            base.Initialize(input);
            input.AnimationHandler.DamageEvent += (dmg, target, augment) => OnDamage?.Invoke(dmg, target);
            input.AnimationHandler.HealEvent += (heal, target) => OnHeal?.Invoke(heal, target);
            input.AnimationHandler.StaminaEvent += (value, target) => OnStamina?.Invoke(value, target);
            elements = GetComponentsInChildren<ScreenSpaceElement>(true);
            if (characterAnchor != null) {
                elements = elements.Concat(characterAnchor.GetComponentsInChildren<ScreenSpaceElement>(true)).ToArray();
            } else Debug.LogError("Assign the Character Anchor in the ScreenSpace State Machine Handler");
            foreach (ScreenSpaceElement element in elements) element.Init(this);
        }

        public Actor FetchActor(ActorData data) => input.ActorHandler.ActorList.FirstOrDefault(actor => actor.Data == data);
    }
}

