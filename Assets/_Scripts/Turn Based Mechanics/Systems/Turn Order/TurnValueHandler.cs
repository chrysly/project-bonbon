using System;

public class TurnValueHandler : IComparable<TurnValueHandler> {
    private const ushort DefaultActionMeter = 10000;
    public int ActionMeter;
    public int Speed => Actor != null ? Actor.Data.BaseSpeed : 0;
    public Actor Actor;

    public TurnValueHandler(Actor actor) {
        Actor = actor;
        ActionMeter = DefaultActionMeter;
    }

    public int TurnValue() {
        return ActionMeter / Speed;
    }
    public int CompareTo(TurnValueHandler incoming) {
        if (incoming == null) {
            return 0;
        }
        int turnValueCompare = this.TurnValue().CompareTo(incoming.TurnValue());
        if (turnValueCompare == 0) {
            return -this.Speed.CompareTo(incoming.Speed);
        }
        return this.TurnValue().CompareTo(incoming.TurnValue());
    }
    public void ResetActionMeter() {
        ActionMeter = DefaultActionMeter;
    }
}