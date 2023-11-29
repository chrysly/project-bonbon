using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UILogicUtils {
    
    public static void SetupButton(GameObject obj, bool toggle, float grayoutAlpha) {
        var images = obj.GetComponentsInChildren<Image>();
        var rawImages = obj.GetComponentsInChildren<RawImage>();
        var texts = obj.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        var targetAlpha = toggle ? grayoutAlpha : 1;
        foreach (Image image in images) {
            Color color = image.color;
            color.a = targetAlpha;
            image.color = color;
        } foreach (RawImage image in rawImages) {
            Color color = image.color;
            color.a = targetAlpha;
            image.color = color;
        } foreach (TMPro.TextMeshProUGUI text in texts) text.alpha = targetAlpha;
    }
}
