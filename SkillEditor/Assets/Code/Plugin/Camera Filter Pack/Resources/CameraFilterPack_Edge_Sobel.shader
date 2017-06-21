// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////
Shader "CameraFilterPack/Edge_Sobel" {
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
#pragma glsl
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

// Use these parameters to fiddle with settings


float intensity(in float4 color){
return sqrt((color.x*color.x)+(color.y*color.y)+(color.z*color.z));
}

float3 sobel(float stepx, float stepy, float2 center){

float tleft 	= intensity(tex2D(_MainTex,center + float2(-stepx,stepy)));
float left 		= intensity(tex2D(_MainTex,center + float2(-stepx,0)));
float bleft 	= intensity(tex2D(_MainTex,center + float2(-stepx,-stepy)));
float top 		= intensity(tex2D(_MainTex,center + float2(0,stepy)));
float bottom 	= intensity(tex2D(_MainTex,center + float2(0,-stepy)));
float tright 	= intensity(tex2D(_MainTex,center + float2(stepx,stepy)));
float right 	= intensity(tex2D(_MainTex,center + float2(stepx,0)));
float bright 	= intensity(tex2D(_MainTex,center + float2(stepx,-stepy)));

float x =  tleft + 2.0 * left + bleft  - tright - 2.0 * right  - bright;
float y = -tleft - 2.0 * top  - tright + bleft  + 2.0 * bottom + bright;

float color = sqrt((x*x) + (y*y));

return float3(color,color,color);
}            

float4 frag (v2f i) : COLOR
{
return float4(sobel(1./_ScreenResolution.x, 1./_ScreenResolution.y, i.texcoord),1.0);;			
}

ENDCG
}

}
}