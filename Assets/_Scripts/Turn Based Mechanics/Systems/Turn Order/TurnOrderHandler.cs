using System.Collections;
using System.Collections.Generic;

/// <summary>
/// C# Object Handler for the Battle Turn Order;
/// </summary>
public class TurnOrderHandler {

    /// <summary> Unordered list of TOV packets, used to generate the turn order dynamically; </summary>
    private List<TurnOrderValue> turnQueue;

    /// <summary> A sequential variable to distinguish turn order priority for worst case scenarios; </summary>
    private int priority;
    /// <summary> Accessor for the turn priority. Increases the count on access; </summary>
    private int Priority => priority++;

    /// <summary>
    /// Creates a Turn Order Handler with TurnOrderValue packets for each actor in the given list;
    /// </summary>
    /// <param name="actorList"></param>
    public TurnOrderHandler(List<Actor> actorList) {
        turnQueue = new List<TurnOrderValue>();
        foreach (Actor actor in actorList) {
            turnQueue.Add(actor, Priority);
        }
    }

    /// <summary>
    /// Generate an ordered list of actors that will be return in subsequent turns;
    /// </summary>
    /// <param name="turns"> How many turns in the future to preview; </param>
    /// <returns> An ordered list of actors in the prospective turn order; </returns>
    public List<Actor> GetTurnPreview(int turns) {
        List<Actor> turnPreview = new List<Actor>();
        List<TurnOrderValue> queueReplica = new List<TurnOrderValue>();
        turnQueue.ForEach(tov => queueReplica.Add(new TurnOrderValue(tov)));
        for (int i = 0; i < turns; i++) turnPreview.Add(queueReplica.Advance());
        return turnPreview;
    }

    /// <summary>
    /// Advance the turn order and return the active actor;
    /// </summary>
    /// <returns> Actor selected for the new turn; </returns>
    public Actor Advance() => turnQueue.Advance();

    /// <summary>
    /// Add an actor to the Turn Order list;
    /// </summary>
    /// <param name="actor"> Actor to add; </param>
    public void Add(Actor actor) => turnQueue.Add(actor, Priority);

    /// <summary>
    /// Remove an actor from the Turn Order list;
    /// </summary>
    /// <param name="actor"> Actor to remove; </param>
    public void Remove(Actor actor) => turnQueue.Remove(actor); 
}