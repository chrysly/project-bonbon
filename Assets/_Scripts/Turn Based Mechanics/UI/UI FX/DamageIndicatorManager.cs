using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class DamageIndicatorManager : MonoBehaviour {
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private BattleStateMachine stateMachine;
    [SerializeField] private float textDelay = 1f;
    [SerializeField] private float textDuration = 1f;
    private IEnumerator _textQueue;

    public void Start() {
        stateMachine.OnStateTransition += SpawnDamageText;
    }

    private void SpawnDamageText(BattleStateMachine.BattleState battleState, BattleStateInput input) {
        if (battleState is BattleStateMachine.AnimateState) {
            Actor target = input.SkillPrep.targets[0];

            bool hasBonbon = input.SkillPrep.bonbon != null;
            
            int damage = input.SkillPrep.skill.ComputeSkillActionValues(target, input.CurrTurn(), input.SkillPrep.bonbon).immediateDamage;
            if (damage > 0) {
                GameObject text = Instantiate(textPrefab, GenerateOffset(target.transform.position), target.transform.rotation);
                text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + damage;
                text.transform.DOScale(0f, 0f);
                RunTextAnimation(text.transform, hasBonbon);
            }
        }
    }

    private void RunTextAnimation(Transform text, bool augmented) {
        if (_textQueue == null) {
            if (augmented) {
                Debug.Log("Changed color");
                text.GetComponentInChildren<TextMeshProUGUI>(true).color = new Color32(240, 108, 163, 255);
            }
            _textQueue = TextAnimationAction(text, augmented);
            StartCoroutine(_textQueue);
        }
    }

    private IEnumerator TextAnimationAction(Transform text, bool augmented) {
        yield return new WaitForSeconds(textDelay);
        if (!augmented) {
            text.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        }
        else {
            text.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce);
        }

        text.transform.DOMoveY(text.transform.position.y + 2f, 1f);
        yield return new WaitForSeconds(textDuration);
        text.transform.DOScale(0f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        Destroy(text.gameObject);
        _textQueue = null;
        yield return null;
    }

    private Vector3 GenerateOffset(Vector3 position) {
        Random random = new Random();
        float randX = (float) (random.NextDouble() * (1.5) - 0.5);
        float randY = (float) (random.NextDouble() * (1.5));
        float randZ = (float) (random.NextDouble() * (1.5) - 0.5);
        Vector3 offset = new Vector3(randX, randY + 1, randZ);
        return position + offset;
    }
}
