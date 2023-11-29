using System.Collections;
using System.Collections.Generic;
using BattleUI;
using UnityEngine;

public class TargetAnimator : UIStateAnimator
{
    public void Init(UIButtonAnimator animator) {
        animator.Init(this);
    }
}
