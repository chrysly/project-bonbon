using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private static string _saveDataLocation;
    private static SaveData saveData;
    [SerializeField, DisableClassEdit] private SaveData saveDataView;
    [SerializeField] private SceneTransition sceneTransition;

    #region Events
    public event Action<SaveData> OnDataLoad;
    #endregion Events

    void Awake() {
        if (_saveDataLocation == null) {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string saveDirectory = Directory.CreateDirectory(Path.Join(basePath, "Bonbon")).FullName;
            _saveDataLocation = Path.Join(saveDirectory, "save.json");
        }
        OnDataLoad += TestFunction;
    }

    void Start() {
        if (saveData == null) {
            saveData = new SaveData();
        }
        saveData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        OnDataLoad.Invoke(saveData);
        saveDataView = saveData;
    }

    public void SaveToFile() {
        string jsonString = saveData.EncodeToJSON();
        Debug.Log(jsonString);
        File.WriteAllText(_saveDataLocation, jsonString);
    }

    public void LoadFromFile() {
        if (!File.Exists(_saveDataLocation)) {
            Debug.Log("No save file found...");
            return;
        }

        string jsonString = File.ReadAllText(_saveDataLocation);
        saveData.LoadFromJSON(jsonString);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (saveData.sceneIndex == currentSceneIndex) {
            OnDataLoad.Invoke(saveData);
        } else {
            sceneTransition.SwitchToScene(saveData.sceneIndex);
        }
    }

    private void TestFunction(SaveData saveData) {
        Debug.Log("Updating variables...");
    }
}
