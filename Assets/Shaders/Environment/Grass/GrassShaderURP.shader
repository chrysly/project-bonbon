Shader "Unlit/GrassShaderURP"
{
    Properties
    {
		[Header(Shading)]
        _TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
    	
    	[Header(Scale and Rotation)]
    	_BladeWidth("Blade Width", Float) = 0.05
    	_BladeWidthRandom("Blade Width Random", Float) = 0.02
    	_BladeHeight("Blade Height", Float) = 0.5
    	_BladeHeightRandom("Blade Height Random", Float) = 0.3
    	_MinimumHeight("Blade Minimum Height Ratio", Range(0, 1)) = 0.1
    	_BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
    	_BladeForward("Blade Forward Amount", Float) = 0.38
		_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2
    	
    	[Header(Group Properties)]
    	_TessellationUniform("Density", Range(1, 64)) = 1
    	_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
    	_WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
    	_WindStrength("Wind Strength", Float) = 1
    	
    	_PlacementMap("Placement Map", 2D) = "white" {}
    }

	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
	
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

	#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
	#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
	#pragma multi_compile_fragment _ _SHADOWS_SOFT
	#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
	#pragma multi_compile _ SHADOWS_SHADOWMASK
	#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
	#pragma multi_compile_fog   
	#pragma multi_compile _ DIRLIGHTMAP_COMBINED
	#pragma multi_compile _ LIGHTMAP_ON

	

	#define BLADE_SEGMENTS 4
	#define UNITY_TWO_PI 6.28318530718f
	#define UNITY_PI 3.14159265359f
	
	float _BendRotationRandom;
	float _BladeHeight;
	float _BladeHeightRandom;
	float _MinimumHeight;
	float _BladeWidth;
	float _BladeWidthRandom;

	float _BladeForward;
	float _BladeCurve;

	sampler2D _WindDistortionMap;
	float4 _WindDistortionMap_ST;
	float2 _WindFrequency;
	float _WindStrength;

	sampler2D _PlacementMap;
	float4 _PlacementMap_ST;

	float3 minBounds;
	float3 maxBounds;

	//Reserved for vertex shader
	struct Attributes
	{
		float4 positionOS   : POSITION;
		float3 normal :NORMAL;
		float2 uv : TEXCOORD0;
		float4 color : COLOR;
		float4 tangent :TANGENT;
	};
	
	struct geometryOutput
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : NORMAL;
	};

	geometryOutput VertexOutput(float3 pos, float2 uv, float3 normal)
	{
		geometryOutput o;
		o.pos = TransformObjectToHClip(pos);
		o.uv = uv;
		o.normal = TransformObjectToWorldNormal(normal);
		return o;
	}

	float4 GetShadowPositionHClip(float3 input, float3 normal) {
		float3 positionWS = TransformObjectToWorld(input.xyz);
		float3 normalWS = TransformObjectToWorldNormal(normal);

		float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, 0));


	#if UNITY_REVERSED_Z
		positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
	#else
		positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
	#endif
		return positionCS;
	}

	// Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
	// Extended discussion on this function can be found at the following link:
	// https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
	// Returns a number in the 0...1 range.
	float rand(float3 co)
	{
		return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
	}

	// Construct a rotation matrix that rotates around the provided axis, sourced from:
	// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
	float3x3 AngleAxis3x3(float angle, float3 axis)
	{
		float c, s;
		sincos(angle, s, c);

		float t = 1 - c;
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;

		return float3x3(
			t * x * x + c, t * x * y - s * z, t * x * z + s * y,
			t * x * y + s * z, t * y * y + c, t * y * z - s * x,
			t * x * z - s * y, t * y * z + s * x, t * z * z + c
			);
	}

	geometryOutput GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformMatrix)
	{
		float3 tangentPoint = float3(width, forward, height);
		
		//lighting
		float3 tangentNormal = float3(0, -1, 0);
		float3 localNormal = mul(transformMatrix, tangentNormal);

		float3 localPosition = vertexPosition + mul(transformMatrix, tangentPoint);
		return VertexOutput(localPosition, uv, localNormal);
	}

	[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
	void geo(triangle vertexOutput IN[3], inout TriangleStream<geometryOutput> triStream)
	{
		float3 pos = IN[0].vertex;
		
		//float2 tiling = _PlacementMap_ST.xy;
		//float2 offset = _PlacementMap_ST.zw;
		float u = (pos.x - minBounds.x) / (maxBounds.x - minBounds.x);
		float v = (pos.z - minBounds.z) / (maxBounds.z - minBounds.z);
		
		float3 placementVal = tex2Dlod(_PlacementMap, float4(u, v, 0, 0));
		if (any(placementVal.rgb != float3(0, 0, 0)))
		{
			float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
			float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

			float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
			float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
			float3 wind = normalize(float3(windSample.x, windSample.y, 0));
			float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);
		
			float3 vNormal = IN[0].normal;
			float4 vTangent = IN[0].tangent;
			float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;

			float3x3 tangentToLocal = float3x3(
				vTangent.x, vBinormal.x, vNormal.x,
				vTangent.y, vBinormal.y, vNormal.y,
				vTangent.z, vBinormal.z, vNormal.z);

			float3x3 transformationMatrix = mul(mul(mul(tangentToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);
			float3x3 transformationMatrixFacing = mul(tangentToLocal, facingRotationMatrix);

			float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
			float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;

			float sizeMultiplier = placementVal.r < _MinimumHeight ? _MinimumHeight : placementVal.r;
			
			height *= sizeMultiplier;
			width *= sizeMultiplier;

			float forward = rand(pos.yyz) * _BladeForward;

			for (int i = 0; i < BLADE_SEGMENTS; i++)
			{
				float t = i / (float)BLADE_SEGMENTS;

				float segmentHeight = height * t;
				float segmentWidth = width * (1 - t);

				float segmentForward = pow(t, _BladeCurve) * forward;

				float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;
				triStream.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix));
				triStream.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix));
			}

			triStream.Append(GenerateGrassVertex(pos, 0, height, forward, float2(0.5, 1), transformationMatrix));
		}
	}
	ENDHLSL

    SubShader
    {
		Cull Off
		
		Tags
		{
			"RenderType" = "Opaque"
			"LightMode" = "UniversalForward"
		}

        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geo
			#pragma target 4.6
            #pragma multi_compile_fwdbase
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
            #pragma hull hull
            #pragma domain domain
            
			#include "Lighting.cginc"

			float4 _TopColor;
			float4 _BottomColor;
			float _TranslucentGain;

			float4 frag (geometryOutput i, fixed facing : VFACE) : SV_Target
            {
            	//#ifdef MAIN_LIGHT_SHADOWS
            		//VertexPositionInputs vertexInput = (VertexPositionInputs)0;
            		//vertexInput.positionWS;
            	
            	//#endif
            	
				return lerp(_BottomColor, _TopColor, i.uv.y);
            	
            }
            ENDHLSL
        }
        
        Pass
		{
			Tags
			{
				"LightMode" = "UniversalForward"
			}
			Blend OneMinusDstColor One
			ZWrite Off
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_fwdadd_fullshadows

			float4 frag(geometryOutput i) : SV_Target
			{
				//UNITY_LIGHT_ATTENUATION(atten, i, i.pos);
				//float3 pointlights = atten * unity_LightColor0.rgb;

				//return float4(atten, 0, 0, 1);
				//return float4(pointlights, 1);
			}

			ENDHLSL
		}
    }
}
