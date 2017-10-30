// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Antialiasing_FXAA" {
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


fixed4 frag (v2f i) : COLOR
{
float FXAA_SPAN_MAX = 8.0;
float FXAA_REDUCE_MUL = 0.125;
float FXAA_REDUCE_MIN = 0.0078125;

float2 texcoordOffset = float2(1.0 / _ScreenResolution.x, 1.0 / _ScreenResolution.y);

fixed3 rgbNW = tex2D(_MainTex, i.texcoord.xy + (float2(-1.0, -1.0) * texcoordOffset)).xyz;
fixed3 rgbNE = tex2D(_MainTex, i.texcoord.xy + (float2(+1.0, -1.0) * texcoordOffset)).xyz;
fixed3 rgbSW = tex2D(_MainTex, i.texcoord.xy + (float2(-1.0, +1.0) * texcoordOffset)).xyz;
fixed3 rgbSE = tex2D(_MainTex, i.texcoord.xy + (float2(+1.0, +1.0) * texcoordOffset)).xyz;
fixed3 rgbM  = tex2D(_MainTex, i.texcoord.xy).xyz;

fixed3 luma = float3(0.299, 0.587, 0.114);
half lumaNW = dot(rgbNW, luma);
half lumaNE = dot(rgbNE, luma);
half lumaSW = dot(rgbSW, luma);
half lumaSE = dot(rgbSE, luma);
half lumaM  = dot( rgbM, luma);

half lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
half lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

float2 dir;
dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
dir.y =  ((lumaNW + lumaSW) - (lumaNE + lumaSE));

half dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) * (0.03125), FXAA_REDUCE_MIN);

half rcpDirMin = 1.0/(min(abs(dir.x), abs(dir.y)) + dirReduce);

dir = min(float2(FXAA_SPAN_MAX,  FXAA_SPAN_MAX), 
max(float2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX), dir * rcpDirMin)) * texcoordOffset;

fixed3 rgbA = 0.5 * (
tex2D(_MainTex, i.texcoord.xy + dir * -0.166).xyz +
tex2D(_MainTex, i.texcoord.xy + dir * -0.166).xyz);
fixed3 rgbB = rgbA * 0.5 + 0.25 * (
tex2D(_MainTex, i.texcoord.xy + dir * - 0.5).xyz +
tex2D(_MainTex, i.texcoord.xy + dir * 0.5).xyz);
float lumaB = dot(rgbB, luma);


float4 color;

if((lumaB < lumaMin) || (lumaB > lumaMax)){
return float4(rgbA,1.0);
} 
else {
return float4(rgbB,1.0);
}		
}

ENDCG
}

}
}