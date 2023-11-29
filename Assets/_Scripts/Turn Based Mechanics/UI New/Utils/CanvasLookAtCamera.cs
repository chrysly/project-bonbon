using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour {

    private Canvas canvas;

    void Awake() {
        canvas = GetComponent<Canvas>();
    }

    private void LateUpdate() {
        canvas.transform.LookAt(Camera.main.transform);
    }
}
