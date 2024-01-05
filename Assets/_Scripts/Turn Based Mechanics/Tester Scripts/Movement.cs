using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement {
    public void Bump(Transform transform, Transform target, float duration) {
        float move = 0.5f;
        Vector3 originalPos = transform.position;
        Vector3 originalRotation = transform.rotation.eulerAngles;
        Vector3 targetPos = target.position;
        targetPos.y = originalPos.y;
        transform.DOLookAt(targetPos, 0.1f);
    }

    public IEnumerator ResetOrientation(Transform transform, Vector3 originalRotation, float delay) {
        yield return new WaitForSeconds(delay);
        transform.DORotate(originalRotation, 0.1f);
    }
}
