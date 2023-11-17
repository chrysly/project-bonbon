using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetBetweenVector : MonoBehaviour {
    [SerializeField] private Transform lookTarget;

    [SerializeField] private Transform followTarget;

    public Vector3 vector;

    private void CalculateVector() {
        if (lookTarget != null && followTarget != null) {
            vector = lookTarget.position - followTarget.position;
        }
    }
}
