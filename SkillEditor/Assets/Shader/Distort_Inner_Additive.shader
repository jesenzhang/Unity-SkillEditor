// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Distort_Inner_Additive" 
{
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}
		_ColorScale("color scale", Range(0, 2)) = 1
		//_TintColor("Tint Color", Color) = (1, 1, 1, 1)
		_NoiseTex ("Noise Tex", 2D) = "white" {}
		_MoveSpeed  ("Move Speed", range (0,1.5)) = 1
			_MoveForce  ("Move Force", range (0,0.1)) = 0.1
	}

	Category 
	{
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend SrcAlpha One 
			//AlphaTest Greater .01
			Cull Back 
			Lighting Off
			ZWrite Off

			SubShader
			{
				Pass {
					Name "BASE"
						Tags { "LightMode" = "Always" }

					CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

						struct appdata_t {
							float4 vertex : POSITION;
							half2 texcoord: TEXCOORD0;
							fixed4 color : COLOR;
						};

					struct v2f {
						float4 vertex : POSITION;
						half2 uvmain : TEXCOORD0;
						fixed4 color : COLOR;
					};

					fixed _MoveSpeed;
					fixed _MoveForce;
					float4 _NoiseTex_ST;
					float4 _MainTex_ST;

					sampler2D _MainTex;
					sampler2D _NoiseTex;

					//float4 _TintColor;
					fixed _ColorScale;

					v2f vert (appdata_t v)
					{
						v2f o;
						o.color = v.color;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
						return o;
					}

					half4 frag( v2f i ) : COLOR
					{
						half4 offsetColor1 = tex2D(_NoiseTex, i.uvmain + _Time.xz * _MoveSpeed);
						half4 offsetColor2 = tex2D(_NoiseTex, i.uvmain - _Time.yx * _MoveSpeed);

						i.uvmain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _MoveForce;
						i.uvmain.y += ((offsetColor1.g + offsetColor2.g) - 1) * _MoveForce;

						half4 col = tex2D(_MainTex, i.uvmain);
						col *= i.color * _ColorScale;
						//col *= _TintColor;
						return  col;
					}
					ENDCG
				}
			}

		SubShader 
		{
			Blend DstColor Zero
				Pass 
				{
					Name "BASE"
						SetTexture [_MainTex] { combine texture }
				}
		}
	}
}
