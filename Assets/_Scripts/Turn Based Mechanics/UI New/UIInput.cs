using System.Linq;
using System.Collections.Generic;
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

    public class UIInputPack {

        public UIButton SelectedButton {
            get {
                try {
                    return inputSpace[verticalIndex][horizontalIndex];
                } catch (System.IndexOutOfRangeException) {
                    return null;
                }
            }
        }
        
        private int verticalIndex;
        private int horizontalIndex;
        private readonly UIButton[][] inputSpace;
        private readonly TraversalMode mode;

        public UIInputPack(UIButton[][] inputSpace, TraversalMode mode) {
            switch (mode) {
                case TraversalMode.Cardinal:
                case TraversalMode.Horizontal:
                    this.inputSpace = inputSpace;
                    break;
                case TraversalMode.DiagonalToRight:
                case TraversalMode.DiagonalToLeft:
                case TraversalMode.Vertical:
                    this.inputSpace = inputSpace[0].Select(arr => new[] { arr }).ToArray();
                    break;
            } this.mode = mode;
        }

        public void ProcessTraversal(InTraversal input) {
            UIButton priorButton = SelectedButton;
            switch (input.ParseInput(mode)) {
                case InTraversal.Up:
                    verticalIndex--;
                    verticalIndex = verticalIndex.mod(inputSpace.Length);
                    break;
                case InTraversal.Down:
                    verticalIndex++;
                    verticalIndex = verticalIndex.mod(inputSpace.Length);
                    break;
                case InTraversal.Left:
                    horizontalIndex--;
                    horizontalIndex = horizontalIndex.mod(inputSpace[verticalIndex].Length);
                    break;
                case InTraversal.Right:
                    horizontalIndex++;
                    horizontalIndex = horizontalIndex.mod(inputSpace[verticalIndex].Length);
                    break;
            } if (SelectedButton != null && priorButton != SelectedButton) {
                SelectedButton.Select();
            } Debug.LogWarning(SelectedButton);
        }
    }

    public static class UIInputUtils {

        public static int mod(this int x, int m) => (x % m + m) % m;

        public static Vector2Int Project2Cardinal(this Vector2 vec) {
            float[] vecArr = new[] { vec.x, vec.y };
            float max = Mathf.Abs(vec.x) > Mathf.Abs(vec.y) ? vec.x : vec.y;
            IEnumerable<int> projArr = vecArr.Select(coord => coord == max ? 1 * System.Math.Sign(max) : 0);
            return new Vector2Int(projArr.ElementAt(0), projArr.ElementAt(1));
        }

        public static InTraversal ParseInput(this InTraversal input, TraversalMode mode) {
            return input == InTraversal.Undefined ? InTraversal.Undefined : InputMap[mode][input];
        }

        public static readonly Dictionary<TraversalMode, Dictionary<InTraversal, InTraversal>> InputMap = new() {
            {
                TraversalMode.Cardinal,
                new() {
                    { InTraversal.Up, InTraversal.Up },
                    { InTraversal.Down, InTraversal.Down },
                    { InTraversal.Left, InTraversal.Left },
                    { InTraversal.Right, InTraversal.Right }
                }
            },
            {
                TraversalMode.DiagonalToLeft,
                new() {
                    { InTraversal.Up, InTraversal.Up },
                    { InTraversal.Down, InTraversal.Down },
                    { InTraversal.Left, InTraversal.Down },
                    { InTraversal.Right, InTraversal.Up }
                }
            },
            {
                TraversalMode.DiagonalToRight,
                new() {
                    { InTraversal.Up, InTraversal.Up },
                    { InTraversal.Down, InTraversal.Down },
                    { InTraversal.Left, InTraversal.Up },
                    { InTraversal.Right, InTraversal.Down }
                }
            },
            {
                TraversalMode.Vertical,
                new() {
                    { InTraversal.Up, InTraversal.Up },
                    { InTraversal.Down, InTraversal.Down },
                    { InTraversal.Left, InTraversal.Undefined },
                    { InTraversal.Right, InTraversal.Undefined }
                }
            },
            {
                TraversalMode.Horizontal,
                new() {
                    { InTraversal.Up, InTraversal.Undefined },
                    { InTraversal.Down, InTraversal.Undefined },
                    { InTraversal.Left, InTraversal.Left },
                    { InTraversal.Right, InTraversal.Right }
                }
            },
        };
    }
}