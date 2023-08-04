using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {
    [SerializeField] private BattleStateSystem battleStateSystem;

    [SerializeField] private List<CharacterActor> actorList;
    private Dictionary<CharacterActor, LinkedList<ActionDisplay>> actorDict;

    [SerializeField] private GameObject moveCursorPrefab;

    private bool isActive = false;

    private CharacterActor activeActor;

    private void Start() {
        actorDict = new Dictionary<CharacterActor, LinkedList<ActionDisplay>>();
        foreach (CharacterActor actor in actorList) {
            actorDict.Add(actor, new LinkedList<ActionDisplay>());
        }
        RegisterEvents();
    }

    private void Update() {
        if (isActive) {
            PlacementCursor();
        }
    }

    private void RegisterEvents() {
        battleStateSystem.OnSkillSelected += CreateSkillCursor;
        battleStateSystem.OnSkillConfirm += DisableSkillCursor;
        battleStateSystem.OnWaypointAdded += CreateMovePoint;
        battleStateSystem.OnUndoAction += RemoveLastDisplay;
        battleStateSystem.OnSwitchState += ClearAll;
    }

    public void CreateSkillCursor(SkillAction skill, CharacterActor actor) {
        ActionDisplay display = gameObject.AddComponent<ActionDisplay>();
        skill.getSkill().InitSkillDisplay(display);

        activeActor = actor;
        actorDict[activeActor].AddLast(display);

        isActive = true;
    }

    public void DisableSkillCursor(bool canceled) {
        if (canceled) {
            actorDict[activeActor].Last.Value.WipeDisplay();
            actorDict[activeActor].RemoveLast();
        }
        isActive = false;
    }

    public void RemoveLastDisplay() {
        if (actorDict[activeActor].Count > 0) {
            actorDict[activeActor].Last.Value.WipeDisplay();
            actorDict[activeActor].RemoveLast();
        }
    }

    public void PlacementCursor() {
        if (actorDict[activeActor].Count > 1) {
            actorDict[activeActor].Last.Value.RunDisplayPlacement(actorDict[activeActor].Last.Previous.Value.GetCursor());
        } else {
            actorDict[activeActor].Last.Value.RunDisplayPlacement(activeActor.transform);
        }
    }

    public void CreateMovePoint(Vector3 location, CharacterActor actor) {
        ActionDisplay display = gameObject.AddComponent<ActionDisplay>();
        display.CreateMoveDisplay(moveCursorPrefab, location);

        activeActor = actor;
        actorDict[activeActor].AddLast(display);
    }

    private void Clear() {
        activeActor = null;
        isActive = false;
        foreach (CharacterActor actor in actorList) {
            foreach(ActionDisplay display in actorDict[actor]) {
                display.WipeDisplay();
            }
            actorDict[actor].Clear();
        }
    }

    private void ClearAll(bool isEnabled) {
        if (isEnabled == false) {
            Clear();
        }
    }
}