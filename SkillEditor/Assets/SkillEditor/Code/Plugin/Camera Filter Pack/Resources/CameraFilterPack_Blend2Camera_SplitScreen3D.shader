// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Blend2Camera_SplitScreen3D" { 
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_TimeX("Time", Range(0.0, 1.0)) = 1.0
		_Distortion("_Distortion", Range(0.0, 1.00)) = 1.0
		_ScreenResolution("_ScreenResolution", Vector) = (0.,0.,0.,0.)
		_ColorRGB("_ColorRGB", Color) = (1,1,1,1)

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
	uniform sampler2D _MainTex2;
	uniform float _TimeX;
	uniform float _Distortion;
	uniform float4 _ScreenResolution;
	uniform float4 _ColorRGB;
	uniform float _Near;
	uniform float _Far;
	uniform float _FarCamera;
	uniform sampler2D _CameraDepthTexture;
	uniform float _FixDistance;
	uniform float _DistortionLevel;
	uniform float _DistortionSize;
	uniform float _LightIntensity;
	uniform float2 _MainTex_TexelSize;

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
		fixed4 color : COLOR;
		float4 projPos : TEXCOORD1;
	};

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color;
		OUT.projPos = ComputeScreenPos(OUT.vertex);

		return OUT;
	}


	float4 frag(v2f i) : COLOR
	{
		float2 uv = i.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
		uv.y = 1 - uv.y;
		#endif

		float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);

		depth /= _FixDistance * 10;
		float ss = smoothstep(_Near, saturate(_Near + _Far), depth);
		depth = 1-ss;// *(1 - ss);

		float4 result = float4(1 - depth,1 - depth,1 - depth,1);

		float4 txt = tex2D(_MainTex,uv);
		float4 txt2 = tex2D(_MainTex2,uv);

		txt = lerp(txt,txt2, saturate(result));
	    

		return txt;
	}

		ENDCG
	}

	}
}