using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPreview : MonoBehaviour
{
    #region Booleans
    private bool isMove = false;
    private bool isActive = false;  //Path is currently being selected
    private bool showLine = false;
    #endregion

    private GameObject activeCursor;
    private GameObject previewCursor;
    private LineController line;

    public void CreateDisplay(GameObject cursorPrefab) {
        isActive = true;
        activeCursor = Instantiate(cursorPrefab);
    }

    public void CreateMoveDisplay(GameObject cursorPrefab) {
        isActive = true;
        activeCursor = Instantiate(cursorPrefab);
        isMove = true;
    }

    public void UpdateDisplay(Transform actor) {
        if (isActive) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
                UpdateCursor(hit.point, actor);

                //BOOLEAN METHODS
                if (showLine) {
                    DrawLine(actor, activeCursor.transform);
                }
            }
        }
    }

    private void UpdateCursor(Vector3 position, Transform actor) {
        position.y = actor.position.y;
        activeCursor.transform.position = position;
    }

    private void DrawLine(Transform source, Transform destination) {
        Transform[] points = { source, destination };
        line.DrawLine(points);
    }

    public void LockDisplay() {
        isActive = false;
    }

    public void WipeDisplay() {
        isActive = false;
        activeCursor = null;
        ClearAll();
        DestroyAll();
    }

    #region Boolean Logic
    public void EnableLine(GameObject lineRenderer) {
        line = Instantiate(lineRenderer).GetComponent<LineController>();
        line.gameObject.SetActive(true);
        showLine = true;
    }

    private void DestroyAll() {
        Destroy(activeCursor);
        Destroy(line.gameObject);
    }

    private void ClearAll() {  //MAKE SURE TO UPDATE WHEN ADDING NEW BOOLEANS
        line = null;
        isMove = false;
        isActive = false;
        showLine = false;
    }
    #endregion
}
