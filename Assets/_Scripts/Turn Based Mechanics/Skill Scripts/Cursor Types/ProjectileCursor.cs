using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCursor : CursorType
{
    public void Draw(Transform cursor, Transform actor, LineController lineController) {
        if (isActive) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
                location = hit.point;
                LevelYPosition(actor);
                DrawCursor(cursor);
                DrawLine(lineController, cursor, actor);
            }
        }
    }

    private void LevelYPosition(Transform actor) {
        location.y = actor.position.y;
    }

    private void DrawCursor(Transform cursor) {
        cursor.position = location;
    }

    private void DrawLine(LineController line, Transform source, Transform destination) {
        Transform[] points = { source, destination };
        line.DrawLine(points);
    }
}
