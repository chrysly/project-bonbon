using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PanGoBurr : MonoBehaviour
{
    public float timeUntilLoad = 0f;
    private float timer = 0f;
    private bool keepGoing = true;

    private void Update() {
        timer += Time.deltaTime;

        if (timer > timeUntilLoad && keepGoing) {
            GameManager.Instance.TransitionToNext();
            keepGoing = false;
        }
    }
}
