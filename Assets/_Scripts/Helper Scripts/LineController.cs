using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform[] points;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawLine(Transform[] points) {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    private void Update() {
        for (int i = 0; i < points.Length; i++) {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }

    public void DestroyLine() {
        Destroy(lineRenderer);
    }
}
