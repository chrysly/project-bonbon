using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASEUtilities {

    public enum SpaceWarning { InvalidDataType, MissingPrefabMapping, InvalidPrefabStructure, NotPrewarmed }
    public enum PrefabWarning { InvalidActorScript, InvalidUIHandler }

    public static class ASEUtils {

        public readonly static Dictionary<SpaceWarning, string> SpaceWarningText = new Dictionary<SpaceWarning, string>() {
            { SpaceWarning.InvalidDataType, "ActorData Type does not match the Space Type;" },
            { SpaceWarning.MissingPrefabMapping, "Missing Prefab Mapping for the ActorData;" },
            { SpaceWarning.InvalidPrefabStructure, "The Prefab Structure is invalid and can't be instantiated;" },
            { SpaceWarning.NotPrewarmed, "An Initial Actor has been assigned to the Space, but the Model has not been spawned;" +
                                         "\nConsider instantiating the model in the Editor to streamline loading at runtime;" }
        };

        public readonly static Dictionary<PrefabWarning, string> PrefabWarningText = new Dictionary<PrefabWarning, string>() {
            { PrefabWarning.InvalidActorScript, "Invalid Actor Script. Potential Causes:" +
                                                "\n  • No Actor Components were found;" +
                                                "\n  • More than one Actor Component was found;" +
                                                "\n  • The Data of the Actor Component does not match the data map;"},
            { PrefabWarning.InvalidUIHandler, "Invalid Actor Scripts. Causes:" +
                                              "\n  • None known;" },
        };

        public static Dictionary<ActorSpace, List<SpaceWarning>> VerifyHandlerStatus(ActorHandler actorHandler) {
            Dictionary<ActorSpace, List<SpaceWarning>> warningMap = new Dictionary<ActorSpace, List<SpaceWarning>>();
            foreach (ActorSpace space in actorHandler.EditorCharacterSpaces) {
                warningMap[space] = VerifySpaceStatus(space, actorHandler);
            } foreach (ActorSpace space in actorHandler.EditorEnemySpaces) {
                warningMap[space] = VerifySpaceStatus(space, actorHandler);
            } return warningMap;
        }

        /// <summary>
        /// Clean up all invalid spaces in the space structures; 
        /// </summary>
        /// <param name="actorHandler"> Handler whose structures will be cleaned; </param>
        /// <returns> True if at least an entry was cleaned up, false otherwise; </returns>
        public static bool CleanupHandlerStructure(ref ActorHandler actorHandler) {
            actorHandler.EditorCharacterSpaces = CleanupSpaceStructure(actorHandler.EditorCharacterSpaces, out bool dirtyCSpaces);
            actorHandler.EditorEnemySpaces = CleanupSpaceStructure(actorHandler.EditorEnemySpaces, out bool dirtyESpaces);
            if (dirtyCSpaces || dirtyESpaces) {
                FixSpaceNotation(actorHandler.EditorCharacterSpaces);
                FixSpaceNotation(actorHandler.EditorEnemySpaces);
                return true;
            } return false;
        }

        public static List<SpaceWarning> VerifySpaceStatus(ActorSpace space, ActorHandler actorHandler) {
            List<SpaceWarning> warnings = new List<SpaceWarning>();
            if (space == null) return warnings;
            if (space.InitialActor == null) return warnings;
            if ((space is CharacterSpace && space.InitialActor is not CharacterData)
                || (space is EnemySpace && space.InitialActor is not EnemyData)) {
                warnings.Add(SpaceWarning.InvalidDataType);
            } GameObject prefab = actorHandler.PrefabMap.PseudoActorMap.ContainsKey(space.InitialActor)
                                  ? actorHandler.PrefabMap.PseudoActorMap[space.InitialActor] : null;
            bool invalidPrefab = prefab == null;
            if (invalidPrefab) {
                warnings.Add(SpaceWarning.MissingPrefabMapping);
            } else if (!invalidPrefab && VerifyPrefabStatus(prefab, space.InitialActor).Count > 0) {
                warnings.Add(SpaceWarning.InvalidPrefabStructure);
            } else if (space.ActorPrefab == null) warnings.Add(SpaceWarning.NotPrewarmed);
            return warnings;
        }

        public static List<PrefabWarning> VerifyPrefabStatus(GameObject prefab, ActorData actorData) {
            List<PrefabWarning> warnings = new List<PrefabWarning>();
            Actor[] actors = prefab.GetComponentsInChildren<Actor>(true);
            if (actors.Length != 1 || actors[0].Data != actorData) warnings.Add(PrefabWarning.InvalidActorScript);
            return warnings;
        }

        private static T[] CleanupSpaceStructure<T>(T[] spaces, out bool dirty) where T : ActorSpace {
            T[] cleanSpaceArr = spaces.Where(space => space != null).ToArray();
            dirty = cleanSpaceArr.Length != spaces.Length;
            return cleanSpaceArr;
        }

        /// <summary>
        /// Fix the notation of Actor Space game objects; <br></br>
        /// Warning! The space structure must be free of invalid entries;
        /// </summary>
        /// <param name="spaces"> Actor Space array to fix the notation for; </param>
        private static void FixSpaceNotation<T>(T[] spaces) where T : ActorSpace {
            for (int i = 0; i < spaces.Length; i++) {
                spaces[i].gameObject.name = $"{SpacePrefix(typeof(T))} {i + 1}";
            }
        }

        public static string SpacePrefix(System.Type spaceType) => spaceType == typeof(CharacterSpace) ? "Character"
                                                           : spaceType == typeof(EnemySpace) ? "Enemy" : "Undefined";
    }
}