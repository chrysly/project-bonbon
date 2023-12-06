using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelaySkillDamageAnimation : DelaySkillPercentAnimation
{
    #if UNITY_EDITOR
    protected override void InnerGUI() {
        description = "damage";
        base.InnerGUI();
    }
    #endif
}
