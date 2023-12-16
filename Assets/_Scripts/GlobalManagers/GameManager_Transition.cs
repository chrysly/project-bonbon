using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameManager {

    private Slider slider;

    private FadeHandler fadeHandler;

    private float progressMultiplier = 0.5f;

    void Start() {
        fadeHandler = GetComponentInChildren<FadeHandler>();
        slider = GetComponentInChildren<Slider>(true);

        SceneManager.sceneLoaded += OnSceneLoaded;
        fadeHandler.Fade(0f, false);
    }

    /// <summary>
    /// Transition to the next scene;
    /// </summary>
    public void TransitionToNext(bool includeLoadingBar = true) => TransitionToLevel(SceneManager.GetActiveScene().buildIndex + 1, includeLoadingBar);

    /// <summary>
    /// Transition to the scene designated by a given index;
    /// </summary>
    /// <param name="levelIndex"> Index of the scene to load; </param>
    public void TransitionToLevel(int levelIndex, bool load = true) => SetActiveScene(levelIndex, load);

    /// <summary>
    /// Initiate a coroutine with the corresponding loading sequence;
    /// </summary>
    /// <param name="sceneIndex"> Index of the scene to load; </param>
    private void SetActiveScene(int sceneIndex, bool load = true) {
        StartCoroutine(LoadSceneSync(sceneIndex));
        fadeHandler.Fade(1f, load);
    }

    /// <summary>
    /// Coroutine to load scenes asynchronously;
    /// </summary>
    /// <param name="sceneToLoad"> Index of the scene to load; </param>
    private IEnumerator LoadSceneSync(int sceneToLoad) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        float currentValue = 0;
        while (!operation.isDone) {
            operation.allowSceneActivation = false;
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            currentValue = Mathf.MoveTowards(currentValue, progress, progressMultiplier * Time.unscaledDeltaTime);
            slider.value = currentValue;
            if (Mathf.Approximately(currentValue, 1)) {
                operation.allowSceneActivation = true;
            } yield return null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => fadeHandler.Fade(0f);


    // (Ryan) Sorry for temp wack Carlos :)
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            TransitionToLevel(0);
        }
    }
}
