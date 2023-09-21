using System;

[Serializable]
public class SkipTurnAction : ImmediateAction {

    public override void Use(StatIteration activeData, Actor target = null) {
        // Skip Turn;
    }
}
