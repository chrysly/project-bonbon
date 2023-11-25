using UnityEngine;

namespace BattleUI {
    public abstract class UIStateHandler : MonoBehaviour {

        public readonly UIStateType type;

        public System.Action<bool> OnStateToggle;
    }
}