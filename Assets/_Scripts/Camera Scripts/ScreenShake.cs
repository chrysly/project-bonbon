using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenShake : MonoBehaviour {
    public IEnumerator shakeAction;
    public AnimationCurve curve;
    public float duration;
    [SerializeField] private Transform camera;

    public void Shake() {
        if (shakeAction == null) {
            shakeAction = ShakeAction();
            StartCoroutine(shakeAction);
        }
    }

    private IEnumerator ShakeAction() {
        Vector3 startPos = camera.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            camera.position = startPos + Random.insideUnitSphere;
            yield return null;
        }

        camera.position = startPos;
    }
}
