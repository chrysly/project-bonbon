using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager {

    public enum CoreScene {
        MainMenu = 0,
        Campfire = 1,
        Cutscene = 2,
    }

    private static int reservedIndices = 2;

    public static void TransitionToCore(CoreScene coreScene) {
        SetActiveScene((int) coreScene);
    }

    public static void TransitionToLevel(int levelIndex) {
        CurrLevel = levelIndex;
        SetActiveScene(levelIndex + reservedIndices);
    }

    private static void SetActiveScene(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }
}
