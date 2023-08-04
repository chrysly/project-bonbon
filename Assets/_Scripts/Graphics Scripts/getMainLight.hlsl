#ifndef MAINLIGHT_INCLUDED
#define MAINLIGHT_INCLUDED

void GetMainLightData_half(float3 WorldPos, out half3 direction, out half3 color, out float DistanceAtten, out float ShadowAtten) {
    #ifdef SHADERGRAPH_PREVIEW
        direction = half3(-0.3, -0.8, 0.6);
        color = half3(1, 1, 1);
        DistanceAtten = 1;
        ShadowAtten = 1;
    #else
    #if SHADOWS_SCREEN
        float4 clipPos = TransformWorldToHClip(WorldPos);
        float4 shadowCoord = ComputeScreenPos(clipPos);
    #else
        float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    #endif
    #if defined(UNIVERSAL_LIGHTING_INCLUDED)
        Light mainLight = GetMainLight();
        direction = mainLight.direction;
        color = mainLight.color;
        DistanceAtten = mainLight.distanceAttenuation;
        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
        float shadowStrength = GetMainLightShadowStrength();
        ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false); 
        #endif
    #endif
}


#endif