Shader "Unlit/EmissionBasic" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        [HDR] _EmissionColor("Emission Color", Color) = (0, 0, 0)
        _EmissionMap("Emission", 2D) = "black" {}
        _MainTex ("Texture", 2D) = "white" {}
        _FlickerRate("Flicker Rate", Range(0, 4)) = 0
        _FlickerIntensity("Flicker Intensity", Range(0, 4)) = 0
        _FlickerOffset("Flicker Offset", Range(0, 4)) = 0

    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            float4 _EmissionColor;
            float _Emission;
            sampler2D _EmissionMap;

            float _FlickerRate;
            float _FlickerIntensity;
            float _FlickerOffset;

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float IntensityLevel() {
                return cos(_Time.y * _FlickerRate) * _FlickerIntensity + _FlickerOffset;
            }

            fixed4 frag (Interpolators i) : SV_Target {
                // sample the texture
                float4 bodyCol = float4(_Color.rgb, _Color.a);
                float4 emission = tex2Dlod(_EmissionMap, float4(i.uv, 0, 0)) * pow(_EmissionColor, IntensityLevel());
                bodyCol.rgb += emission.rgb;
                return bodyCol + _Color;
            }
            ENDCG
        }
    }
}
