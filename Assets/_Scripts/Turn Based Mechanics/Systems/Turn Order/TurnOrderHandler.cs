using System;
using System.Collections.Generic;

public class TurnOrderHandler
{
    private TurnValueHeap TurnOrder;
    private const ushort MaxTurnsInDisplay = 6;
    
    public TurnOrderHandler() {
        TurnOrder = new TurnValueHeap();
    }
    public TurnOrderHandler(List<Actor> actors) {
        TurnOrder = new TurnValueHeap(actors);
    }
    
    public Actor Advance() {
        TurnValueHandler currentTurn;
        try {
            currentTurn = TurnOrder.Peek();
        } catch (IndexOutOfRangeException) {
            throw new EmptyTurnOrderException("Turn Order is empty! Make sure to populate TurnOrder using AddActor() " +
            "or pass in a list of actors to constructor!");
        }

        Actor currentActor = currentTurn.Actor;
        TurnOrder.FlatModifyTurnValueAll(-currentTurn.TurnValue());
        currentTurn.ResetActionMeter();
        TurnOrder.BuildTurnValueHeap();

        return currentActor;
    }
    public List<Actor> GetTurnDisplay() {
        List<Actor> turnDisplay = new List<Actor>();

        TurnValueHeap fastForwardCopy = new TurnValueHeap(TurnOrder);

        for (int i = 0; i < MaxTurnsInDisplay; i++) {
            TurnValueHandler current = fastForwardCopy.Peek();
            fastForwardCopy.FlatModifyTurnValueAll(-current.TurnValue());
            current.ResetActionMeter();
            fastForwardCopy.BuildTurnValueHeap();

            turnDisplay.Add(current.Actor);
        }

        return turnDisplay;
    }
    public void ModifyActor(Actor actor, int modifyTurnValue) {
        TurnOrder.FlatModifyTurnValue(actor, modifyTurnValue);
    }
    public void AddActor(Actor actor) {
        TurnOrder.Add(new TurnValueHandler(actor));
    }
    public void RemoveActor(Actor actor) {
        TurnOrder.RemoveActor(actor);
    }

    internal class EmptyTurnOrderException : Exception {
        public EmptyTurnOrderException() : base() {}
        public EmptyTurnOrderException(string message) : base(message) {}
    }
    internal class OutOfOrderException : Exception {
        public OutOfOrderException() : base() {}
        public OutOfOrderException(string message) : base(message) {}
    }
}
