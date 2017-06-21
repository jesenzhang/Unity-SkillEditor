// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Blur_Steam" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Radius ("_Radius", Range(0, 1)) = 0.1
_Quality ("_Quality", Range(0, 1)) = 0.75
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
uniform float _Radius;
uniform float _Quality;

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

#define tex2D(sampler,uvs)  tex2Dlod( sampler , float4( ( uvs ) , 0.0f , 0.0f) )

float4 blur(float2 uv) {
fixed4 col = 0.0;
if(_Quality == 1.0) _Quality = 0.99;
for(float r = 0.0 ; r < 1.0 ; r += (1.0 - _Quality)) {
for(float a = 0.0 ; a < 1.0 ; a += (1.0 - _Quality)) {
col += tex2D(_MainTex, uv + float2(cos(a*6.283184), sin(a*6.283184)) * (r*_Radius));
}
}

col *= (1.0 - _Quality) * (1.0 - _Quality);
return col;
}

float4 frag (v2f i) : COLOR
{
return blur(i.texcoord.xy);	
}

ENDCG
}

}
}