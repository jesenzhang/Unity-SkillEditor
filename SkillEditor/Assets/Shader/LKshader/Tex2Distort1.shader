// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LK/Transparent/Tex2Distort1"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_Intensity ("Intensity", float) = 1
		_MainTex ("Main Tex", 2D) = "white" {}
		_UVRotateSpeed ("UVRotateSpeed", Float) = 0
		_AddTex ("Add Tex", 2D) = "white" {}
		_HeatTime  ("Heat Time", range (-1,1)) = 0
		_ForceX ("Strength X", range (0,1)) = 0.1
		_ForceY ("Strength Y", range (0,1)) = 0.1

		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Op", float) = 0 //Add
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Blend Src Factor", float) = 5  //SrcAlpha
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Blend Dst Factor", float) = 10 //OneMinusSrcAlpha
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Cull Mode", float) = 2 //Back
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Z Test", float) = 4 //LessEqual
		[Enum(Off, 0, On, 1)] _ZWrite("Z Write", float) = 0 //Off
	}

	CGINCLUDE
	ENDCG

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		Pass
		{
			Tags { "LIGHTMODE"="Always" }
			Lighting Off
			Fog { Mode Off }
			BlendOp [_BlendOp]
			Blend [_BlendSrc] [_BlendDst]
			Cull [_CullMode]
			ZTest [_ZTest]
			ZWrite [_ZWrite]

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};
			
			float4 _Color;
			float _Intensity;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AddTex;
			float4 _AddTex_ST;
			
			float _UVRotateSpeed;
			float _USpeed;
			float _VSpeed;
			float _USpeed1;
			float _VSpeed1;

			half _ForceX;
			half _ForceY;
			float _HeatTime;
			
			inline float2 Calculate_UVAnim(float2 uv, float uSpeed, float vSpeed)
			{
				float time = _Time.z;
				float absUOffsetSpeed   = abs(uSpeed);
				float absVOffsetSpeed   = abs(vSpeed);

				if (absUOffsetSpeed > 0)
				{
					uv.x += frac(time * uSpeed);
				}

				if (absVOffsetSpeed > 0)
				{
					uv.y += frac(time * vSpeed);
				}

				return uv;
			}

			inline float2 Calculate_UVRotate(float2 uv, float uvRotateSpeed)
			{
				const half TWO_PI = 3.14159 * 2;
				const half2 VEC_CENTER = half2(0.5h, 0.5h);

				float time = _Time.z;
				float absUVRotSpeed = abs(uvRotateSpeed);
				half2 finaluv = uv;
				if (absUVRotSpeed > 0)
				{
					finaluv -= VEC_CENTER;
					half rotation = TWO_PI * frac(time * uvRotateSpeed);
					half sin_rot = sin(rotation);
					half cos_rot = cos(rotation);
					finaluv = half2(
						finaluv.x * cos_rot - finaluv.y * sin_rot,
						finaluv.x * sin_rot + finaluv.y * cos_rot);
					finaluv += VEC_CENTER;
				}
				uv = finaluv;
				return uv;
			}

			inline float2 Calculate_NoiseFromTex(float2 uv, sampler2D addTex)
			{
				float4 time = _Time;
				half offsetColor1 = tex2D(addTex, uv + frac(time.xz * _HeatTime));
				half offsetColor2 = tex2D(addTex, uv + frac(time.yx * _HeatTime));
				uv.x += (offsetColor1 - 0.5h) * _ForceX;
				uv.y += (offsetColor2 - 0.5h) * _ForceY;
				return uv;
			}

			v2f vert(appdata i)
			{
				v2f o;
				o.color = i.color * _Color;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				
				//main tex uv rotate
				o.uv = Calculate_UVRotate(o.uv, _UVRotateSpeed);

				//Add tex uv stay

				return o;
			}

			fixed4 frag(v2f i) : COLOR0
			{
				fixed4 color = i.color * _Intensity;

            	//noise effect
				i.uv = Calculate_NoiseFromTex(i.uv, _AddTex);

				fixed4 mainColor = tex2D(_MainTex, i.uv);
				color *= mainColor;
				return color;
			}
	
			ENDCG
		} 
	}
	//Fallback "VertexLit"
}