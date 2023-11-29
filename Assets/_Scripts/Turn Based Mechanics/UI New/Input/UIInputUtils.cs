using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
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