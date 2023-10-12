using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class BattleButton : MonoBehaviour {
    public Transform targetPoint;
    public virtual void Activate(BattleUIStateMachine stateMachine, float delay) {}
    public void Scale(Vector3 scale, float duration) {
        transform.DOScale(scale, duration);
    }
}
