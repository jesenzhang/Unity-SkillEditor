// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Edge_Golden" {
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



float4 frag (v2f i) : COLOR
{

float2 uv = i.texcoord.xy;

float2 step = 1.0 / _ScreenResolution.xy;

float3 texA = tex2D( _MainTex, uv + float2(-step.x, -step.y) * 1.5 ).rgb;
float3 texB = tex2D( _MainTex, uv + float2( step.x, -step.y) * 1.5 ).rgb;
float3 texC = tex2D( _MainTex, uv + float2(-step.x,  step.y) * 1.5 ).rgb;
float3 texD = tex2D( _MainTex, uv + float2( step.x,  step.y) * 1.5 ).rgb;

float shadeA = dot(texA, 0.333333);
float shadeB = dot(texB, 0.333333);
float shadeC = dot(texC, 0.333333);
float shadeD = dot(texD, 0.333333);

float shade = 15.0 * pow(max(abs(shadeA - shadeD), abs(shadeB - shadeC)), 0.5);

float3 col = lerp(float3(0.1, 0.18, 0.3), float3(0.4, 0.3, 0.2), shade);

return fixed4(col,1.0);

}

ENDCG
}

}
}