using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelaySkillHealAnimation : DelaySkillPercentAnimation
{
    #if UNITY_EDITOR
    protected override void InnerGUI() {
        description = "healing";
        base.InnerGUI();
    }
    #endif
}
