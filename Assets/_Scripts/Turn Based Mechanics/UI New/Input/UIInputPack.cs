using System.Linq;
using UnityEngine;

namespace BattleUI {
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
}