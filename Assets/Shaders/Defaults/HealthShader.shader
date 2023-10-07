Shader "Unlit/HealthShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA("Color A", Color) = (1, 1, 1, 1)
        _ColorB("Color B", Color) = (1, 1, 1, 1)
        _BorderSize ("Border Size", Range(0, 0.5)) = 0.1
        _Health("Health", Range(0, 1)) = 1
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

            float4 _ColorA;
            float4 _ColorB;
            float _Health; 
            float _BorderSize;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float InverseLerp(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target {
                // sample the texture
                float2 coords = i.uv;
                coords.x *= 8;
                float2 pointOnLineSeg = float2(clamp( coords.x, 0.5, 7.5), 0.5);
                float sdf = distance(coords, pointOnLineSeg) * 2 - 1;
                
                float borderSdf = sdf + _BorderSize;
                float pd = fwidth(borderSdf);           //screen space partial derivative
                float borderMask = 1 - saturate(borderSdf / pd);
                clip(-sdf);
                float healthBarMax = _Health < i.uv.x;
                float3 healthbarColor = tex2D(_MainTex, float2(_Health, i.uv.y));
                healthbarColor *= ((1 - healthBarMax));

                if (_Health < 0.2) {
                    float flash = cos( _Time.y * 4 ) * 0.2 + 1;
                    healthbarColor *= flash;
                }
                //float colorClamp = saturate(InverseLerp(0.2, 0.8, _Health));
                //float4 barColor = lerp(_ColorA, _ColorB, colorClamp);
                //float4 healthColor = lerp(barColor, float4(0, 0, 0, 0), healthBarMax);
                //clip(_Health - i.uv.x);
                return float4(healthbarColor.rgb * borderMask, 1);
            }
            ENDCG
        }
    }
}