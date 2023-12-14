using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorIdentifier : MonoBehaviour
{
    private void Awake() {
        gameObject.layer = LayerMask.NameToLayer("TopLevelUI");
    }
}
