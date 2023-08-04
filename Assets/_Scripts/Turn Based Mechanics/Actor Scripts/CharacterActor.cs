using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterActor : MonoBehaviour, IComparable<CharacterActor>
{
    #region Static Attributes
    [SerializeField] private CharacterData data;
    [SerializeField] private float durationBetweenWaypoints = 2f;
    #endregion Static Attributes

    #region Variable Attributes
    private int hitpoints;
    private int stamina;

    private MoveAction moveAction = null;
    //Pass Action
    private LinkedList<ActorAction> actionList;
    #endregion Variable Attributes

    // Start is called before the first frame update
    void Start()
    {
        InitializeAttributes();
    }

    void InitializeAttributes() {
        hitpoints = data.MaxHitpoints();
        stamina = data.MaxStamina();
        actionList = new LinkedList<ActorAction>();
    }

    public void AppendAction(ActorAction action) {
        if (HasRemainingStamina(action.GetCost()) && IsValidAction(action)) {
            UpdateSingleUseAction(action);
            actionList.AddLast(action);
            ConsumeActionPoints(action.GetCost());
        }
    }

    public void UndoLastAction() {
        if (actionList.Count > 0) {
            RefundStamina(actionList.Last.Value.GetCost());

            if (actionList.Last.Value is MoveAction) {
                moveAction = null;
            }

            actionList.RemoveLast();
        }
    }

    public void RunNextAction(float duration) {
        if (actionList.Count > 0) {
            actionList.First.Value.RunAction(transform, duration);
            actionList.RemoveFirst();
        } else {
            Debug.LogError("Next action called on empty action list");
        }
    }

    public bool IsValidAction(ActorAction action) {
        if (action is MoveAction) {
            return moveAction == null;
        }
        return true;
    }

    private void UpdateSingleUseAction(ActorAction action) {
        if (action is MoveAction move) {
            moveAction = move;
        } //TODO: ADD MORE ACTIONS
    }

    public bool ActionQueueIsEmpty() {
        return actionList.Count <= 0;
    }

    public bool HasRemainingStamina() {
        return stamina > 0;
    }

    public bool HasRemainingStamina(int cost) {
        return stamina - cost > 0;
    }

    public void ConsumeActionPoints(int cost) {
        if (!HasRemainingStamina(cost)) {
            Debug.LogError("Prompting action that costs more than the current character's action points");
        } else {
            stamina -= cost;
        }
    }

    public void RefundStamina(int cost) {
        if (stamina + cost > data.MaxStamina()) {
            stamina = data.MaxStamina();
        } else {
            stamina += cost;
        }
    }

    public LinkedList<ActorAction> GetActionList() {
        return actionList;
    }

    public CharacterData Data() {
        return data;
    }

    public bool HasExistingMoveAction() {
        return moveAction != null;
    }

    public bool HasAvailableActions() {
        return actionList.Count < data.MaxActions();
    }

    public int CompareTo(CharacterActor actor) {
        return data.BaseSpeed().CompareTo(actor.Data().BaseSpeed());
    }

    public override bool Equals(object obj) {
        var item = obj as CharacterActor;

        if (item == null) {
            return false;
        }

        return item.Data().ID() == data.ID();
    }
}
