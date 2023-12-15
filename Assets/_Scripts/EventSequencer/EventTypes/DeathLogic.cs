using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathLogic : MonoBehaviour
{
    [SerializeField] private Image background;

    public void BackgroundFlash() {
        background.enabled = true;
    }
}
