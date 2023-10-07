Shader "Unlit/Texture" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Tile ("Tile", 2D) = "white" {}
        _Pattern ("Pattern", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            struct MeshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            float GetWave(float2 coord) {
                float t = cos((coord - _Time.y * 0.1) * TAU * 5);
                t *= 1 - coord;
                return t;
            }

            sampler2D _MainTex;     //required to sample from texture
            float4 _MainTex_ST;     //optional, sets tiling and offset

            sampler2D _Pattern;
            sampler2D _Tile;
            //usage: [sampler2DName]_ST, will auto-link

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.worldPos = mul( UNITY_MATRIX_M, float4( v.vertex.xyz, 1 ) );
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target {
                // sample the texture

                float2 topDownProjection = i.worldPos.xz;
                //float4 tile = tex2D(_MainTex, topDownProjection);
                //return float4(topDownProjection, 0, 1);
                float4 moss = tex2D(_MainTex, topDownProjection);
                float4 tile = tex2D(_Tile, topDownProjection);
                float pattern = tex2D( _Pattern, i.uv).x;

                float4 finalColor = lerp( tile, moss, pattern);
                return finalColor;
            }
            ENDCG
        }
    }
}
