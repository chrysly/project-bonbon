using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActionDisplay : MonoBehaviour
{
    #region Booleans
    private bool isActive = false;  //Path is currently being selected
    private bool showLine = false;
    #endregion

    private GameObject activeCursor;
    private LineController line;

    public void CreateDisplay(GameObject cursorPrefab) {
        isActive = true;
        activeCursor = Instantiate(cursorPrefab);
    }

    public void CreateMoveDisplay(GameObject cursorPrefab, Vector3 location) {
        location.y++;
        activeCursor = Instantiate(cursorPrefab, location, Quaternion.identity);
        activeCursor.transform.DOScaleY(0, 0);
        activeCursor.transform.DOScaleY(1, 0.5f);
    }

    public void RunDisplayPlacement(Transform actor) {
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
        DestroyAll();
        ClearAll();
    }

    #region Boolean Logic
    public void EnableLine(GameObject lineRenderer) {
        line = Instantiate(lineRenderer).GetComponent<LineController>();
        line.gameObject.SetActive(true);
        showLine = true;
    }

    private void DestroyAll() {
        Destroy(activeCursor);
        if (showLine) Destroy(line.gameObject);
    }

    private void ClearAll() {  //MAKE SURE TO UPDATE WHEN ADDING NEW BOOLEANS
        line = null;
        isActive = false;
        showLine = false;
        activeCursor = null;
    }
    #endregion

    public Transform GetCursor() {
        return activeCursor.transform;
    }
}
