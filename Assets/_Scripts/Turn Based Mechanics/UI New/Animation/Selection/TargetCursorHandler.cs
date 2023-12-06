using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class TargetCursorHandler : CursorHandler {

        [SerializeField] private GameObject cursorPrefab;

        protected override void InitializeCursor(UIButtonAnimator target) {
            GameObject cursorInstance = Instantiate(cursorPrefab, target.CursorTarget, false);
            cursorAnimator = cursorInstance.GetComponent<TargetCursorAnimator>();
            cursorAnimator.Init();
        }
    }
}