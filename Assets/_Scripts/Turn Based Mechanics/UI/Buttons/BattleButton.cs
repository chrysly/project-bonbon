using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleButton : MonoBehaviour
{
    public virtual void Activate(BattleUIStateMachine stateMachine) {}
    public void Scale(Vector3 scale, float duration) {
        transform.DOScale(scale, duration);
    }
}
