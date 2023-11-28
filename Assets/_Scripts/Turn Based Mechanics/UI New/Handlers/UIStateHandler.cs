using UnityEngine;
using PseudoDataStructures;

namespace BattleUI {

    /// <summary>
    /// Base class for a State Handler. Defines some data and enforces some behaviours;
    /// </summary>
    public abstract class UIStateHandler : MonoBehaviour {

        [SerializeField] protected TraversalMode traversalMode;
        [SerializeField] protected SMatrix<UIButton> buttonMatrix;

        public UIStateType Type { get; protected set; }
        public UIBrain Brain { get; protected set; }
        public Actor CurrActor => Brain.CurrActor;

        public virtual void Init(UIBrain brain) {
            Brain = brain;
            foreach (UIButton button in buttonMatrix.Values) button.Init(this);
        }

        public virtual void Transition<T>() where T : UIStateHandler => Brain.Transition<T>();

        public virtual UIInputPack Enable(BaseTransitionInfo info) {
            OnHandlerToggle?.Invoke(true);
            return InputArrangement();
        }

        public virtual void SoftEnable() => OnHandlerToggle?.Invoke(true);

        public virtual UIInputPack InputArrangement() => new UIInputPack(buttonMatrix.To2DArray(), traversalMode);

        public virtual void Disable() => OnHandlerToggle?.Invoke(false);

        public virtual void Revert() => Disable();

        public System.Action<bool> OnHandlerToggle;
    }
}