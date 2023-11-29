using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class UITransitionManager {

        public UIStateHandler CurrHandler { get; private set; }
        public UIInputPack CurrInput { get; private set; }

        private Stack<UITransitionData> transitionStack;
        public UITransitionManager() {
            transitionStack = new(); 
        }

        public bool Reversible => transitionStack.Count > 1;

        public void Transition(UITransitionData data) => Transition(data.handler, data.input, true);

        public void Transition(UIStateHandler handler, UIInputPack input, bool softEnable = false) {
            CurrHandler = handler;
            CurrInput = input;
            if (softEnable) CurrHandler.SoftEnable();
            CurrInput.SelectedButton.Select();
            Debug.LogWarning(CurrInput.SelectedButton);
        }

        public void Record(UIStateHandler handler, UIInputPack input) {
            transitionStack.Push(new UITransitionData(handler, input));
        }

        public void RevertTo<T>() where T : UIStateHandler {
            while (CurrHandler.GetType() != typeof(T)
                   && Reversible) Revert();
        }

        public void Revert() {
            CurrHandler.Revert();
            UITransitionData prevData = transitionStack.Pop();
            Transition(prevData);
        }

        public void RevertAll() {
            CurrHandler.Revert();
            while (Reversible) transitionStack.Pop().handler.Revert();
            transitionStack = new();
        }
    }
}