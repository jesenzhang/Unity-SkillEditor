// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_Half_Sphere" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
		_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
		_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
		_SphereSize ("_SphereSize", Range(1.0, 10.0)) = 1
		_SpherePositionX ("_SpherePositionX", Range(-1.0, 1.0)) = 0
		_SpherePositionY ("_SpherePositionY", Range(-1.0, 1.0)) = 0
		_Strength ("_Strength", Range(1, 10)) = 5
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
			uniform float _SphereSize;
			uniform float _SpherePositionX;
			uniform float _SpherePositionY;
			uniform float _Strength;

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

			inline float2 mix(float2 a, float2 b, float t)
			{
				return a * (1.0 - t) + b * t;
			}

			float atan2(float y, float x){
				if(x>0.) return atan(y/x);
				if(y>=0. && x<0.) return atan(y/x) + 3.14; 
				if(y<0. && x<0.) return atan(y/x) - 3.14; 
				if(y>0. && x==0.) return 1.57;
				if(y<0. && x==0.) return -1.57;
				if(y==0. && x==0.) return 1.57;
				return 1.57;
			}

			float2 uv_polar(float2 uv, float2 center){
				float2 c = uv - center;
				float rad = length(c);
				float ang = atan2(c.x,c.y);
				return float2(ang, rad);
			}

			float2 uv_lens_half_sphere(float2 uv, float2 position, float radius, float refractivity){
				float2 polar = uv_polar(uv, position);
				float cone = clamp(1.-polar.y/radius, 0., 1.);
				float halfsphere = sqrt(1.-pow(cone-1.,2.));
				float w = atan2(1.-cone, halfsphere);
				float refrac_w = w-asin(sin(w)/refractivity);
				float refrac_d = 1.-cone - sin(refrac_w)*halfsphere/cos(refrac_w);
				float2 refrac_uv = position + float2(sin(polar.x),cos(polar.x))*refrac_d*radius;
				return mix(uv, refrac_uv, float(length(uv-position)<radius));
			}

			float4 frag (v2f i) : COLOR
			{
				float2 aspect = float2(1.,_ScreenResolution.y/_ScreenResolution.x);
				float2 uv_correct = 0.5 + (i.texcoord - 0.5)* aspect;

				float2 pos = float2((0.5 + _SpherePositionX / 2), (0.5 - _SpherePositionY / 2));

				float radius = 0.100 * _SphereSize;
				float strgth = 1.075 * _Strength;
				float2 uv_lense_distorted = uv_lens_half_sphere(uv_correct, pos, radius, strgth);

				uv_lense_distorted = 0.5 + (uv_lense_distorted - 0.5) / aspect;

				return tex2D(_MainTex, uv_lense_distorted);	
			}

			ENDCG
		}

	}
}
