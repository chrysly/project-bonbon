float LightingSpecular(float3 L, float3 N, float3 V, float smoothness)
{
    float3 H = SafeNormalize(float3(L) + float3(V));
    float NdotH = saturate(dot(N, H));
    return pow(NdotH, smoothness);
}

void MainLighting_float(float3 Normal, float3 Position, float3 View, float Smoothness, out float Specular)
{
    Specular = 0.0;

    #ifndef SHADERGRAPH_PREVIEW
    Smoothness = exp2(10 * Smoothness + 1);

    Normal = normalize(Normal);
    View = SafeNormalize(View);
    
    Light mainLight = GetMainLight(TransformWorldToShadowCoord(Position));
    Specular = LightingSpecular(mainLight.direction, Normal, View, Smoothness);
    
    #endif
}

void AdditionalLighting_float(float3 Normal, float3 Position, float3 View, float Smoothness, float Hardness, out float3 Specular)
{
    Specular = 0.0;

    #ifndef SHADERGRAPH_PREVIEW
    Smoothness = exp2(10 * Smoothness + 1);

    Normal = normalize(Normal);
    View = SafeNormalize(View);

    // additional lights
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, Position);
        float3 attenuatedLight = light.color * light.distanceAttenuation * light.shadowAttenuation;

        float specular_soft = LightingSpecular(light.direction, Normal, View, Smoothness);
        float specular_hard = smoothstep(0.005,0.01,specular_soft);
        float specular_term = lerp(specular_soft, specular_hard, Hardness);

        Specular += specular_term * attenuatedLight;
    }
    #endif
}
