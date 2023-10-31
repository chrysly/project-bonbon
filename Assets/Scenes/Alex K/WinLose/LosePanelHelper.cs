using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosePanelHelper : MonoBehaviour
{
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private float fadeDelay;
    [SerializeField] private float fadeDuration = 1;
    private float activeTime;
    private Image panelImage;

    private void Awake() {
        panelImage = GetComponent<Image>();
    }

    private void OnEnable() {
        activeTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        activeTime += Time.deltaTime;
        panelImage.color = colorGradient.Evaluate(Mathf.Clamp((activeTime - fadeDelay) / fadeDuration, 0, 1));
    }
}
