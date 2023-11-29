using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasShaderChannelSetup : MonoBehaviour
{

    private void Update() {
        GetComponent<Canvas>().additionalShaderChannels = AdditionalCanvasShaderChannels.None;
    }
}
