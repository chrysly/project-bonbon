using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ActorHandler : StateMachineHandler {

    [SerializeField] private ActorMap prefabMap;
    public ActorMap PrefabMap => prefabMap;
    public BattleStateInput CurrInput => input;

    /// <summary> A convenient way to pass down Screen Canvas references to Actors; </summary>
    [System.Serializable]
    public class SSCanvasRefs {
        public BattleUIStateMachine stateMachine;
        public BattleSkillWindow skillWindow;
        public IngredientSelectWindow ingredientWindow;

        public void SetupRefs(ref BattleUIStateMachine stateMachine, ref BattleSkillWindow skillWindow,
                              ref IngredientSelectWindow ingredientWindow) {
            stateMachine = this.stateMachine;
            skillWindow = this.skillWindow;
            ingredientWindow = this.ingredientWindow;
        }
    } [HideInInspector] 
    [SerializeField] private SSCanvasRefs screenCanvasRefs;
    public SSCanvasRefs ScreenCanvasRefs {
        get {
            if (screenCanvasRefs == null) screenCanvasRefs = new SSCanvasRefs();
            return screenCanvasRefs;
        }
    }

    [HideInInspector]
    [SerializeField] private CharacterSpace[] characterSpaces;
    [HideInInspector]
    [SerializeField] private EnemySpace[] enemySpaces;

    public List<Actor> ActorList => characterSpaces.Where(space => space.CurrActor != null)
                                                   .Select(space => space.CurrActor)
                                                   .Concat(enemySpaces.Where(space => space.CurrActor != null)
                                                                      .Select(space => space.CurrActor)).ToList();

    public override void Initialize(BattleStateInput input) {
        base.Initialize(input);
        foreach (CharacterSpace characterSpace in characterSpaces) {
            characterSpace.Init(this);
        } foreach (EnemySpace enemySpace in enemySpaces) {
            enemySpace.Init(this);
        } input.InitializeTurnOrder(ActorList);
    }

    public void InitializePrefab(GameObject actorPrefab) {
        Actor actor = actorPrefab.GetComponentInChildren<Actor>(true);
        actor.Init(CurrInput);
    }

    public void KillActor(Actor actor) {
        ActorSpace[] targetSpace = actor is CharacterActor ? characterSpaces : enemySpaces;
        ActorSpace space = targetSpace.FirstOrDefault(actorSpace => actorSpace.CurrActor == actor);
        if (space != null) space.DespawnActor();
        input.TurnOrderHandler.Remove(actor);
    }

    #if UNITY_EDITOR

    [HideInInspector] public Transform anchorTransform;

    public CharacterSpace[] EditorCharacterSpaces {
        get {
            if (characterSpaces == null) characterSpaces = new CharacterSpace[0];
            return characterSpaces;
        } set {
            characterSpaces = value;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    public EnemySpace[] EditorEnemySpaces {
        get {
            if (enemySpaces == null) enemySpaces = new EnemySpace[0];
            return enemySpaces;
        } set {
            enemySpaces = value;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    public void SetupCanvasReferences() {
        screenCanvasRefs.stateMachine = transform.parent.GetComponentInChildren<BattleUIStateMachine>(true);
        screenCanvasRefs.skillWindow = transform.parent.GetComponentInChildren<BattleSkillWindow>(true);
        screenCanvasRefs.ingredientWindow = transform.parent.GetComponentInChildren<IngredientSelectWindow>(true);
        UnityEditor.EditorUtility.SetDirty(this);
    }

    #endif
}