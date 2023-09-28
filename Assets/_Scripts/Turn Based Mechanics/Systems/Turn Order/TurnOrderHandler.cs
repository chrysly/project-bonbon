using System.Collections.Generic;

public class TurnOrderHandler
{
    private TurnValueHeap TurnOrder;

    public TurnOrderHandler(List<Actor> actors) {
        TurnOrder = new TurnValueHeap(actors);
    }

    private void BuildTurnOrder(List<Actor> actors) {
        foreach (Actor actor in actors) {
            TurnOrder.Add(new TurnValueHandler(actor));
        }
    }
}
