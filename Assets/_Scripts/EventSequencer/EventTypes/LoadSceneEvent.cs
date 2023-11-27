using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Right now it just loads the next scene in the build, but i can make it take in a parameter if need be
/// </summary>
[CreateAssetMenu(fileName = "New EventObject", menuName ="Event System/LoadSceneEvent" )]
public class LoadSceneEvent : EventObject {
    [SerializeField] private bool includeLoadingBar;

    public override void OnEventEnd() {
        GameManager.Instance.TransitionToNext(includeLoadingBar);
    }
}
