using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public class TargetCursorHandler : CursorHandler {

        [SerializeField] private GameObject cursorPrefab;
        [SerializeField] private GameObject augmentPrefab;

        protected override void InitializeCursor(UIButtonAnimator target) {
            GameObject cursorInstance;
            if (((TargetSelectHandler)stateAnimator.StateHandler).skillTransitionInfo.SkillPrep.bonbon == null) {
                cursorInstance = Instantiate(cursorPrefab, target.CursorTarget, false);
            }
            else {
                cursorInstance = Instantiate(augmentPrefab, target.CursorTarget, false);
            }
            cursorAnimator = cursorInstance.GetComponent<TargetCursorAnimator>();
            cursorAnimator.Init();
        }
    }
}