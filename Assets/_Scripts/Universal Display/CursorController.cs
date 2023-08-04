using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

    public Texture2D cursor;
    public Texture2D cursorClicked;


    private void Awake() {
        SwitchCursor(cursor);
        //Cursor.lockState = CursorLockMode.Confined;
    }

    private void SwitchCursor(Texture2D cursorType) {
        Vector2 hotspot = new(0, cursorType.height);
        Cursor.SetCursor(cursorType, hotspot, CursorMode.Auto);
    }
}
