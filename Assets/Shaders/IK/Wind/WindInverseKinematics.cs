using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindInverseKinematics : MonoBehaviour {

    private Animator animator;
    [SerializeField] private WindIKManager ikManager;
    
    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
