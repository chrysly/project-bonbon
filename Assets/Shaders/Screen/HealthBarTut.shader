Shader "Unlit/HealthBarTut"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _Health("Health", range(0, 1)) = 1
        _uvOffset("uvOffset", range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

        Pass
        {
            ZWrite Off
            // src * X + Dst * Y
            Blend SrcAlpha OneMinusSrcAlpha // Alpha Blending
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolator
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Health;
            float _uvOffset;
            

            Interpolator vert (MeshData v)
            {
                Interpolator o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float InverseLerp (float a, float b, float v){
                return (v-a)/(b-a);
            }

            float4 frag (Interpolator i) : SV_Target
            {
                float healthbarMask = _Health > (i.uv.x);
                float3 col = tex2D(_MainTex, float2(_Health, i.uv.y));
                
                
                return float4(col.rgb * healthbarMask, healthbarMask);
            }
            ENDCG
        }
    }
}
