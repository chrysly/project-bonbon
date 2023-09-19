using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashTest : MonoBehaviour
{
    public Animator anim;
    public List<Slash> slashes;

    private bool attacking;

    private void Start() {
        DisableSlashes();
        attacking = true;
        anim.SetTrigger("OnStart");
        Debug.Log("oof");
        StartCoroutine(SlashAttack());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !attacking) {
            attacking = true;
            anim.SetTrigger("OnStart");
            Debug.Log("oof");
            StartCoroutine(SlashAttack());
        }
    }

    IEnumerator SlashAttack() {
        for (int i = 0; i < slashes.Count; i++) {
            yield return new WaitForSeconds(slashes[i].delay);
            slashes[i].slashObj.SetActive(true);
        }

        yield return new WaitForSeconds(2f);
        DisableSlashes();
        attacking = false;
    }

    void DisableSlashes() {
        for (int i = 0; i < slashes.Count; i++) {
            slashes[i].slashObj.SetActive(false);
        }
    }
}

[System.Serializable]
public class Slash {
    public GameObject slashObj;
    public float delay;
}
