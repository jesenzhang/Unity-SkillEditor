// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ColorDiffuse" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("EffectColor", Color) = (1,1,1,1)
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 150
				
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform fixed4 _Color;

			struct v2f
			{
				float4 pos	: SV_POSITION;
				float2 uv_MainTex 	: TEXCOORD0;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
												
				return o;
			}
							
			float4 frag (v2f i) : COLOR
			{
				float4 mainColor = tex2D(_MainTex, i.uv_MainTex);
				
				fixed4 finalColor;
				finalColor.rgb = mainColor.rgb * _Color.rgb;
				finalColor.a = mainColor.a * _Color.a;
									
				return finalColor;
			}
			ENDCG
		}
	} 
	
	FallBack "Diffuse"
}
