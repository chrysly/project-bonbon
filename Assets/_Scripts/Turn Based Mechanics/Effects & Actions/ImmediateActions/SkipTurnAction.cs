using System;

[Serializable]
public class SkipTurnAction : ImmediateAction.EffectOnly {

    public override void Use(StatIteration activeData, Actor target = null) {
        target.ApplyState(Actor.ActorState.Paralyzed);
    }

    #if UNITY_EDITOR

    protected override void DrawActionGUI() {
        UnityEngine.GUILayout.Label("The player turn will skipped for the duration of the Effect;");
    }

    #endif
}
