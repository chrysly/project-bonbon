using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterPanel : MonoBehaviour {
    
    private Vector3 resetPos;
    private Vector3 newPos;

    private void Start() {
        resetPos = transform.position;
        newPos = new Vector3(resetPos.x, resetPos.y - 300, resetPos.z);
        transform.DOMove(newPos, 0f);
        DialogueManager.OnDialogueStart += Toggle;
    }

    private void Toggle(bool active) {
        if (active) transform.DOMove(newPos, 0.5f);
        else transform.DOMove(resetPos, 0.5f);
    }
}
