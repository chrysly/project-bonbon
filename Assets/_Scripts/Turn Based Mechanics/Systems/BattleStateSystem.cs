using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, CHARSELECT, PATHSELECT, SKILLSELECT, BAKE, ANIMATE, WIN, LOSE }

public class BattleStateSystem : MonoBehaviour
{

    [SerializeField] private SelectManager selector;
    [SerializeField] private CharacterPathHandler pathHandler;
    [SerializeField] private ControllerDisabler disabler;
    [SerializeField] private ActorMovementHandler animator;
    [SerializeField] private CursorManager cursorManager;

    private IEnumerator activeAnimationCycle;
    private int maxSteps;
    private int animationStep; 

    public BattleState battleState;

    #region Battle Actors
    [SerializeField] private List<CharacterActor> actorList;
    [SerializeField] private List<EnemyActor> enemyList;
    private CharacterActor activeActor = null;
    private List<CharacterActor> actorQueue;

    [SerializeField] private float animationCycleDuration = 0.5f;
    private bool isAnimating = false;

    #endregion Battle Actors

    #region Battle Event Timing
    [SerializeField] private float switchToStartDelay = 1f;
    [SerializeField] private float battleStartAnimationDuration = 0.5f;
    [SerializeField] private float characterSelectTransitionDelay = 1f;
    #endregion Battle Event Timing

    #region Events
    public delegate void SkillSelected(SkillAction action, CharacterActor actor);
    public event SkillSelected OnSkillSelected;

    public delegate void SkillConfirm(bool canceled);
    public event SkillConfirm OnSkillConfirm;

    public delegate void WaypointAdded(Vector3 location, CharacterActor actor);
    public event WaypointAdded OnWaypointAdded;

    public delegate void UndoAction();
    public event UndoAction OnUndoAction;

    public delegate void Selection(bool enable);
    public event Selection OnSwitchState;

    public delegate void IsBaking();
    public event IsBaking OnBake;
    #endregion Events

    private SkillAction activeSkill;

    // Start is called before the first frame update
    void Start()
    {
        battleState = BattleState.START;
        selector.OnSelect += SwitchToPathSelect;
        StartCoroutine(InitializeBattle());
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();
    }

    private void HandleState() {
        switch (battleState) {
            case BattleState.START:
                StartCoroutine(BattleStart());
                break;
            case BattleState.CHARSELECT:
                CharacterSelection();
                break;
            case BattleState.PATHSELECT:
                PathSelect();
                break;
            case BattleState.SKILLSELECT:
                SkillSelect();
                break;
            case BattleState.BAKE:
                Bake();
                break;
            case BattleState.ANIMATE:
                Animate();
                break;
        }
    }

    private IEnumerator InitializeBattle() {

        maxSteps = actorList.Count;
        animationStep = 0;

        actorQueue = new List<CharacterActor>(actorList);
        actorQueue.Sort();

        //TODO: Initialize actors at actor spawn points
        yield return new WaitForSeconds(switchToStartDelay);

        SwitchToStart();

        yield return null;
    }

    #region Game States
    private void SwitchToStart() {
        battleState = BattleState.START;
        selector.OnDeselect += SwitchToCharacterSelect;
        Debug.Log("Switching to START");
    }

    private IEnumerator BattleStart() {
        //Battle intro animation
        yield return new WaitForSeconds(battleStartAnimationDuration);

        SwitchToCharacterSelect();

        yield return null;
    }

    public void SwitchToCharacterSelect() {
        if (!isAnimating) {
            battleState = BattleState.CHARSELECT;
            //pathHandler.DisableWaypoints();
            disabler.EnableControllers();
            Debug.Log("Switching to CHARSELECT");
            OnSwitchState.Invoke(true);
        }

        //EVENTS
    }

    private void CharacterSelection() {
        //something here
    }

    public void SwitchToPathSelect(CharacterActor actor) {
        activeActor = actor;
        battleState = BattleState.PATHSELECT;
        activeSkill = null;
        Debug.Log("Switching to PATHSELECT");
    }

    private void PathSelect() {

        if (activeActor == null) {
            SwitchToCharacterSelect();
        }

        //pathHandler.DisplayWaypoints(activeActor);
        //TODO: Invoke OnActionUpdate
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift)) {
            Vector3 waypoint = pathHandler.AddWaypoint(activeActor);
            if (waypoint != Vector3.zero && !activeActor.HasExistingMoveAction()) {
                OnWaypointAdded?.Invoke(waypoint, activeActor);
            }
        } else if (Input.GetMouseButtonUp(1)) {
            OnUndoAction?.Invoke();
            pathHandler.UndoWaypoint(activeActor);
        }
    }

    public void SwitchToSkillSelect(SkillObject skill) {
        if (activeActor.HasAvailableActions()) {
            battleState = BattleState.SKILLSELECT;
            activeSkill = new SkillAction(skill, activeActor.transform);
            OnSkillSelected?.Invoke(activeSkill, activeActor);
            Debug.Log("Switching to SKILLSELECT: " + skill.GetSkillName());
        }
    }

    private void SkillSelect() {

        if (activeActor == null) {
            SwitchToCharacterSelect();
            activeSkill = null;
            OnSkillConfirm?.Invoke(true);
        }

        if (Input.GetMouseButtonDown(1)) {
            activeSkill = null;
            SwitchToCharacterSelect();
            OnUndoAction?.Invoke();
        }


        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
                OnSkillConfirm?.Invoke(false);
                activeSkill.StoreLocation(hit.point);
                activeActor.AppendAction(activeSkill);
                SwitchToPathSelect(activeActor);
            }
        }
    }

    public void SwitchToBake(Ingredient ingredient) {
        if (activeActor.HasAvailableActions()) {
            battleState = BattleState.BAKE;
        }
    }

    private void Bake() {

    }

    public void SwitchToAnimate() {
        //activeActor = null;
        selector.OnDeselect -= SwitchToCharacterSelect;
        activeActor = null;
        activeSkill = null;
        battleState = BattleState.ANIMATE;
        animationStep = 0;
        //pathHandler.DisableWaypoints();
        disabler.DisableControllers();
        OnSwitchState.Invoke(false);
    }

    private void Animate() {
        if (activeAnimationCycle == null) {
            activeAnimationCycle = AnimateBattle();
            StartCoroutine(AnimateBattle());
        }
    }

    private IEnumerator AnimateBattle() {
        //additional actions during animate state

        if (animationStep > maxSteps) {
            isAnimating = false;
            animationStep = 0;
            selector.OnDeselect += SwitchToCharacterSelect;
            SwitchToCharacterSelect();
        }

        if (animationStep > 0) {
            CharacterActor prevActor = actorQueue[animationStep - 1];
            if (prevActor.GetActionList().Count > 0) {
                prevActor.RunNextAction(animationCycleDuration);
            }
        }
        CharacterActor currActor = actorQueue[animationStep];
        if (currActor.GetActionList().Count > 0) {
            currActor.RunNextAction(animationCycleDuration);
        }
        animationStep++;

        yield return new WaitForSeconds(animationCycleDuration + 0.1f);

        activeAnimationCycle = null;
        yield return null;
    }

    #endregion Game States
}
