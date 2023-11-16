using System.Linq;
using System.Collections;
using UnityEngine;

public class ActorHandler : StateMachineHandler {

    [SerializeField] private ActorMap prefabMap;
    public ActorMap PrefabMap => prefabMap;

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

    private Actor[] actorList;

    public void InitializePrefab(GameObject actorPrefab) {
        Actor actor = actorPrefab.GetComponentInChildren<Actor>(true);
        if (actor is CharacterActor) {
            UIAnimationHandler uHandler = actorPrefab.GetComponentInChildren<UIAnimationHandler>(true);
            ScreenCanvasRefs.SetupRefs(ref uHandler._stateMachine, ref uHandler.skillWindow, ref uHandler.ingredientWindow);
        } actor.InjectHandler(this);
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
        } set => enemySpaces = value;
    }

    public void SetupCanvasReferences() {
        screenCanvasRefs.stateMachine = transform.parent.GetComponentInChildren<BattleUIStateMachine>(true);
        screenCanvasRefs.skillWindow = transform.parent.GetComponentInChildren<BattleSkillWindow>(true);
        screenCanvasRefs.ingredientWindow = transform.parent.GetComponentInChildren<IngredientSelectWindow>(true);
    }

    #endif
}