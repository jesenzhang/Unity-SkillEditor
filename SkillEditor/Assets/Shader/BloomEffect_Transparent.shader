// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/BloomEffect_Transparent" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Amount ("_Amount", Range(0.0, 20.0)) = 5.0
			_Glow ("_Glow", Range(0.0, 20.0)) = 5.0
	}

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha 

			Pass 
			{  
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog

#include "UnityCG.cginc"

					struct appdata_t 
					{
						float4 vertex : POSITION;
						float2 texcoord : TEXCOORD0;
					};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

			uniform float _Amount;
			uniform float _Glow;

			float nrand(float2 n) 
			{
				return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
			}

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
				{
					float stepU = _Amount;
					float stepV = _Amount;
					fixed4 color = tex2D(_MainTex,i.texcoord.xy );
					fixed3x3 gaussian = fixed3x3(
							1.0,	2.0,	1.0,
							2.0,	4.0,	2.0,
							1.0,	2.0,	1.0);

					float4 result = 0;

					for(int u=0;u<3;u++) 
					{
						for(int j=0;j<3;j++) 
						{
							float2 texCoord = i.texcoord.xy + float2( float(u-1)*stepU, float(j-1)*stepV );
							result += gaussian[u][j] * tex2D(_MainTex,texCoord);
						}

					}

					result /=8;

					result.rgb = lerp (color.rgb,result.rgb, _Glow);

					result.a = color.a;

					return result;
					/*
					   fixed4 col = tex2D(_MainTex, i.texcoord);
					   UNITY_APPLY_FOG(i.fogCoord, col);
					   return col;
					 */
				}
			ENDCG
			}
	}
}

