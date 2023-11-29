using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleUI {

    public enum InTraversal { Undefined, Up, Down, Left, Right }
    public enum InAction { Confirm, Back, Pause }
    public enum TraversalMode { Cardinal, DiagonalToLeft, DiagonalToRight, Vertical, Horizontal }

    public class UIInput {

        public UIInputMap Input { get; private set; }
        public event System.Action<InTraversal> OnInputTraversal;
        public event System.Action<InAction> OnInputAction;

        public UIInput() {
            Input = new UIInputMap();
            Input.UIActionMap.Traverse.performed += UIInput_OnInputTraversal;
            Input.UIActionMap.Confirm.performed += UIInput_OnInputConfirm;
            Input.UIActionMap.Back.performed += UIInput_OnInputBack;
        }

        public void Enable() => Input.UIActionMap.Enable();
        public void Disable() => Input.UIActionMap.Disable();

        private void UIInput_OnInputTraversal(InputAction.CallbackContext context) {
            Vector2Int inputVec = context.ReadValue<Vector2>().Project2Cardinal();
            InTraversal output = inputVec == Vector2Int.up ? InTraversal.Up
                               : inputVec == Vector2Int.down ? InTraversal.Down
                               : inputVec == Vector2Int.left ? InTraversal.Left
                               : inputVec == Vector2Int.right ? InTraversal.Right
                               : InTraversal.Undefined;
            OnInputTraversal?.Invoke(output);
        }

        private void UIInput_OnInputConfirm(InputAction.CallbackContext context) => OnInputAction?.Invoke(InAction.Confirm);
        private void UIInput_OnInputBack(InputAction.CallbackContext context) => OnInputAction?.Invoke(InAction.Back);
    }
}