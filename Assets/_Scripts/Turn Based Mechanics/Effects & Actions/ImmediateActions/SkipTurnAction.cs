using System;

[Serializable]
public class SkipTurnAction : ImmediateAction.EffectOnly {

    public override void Use(StatIteration activeData, Actor target = null, SkillAugment augment = null) {
        // Skip Turn;
    }

    #if UNITY_EDITOR

    protected override void DrawActionGUI() {
        UnityEngine.GUILayout.Label("The player turn will skipped for the duration of the Effect;");
    }

    #endif
}
