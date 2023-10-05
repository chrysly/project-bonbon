Shader "Unlit/VertexOffset" {
    Properties{    //input data
        //_MainTex ("Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1, 1, 1, 1)
        _ColorB ("Color B", Color) = (1, 1, 1, 1)
        _ColorStart ("Color Start", Range(0,1) ) = 0
        _ColorEnd ("Color End", Range(0,1) ) = 1
        _WaveAmp ("Wave Amplitude", Range(0, 0.2)) = 0.1
    }
    SubShader {
        Tags { 
            "RenderType"="Opaque"  //tag to inform the render pipeline of what type this is
        }  //how stuff renders in the pipeline
        //LOD 100                       //picks different subshaders based on LOD, rarely used?

        Pass {                          //explicit rendering for this pass itself, graphics
          //additive
            //Blend DstColor Zero

            CGPROGRAM
            #pragma vertex vert         //tells compiler what function is vertex
            #pragma fragment frag       //tells compiler what function is fragment shader

            
            // make fog work - IGNORE FOR NOW
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"    //file containing unity specific functions for efficiency

            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;

            float _ColorStart;
            float _ColorEnd;
            float _WaveAmp;
            //float _Scale;
            //float _Offset

            //sampler2D _MainTex;
            //float4 _MainTex_ST;

            struct MeshData {               //per-vertex mesh data
                float4 vertex : POSITION;   // vertex position
                float3 normals: NORMAL;
                //float4 tangent :TANGENT;
                //float4 color : COLOR;
                float4 uv0 : TEXCOORD0;      // uv coordinates
                //float2 uv1 : TEXCORRD1;
            };

            struct Interpolator {                    //default name for data passed from vertex shader to fragment shader
                //float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;         //clip space position of vertex
                float3 normal : TEXCOORD0;           //corresponds to a data stream
                float2 uv : TEXCOORD1;
                //float2 tangent : TEXCOORD1;
                //float2 justSomeValues : TEXCOORD2; 
            };

            float GetWave(float2 uv) {
                float2 uvsCentered = uv * 2 - 1;
                float radialDistance = length(uvsCentered);
                float t = cos((radialDistance - _Time.y * 0.1) * TAU * 5);
                t *= 1 - radialDistance;
                return t;
            }

            Interpolator vert (MeshData v) {
                Interpolator o;

                //float wave1 = cos((v.uv0.y - _Time.y * 0.1) * TAU * 5);
                //float wave2 = cos((v.uv0.x - _Time.y * 0.1) * TAU * 5);
                //v.vertex.y = wave1 * wave2 * _WaveAmp;
                v.vertex.y = GetWave( v.uv0 ) * _WaveAmp;

                o.vertex = UnityObjectToClipPos(v.vertex);  //multiply by model view project matrix, converts local space to clip space
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                o.normal = UnityObjectToWorldNormal( v.normals );   //just pass data through immediately through fragment shader
                //manual: o.normal = mul( (float3x3)unity_ObjectToWorld, v.normals );
                o.uv = v.uv0; //(v.uv0 + _Offset) * _Scale;   //passthrough
                return o;
            }

            float InverseLerp(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            //float4 = Vector4 (R, G, B, ALPHA?)
            //float (32 bit float), works for anything in world space
            //half (16 bit float), useful for most <<
            //fixed (lower precision), only useful for -1 to 1
            //matrix = float4x4 -> half4x4 (C#: Matrix 4x4)

            float4 frag (Interpolator i) : SV_Target {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                //float t = saturate(InverseLerp( _ColorStart, _ColorEnd, i.uv.x));
                
                //t = frac(t);
                //float4 outColor = lerp( _ColorA, _ColorB, t);  //blend between two colors
                float2 uvsCentered = i.uv * 2 - 1;
                float radialDistance = length(uvsCentered);
                float t = cos((radialDistance - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5;
                t *= 1 - radialDistance;
                return t;
                //return outColor;
            }
            ENDCG
        }
    }
}
