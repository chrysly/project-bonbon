using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTest : MonoBehaviour
{
    [SerializeField] private LineController lineController;
    [SerializeField] private Transform actor;
    [SerializeField] private Transform cursor;

    private ProjectileCursor pCursor = new();

    private void Update() {
        pCursor.Draw(cursor, actor, lineController);
    }
}
