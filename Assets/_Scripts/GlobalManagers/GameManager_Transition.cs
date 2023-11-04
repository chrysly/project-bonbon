using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameManager {

    [SerializeField] private GameObject sliderPanel;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject loadingCanvas;
    private float currentValue;
    private float progressMultiplier = 0.5f;

    /// <summary>
    /// Transition to the next scene;
    /// </summary>
    public void TransitionToNext() => TransitionToLevel(SceneManager.GetActiveScene().buildIndex + 1);

    /// <summary>
    /// Transition to the scene designated by a given index;
    /// </summary>
    /// <param name="levelIndex"> Index of the scene to load; </param>
    public void TransitionToLevel(int levelIndex) => SetActiveScene(levelIndex);

    /// <summary>
    /// Initiate a coroutine with the corresponding loading sequence;
    /// </summary>
    /// <param name="sceneIndex"> Index of the scene to load; </param>
    private void SetActiveScene(int sceneIndex) {
        sliderPanel.SetActive(true);
        StartCoroutine(LoadSceneSync(sceneIndex));
        //fa = GameObject.FindGameObjectWithTag("loading").GetComponent<fadeInOut>();
        loadingCanvas.GetComponentInChildren<fadeInOut>(true).FadeIn();
        // loadingCanvas.FadeIn();
    }

    /// <summary>
    /// Coroutine to load scenes asynchronously;
    /// </summary>
    /// <param name="sceneToLoad"> Index of the scene to load; </param>
    private IEnumerator LoadSceneSync(int sceneToLoad)
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
        } //sliderPanel.SetActive(false);
    }
    
    void Start(){
        SceneManager.sceneLoaded+=OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("here");
        loadingCanvas.GetComponent<fadeInOut>().FadeOut();
        StartCoroutine(threesec());
        currentValue=0f;
        
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    IEnumerator threesec(){
        yield return new WaitForSeconds(1);
        loadingCanvas.SetActive(false);
    }

}
