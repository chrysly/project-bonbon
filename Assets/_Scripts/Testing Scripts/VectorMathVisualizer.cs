using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class VectorMathVisualizer : MonoBehaviour {
    [SerializeField] private Transform source;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 angle;
    [SerializeField] private float numericAngle;

    public Vector3 forward;

    public void ReturnForward() {
        forward = source.forward;
        numericAngle = FullRangeYVectorAngle(transform.forward, Quaternion.Euler(angle.x, angle.y, angle.z) * offset);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(angle.x, angle.y, angle.z) * offset);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }

    private float FullRangeYVectorAngle(Vector3 a, Vector3 b) {
        a.y = 0;
        b.y = 0;
        float dotProduct = Vector3.Dot(a, b);
        float angle = Mathf.Acos(dotProduct / (a.magnitude * b.magnitude));

        return angle * Mathf.Rad2Deg;
    }
}
