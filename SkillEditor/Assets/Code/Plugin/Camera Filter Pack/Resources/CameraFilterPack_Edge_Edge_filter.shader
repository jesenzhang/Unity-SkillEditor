// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Edge_Edge_filter" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_RedAmplifier ("_RedAmplifier", Range(0.0, 10.0)) = 0.0
_GreenAmplifier ("_GreenAmplifier", Range(0.0, 10.0)) = 2.0
_BlueAmplifier ("_BlueAmplifier", Range(0.0, 10.0)) = 0.0
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
uniform float _RedAmplifier;
uniform float _GreenAmplifier;
uniform float _BlueAmplifier;

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

float4 getPixel(in int x, in int y, v2f i)
{
return tex2D(_MainTex, ((i.texcoord.xy * _ScreenResolution.xy) + float2(x, y)) / _ScreenResolution.xy);
}

float4 frag (v2f i) : COLOR
{
fixed4 sum = abs(getPixel(0, 1, i) - getPixel(0, -1, i));
sum += abs(getPixel(1, 0, i) - getPixel(-1, 0, i));
sum /= 2.0;
fixed4 color = getPixel(0, 0, i);
color.r += length(sum) * _RedAmplifier;
color.g += length(sum) * _GreenAmplifier;
color.b += length(sum) * _BlueAmplifier;
return color;
}

ENDCG
}

}
}