using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameManager {

    [SerializeField] private GameObject sliderPanel;
    [SerializeField] private Slider slider;
    private float currentValue;
    private float progressMultiplier = 0.5f;
    public enum CoreScene {
        MainMenu = 0,
        Campfire = 1,
        Cutscene = 2,
    }
    private int reservedIndices = 2;

    public void TransitionToCore(CoreScene coreScene) {
        SetActiveScene((int) coreScene);
    }

    public void TransitionToLevel(int levelIndex) {
        CurrLevel = levelIndex;
        SetActiveScene(levelIndex + reservedIndices);
    }

    private void SetActiveScene(int sceneIndex) {
        //SceneManager.LoadScene(sceneIndex);
        Debug.Log("1");
        sliderPanel.SetActive(true);
        Debug.Log("2");
        StartCoroutine(LoadSceneSync(sceneIndex));
        Debug.Log("3");
    }

    IEnumerator LoadSceneSync(int sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!operation.isDone)
        {
            operation.allowSceneActivation = false;
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            currentValue = Mathf.MoveTowards(currentValue, progress, progressMultiplier * Time.deltaTime);
            slider.value = currentValue;
            if (Mathf.Approximately(currentValue, 1))
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
