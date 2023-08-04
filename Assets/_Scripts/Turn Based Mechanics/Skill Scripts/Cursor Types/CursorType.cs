using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorType
{
    protected Vector3 location;
    protected bool isActive = true;

    public virtual void Draw(Transform cursor) {
        if (isActive) {
            DrawCursor(cursor);
        }
    }

    private void DrawCursor(Transform cursor) {
        cursor.position = location;
    }

    public void Enable() {
        isActive = true;
    }

    public void Disable() {
        isActive = false;
    }

    public Vector3 getLocation() {
        return location;
    }

}
