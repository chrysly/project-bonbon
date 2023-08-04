using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDisabler : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToDisable;

    public void EnableControllers() {
        foreach (GameObject obj in objectsToDisable) {
            obj.SetActive(true);
        }
    }

    public void DisableControllers() {
        foreach (GameObject obj in objectsToDisable) {
            obj.SetActive(false);
        }
    }
}
