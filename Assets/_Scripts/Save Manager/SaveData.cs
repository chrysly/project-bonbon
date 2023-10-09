using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int sceneIndex = 0;

    public string EncodeToJSON() {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJSON(string jsonString) {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }
}
