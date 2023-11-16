
/// <summary>
/// Data class for actor-related turn order data;
/// </summary>
public class TurnOrderValue {
    public const int MAX_GAUGE = 1000;

    public readonly Actor actor;
    public readonly int priority;

    public int Speed => actor.ActiveData.Speed;
    public int actionGauge;

    public TurnOrderValue(Actor actor, int priority) {
        this.actor = actor;
        this.priority = priority;
    }

    public TurnOrderValue(TurnOrderValue tov) {
        actor = tov.actor;
        priority = tov.priority;
        actionGauge = tov.actionGauge;
    }

    public void ResetActionGauge() => actionGauge = MAX_GAUGE;
}