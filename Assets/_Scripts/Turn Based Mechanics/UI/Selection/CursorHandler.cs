using UnityEngine;

namespace BattleUI {
    public abstract class CursorHandler : MonoBehaviour {
        public abstract void FocusEntity(Transform target);
        public virtual void Deactivate() { }
    }
}
