using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinButton : MonoBehaviour
{
    public void TransitionScene() {
        GameManager.Instance.TransitionToNext();
    }
}
