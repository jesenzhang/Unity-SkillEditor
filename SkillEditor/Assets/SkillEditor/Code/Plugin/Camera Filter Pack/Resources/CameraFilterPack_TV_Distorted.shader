// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_Distorted" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(1.0, 10.0)) = 1.0
_RGB ("_RGB", Range(1.0, 10.0)) = 1.0
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
uniform float _RGB;

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


float snoise(float2 n) {
float t=frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
t=sin(_TimeX*n.y/35)*t;
return t;
}

float4 frag (v2f i) : COLOR
{

float2 uv 		=  i.texcoord;

float wOffset  = snoise(float2(_TimeX*15.0,uv.y*80.0))*0.003*_Distortion;
float lWOffset = snoise(float2(_TimeX,uv.y*25.0))*0.004*_Distortion;
float xOffset 	= wOffset + lWOffset;

float red 	=   tex2D(	_MainTex, 	float2(uv.x + xOffset -_RGB,uv.y)).r;
float green = 	tex2D(	_MainTex, 	float2(uv.x + xOffset,	  uv.y)).g;
float blue 	=	tex2D(	_MainTex, 	float2(uv.x + xOffset +_RGB,uv.y)).b;

float3 color 	= float3(red,green,blue);
float scanline 	= sin(uv.y*800.0)*0.04;
color -= scanline;

return float4(color,1.0);

}

ENDCG
}

}
}