using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAudioHelper : MonoBehaviour
{
    private AudioSource myAudio;

    private void Awake() {
        myAudio = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        myAudio.Play();
    }
}
