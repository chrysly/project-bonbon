using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class DamageIndicatorManager : MonoBehaviour {

    [SerializeField] private GameObject textPrefab;
    private BattleStateMachine stateMachine => BattleStateMachine.Instance;
    [SerializeField] private float textDelay = 0f;
    [SerializeField] private float textDuration = 1f;

    public void Start() {
        StartCoroutine(DelaySubscription());
    }

    private IEnumerator DelaySubscription() {
        yield return new WaitForSeconds(0.5f);
        stateMachine.CurrInput.AnimationHandler.DamageEvent += SpawnDamageText;
        stateMachine.CurrInput.AnimationHandler.HealEvent += SpawnHealText;
        stateMachine.CurrInput.AnimationHandler.EffectEvent += SpawnEffectText;
    }

    private void SpawnDamageText(float damage, Actor target, bool augmented) {
        if (damage > 0) {
            GameObject text = Instantiate(textPrefab, GenerateOffset(target.transform.position), target.transform.rotation);
            text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + (int) damage;
            RunTextAnimation(text.transform, augmented ? new Color32(240, 108, 163, 255) : Color.white, augmented ? 1.5f : 1f);
        }
    }

    private void SpawnHealText(float heal, Actor target) {
        GameObject text = Instantiate(textPrefab, GenerateOffset(target.transform.position), target.transform.rotation);
        text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + (int) heal;
        RunTextAnimation(text.transform, new Color(0.11f, 0.80f, 0.60f, 1f));
    }

    private void SpawnEffectText(List<EffectBlueprint> effects, Actor target) {
        if (effects == null || effects.Count == 0) return;
        foreach (EffectBlueprint effect in effects) {
            Debug.LogError("Fuck");
            if (effect.modifiers.flatAttack > 0 || effect.modifiers.percentAttack > 0) {
                GenerateEffectText("<color=#FF7F7F>ATK</color><color=yellow>↑</color>", target);
            } if (effect.modifiers.flatAttack < 0 || effect.modifiers.percentAttack < 0) {
                GenerateEffectText("<color=#FF7F7F>ATK</color><color=blue>↓</color>", target);
            } if (effect.modifiers.flatDefense > 0 || effect.modifiers.percentDefense > 0) {
                GenerateEffectText("<color=#9BEDFF>DEF</color><color=yellow>↑</color>", target);
            } if (effect.modifiers.flatDefense < 0 || effect.modifiers.percentDefense < 0) {
                GenerateEffectText("<color=#9BEDFF>DEF</color><color=blue>↓</color>", target);
            }
        }
    }

    private void RunTextAnimation(Transform text, Color32 color, float targetScale = 1) {
        text.GetComponentInChildren<TextMeshProUGUI>(true).color = color;
        StartCoroutine(TextAnimationAction(text, targetScale));
    }

    private IEnumerator TextAnimationAction(Transform text, float targetScale) {
        yield return new WaitForSeconds(textDelay);
        text.transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBounce);

        text.transform.DOMoveY(text.transform.position.y + 2f, 1f);
        yield return new WaitForSeconds(textDuration);
        text.transform.DOScale(0f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        Destroy(text.gameObject);
        yield return null;
    }

    private void GenerateEffectText(string effectText, Actor target) {
        GameObject text = Instantiate(textPrefab, GenerateOffset(target.transform.position), target.transform.rotation);
        TextMeshProUGUI textComp = text.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textComp.richText = true;
        textComp.text = effectText;
        RunTextAnimation(text.transform, Color.white, 0.5f);
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
