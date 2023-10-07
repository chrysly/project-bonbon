Shader "Unlit/Shader1"{
    Properties{ //input data
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)
        _ColorStart ("Color Start", Range(0, 1) ) = 0
        _ColorEnd ("Color End", Range(0, 1) ) = 1
    }
    SubShader{
        
        // subshader tags
        Tags { 
            "RenderType"="Transparent"  // tag to inform the render pipeline of what type this is
            "Queue" = "Transparent" // changes the render order
            }

        Pass{

            // pass tags
            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend One One // additive
            
            //Blend DstColor Zero // multiply
            
            //shader code
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;

            //automatically filled out by unity
            struct MeshData{ //per vertex mesh data
                float4 vertex : POSITION; // vertex position
                float3 normals : NORMAL; // normal direction
                //float4 color : COLOR; // vertex color
                //float4 tangent: TANGENT; // tangent direction(xyz) tangent sign (w)
                float4 uv0 : TEXCOORD0; // uv0 coordinates
            };

            // data passed from the vertex shader to the fragment shader
            // this will interpolate/blend across the triangle!!!
            struct Interpolators{
                float4 vertex : SV_POSITION; // clip space position
                float3 normal : TEXCOORD0;
                float2 uv: TEXCOORD1;
            };

            Interpolators vert (MeshData v){
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // converts local space to clip space
                o.normal = UnityObjectToWorldNormal(v.normals); // just pass through
                o.uv = v.uv0;
                return o;
            }
            
            // bool 0 1
            // float4 = Vector4 (32 bit float)
            // half (16 bit float)
            // fixed (12 bit float) -1 to 1
            // float4 -> half4 -> fixed4
            // float4x4 -> half4x4 (C#: Matrix4x4)

            float4 frag (Interpolators i) : SV_Target{

                // blend between two colors based on the x UV Coordinate
                //float t = saturate(InverseLerp(_ColorStart, _ColorEnd, i.uv.x));

                //return i.uv.y;
                
                float xOffset = cos(i.uv.x * TAU * 4) * 0.01;
                float t = cos((i.uv.y + xOffset - _Time.y * .5f)* TAU * (5 * _ColorStart)) * .2 + 0.5;
                t *= (1-i.uv.y);

                float topBottomRemover = (abs(i.normal.y) < .999);
                float waves = t * topBottomRemover;
                float4 gradient = lerp(_ColorA, _ColorB, i.uv.y);
                
                return gradient * waves;
            }

            float InverseLerp(float a, float b, float v){
                return (v-a) / (b-a);
            }
            ENDCG
        }
    }
}
