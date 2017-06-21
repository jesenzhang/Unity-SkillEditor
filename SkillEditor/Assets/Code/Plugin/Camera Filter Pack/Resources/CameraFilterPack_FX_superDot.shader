// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_superDot" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
			_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
			_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
	}
	SubShader 
	{
		Pass
		{
			ZTest Always
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#include "UnityCG.cginc"


				uniform sampler2D _MainTex;
			uniform float _TimeX;
			uniform float _Distortion;
			uniform float4 _ScreenResolution;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				half2 texcoord  : TEXCOORD0;
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
			};   

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;

				return OUT;
			}


			float _smooth(float f) {
				return 32.0*(0.25*f*f*f*f-0.5*f*f*f+0.25*f*f)+0.5;
			}



			float4 frag (v2f i) : COLOR
			{
				float PIXEL_FACTOR 	= 8.0;
				float2 chunkCoord 	= floor(i.texcoord.xy * _ScreenResolution.xy / PIXEL_FACTOR) * PIXEL_FACTOR;
				float2 locCoord 	= (i.texcoord.xy*_ScreenResolution.xy - chunkCoord) / PIXEL_FACTOR;
				float4 color 		= tex2D(_MainTex, chunkCoord / _ScreenResolution.xy);
				float grey 			= (color.x + color.y + color.z) / 3.0;

				return color * _smooth(locCoord.x) * _smooth(locCoord.y);	
			}

			ENDCG
		}

	}
}
