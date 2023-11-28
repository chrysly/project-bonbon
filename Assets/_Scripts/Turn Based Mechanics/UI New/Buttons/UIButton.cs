using UnityEngine;

namespace BattleUI {
    public abstract class UIButton : MonoBehaviour {

        public abstract void Init<T>(T stateHandler) where T : UIStateHandler;

        public bool Available => IsAvailable();

        public virtual void Select() => OnSelect?.Invoke();

        public virtual void Activate() => OnActivate?.Invoke();

        public virtual bool IsAvailable() => true;

        public event System.Action OnSelect;
        public event System.Action OnActivate;
    }
}