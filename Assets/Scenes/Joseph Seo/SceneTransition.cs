using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private int sceneWant;
    [SerializeField] private GameObject sliderPanel;
    [SerializeField] private Slider slider;
    private float currentValue;
    [SerializeField] private float progressMultiplier = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IncrementScene();
        JumpScene();
    }
    //Increment scenes up and down 1 index from the build index by pressing the respective arrow key
    public void IncrementScene()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings -1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                sliderPanel.SetActive(true);
                StartCoroutine(LoadSceneSync(SceneManager.GetActiveScene().buildIndex + 1));
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                sliderPanel.SetActive(true);
                StartCoroutine(LoadSceneSync(SceneManager.GetActiveScene().buildIndex - 1));
            }
        }
    }
    //Jump from scene to scene by using the serialized variable "sceneWant" and pressing down arrow
    public void JumpScene()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            sliderPanel.SetActive(true);
            StartCoroutine(LoadSceneSync(sceneWant));
            //SceneManager.LoadScene(sceneWant);
        }
    }
    
    

    IEnumerator LoadSceneSync(int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        while(!operation.isDone)
        {
            operation.allowSceneActivation = false;
            float progress = Mathf.Clamp01(operation.progress/0.9f);
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
