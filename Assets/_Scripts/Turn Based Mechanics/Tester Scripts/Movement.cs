using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement {
    public void Bump(Transform transform, Transform target) {
        float move = 0.5f;
        Vector3 originalPos = transform.position;
        Vector3 targetPos = target.position;
        targetPos.y = originalPos.y;
        transform.DOLookAt(targetPos, 0.1f).OnComplete(() => {
            transform.DOMove(originalPos + transform.forward * move, 0.1f)
                .OnComplete(() => { transform.DOMove(originalPos, 0.1f); });
        });
        Bump(transform);
    }

    public void Bump(Transform transform) {
        float move = 0.5f;
        Vector3 originalPos = transform.position; 
        transform.DOMove(originalPos + transform.forward * move, 0.1f)
                .OnComplete(() => { transform.DOMove(originalPos, 0.1f); });
    }
}
