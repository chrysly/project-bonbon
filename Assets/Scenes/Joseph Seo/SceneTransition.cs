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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentSceneIndex < SceneManager.sceneCountInBuildSettings -1)
            {
                SwitchToScene(currentSceneIndex + 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentSceneIndex > 0)
            {
                SwitchToScene(currentSceneIndex - 1);
            }
        }
    }
    //Jump from scene to scene by using the serialized variable "sceneWant" and pressing down arrow
    public void JumpScene()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SwitchToScene(sceneWant);
        }
    }
    
    
    IEnumerator LoadSceneSync(int sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress);
            //Debug.Log("-------------------------------------------------------------------------------------------------------------------------");
            slider.value = progress;
            yield return null;
        }
    }

    public void SwitchToScene(int sceneIndex) {
        sliderPanel.SetActive(true);
        StartCoroutine(LoadSceneSync(sceneIndex));
    }
}
