// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_Hexagon_Black" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Value ("_Value", Range(0.2, 10.0)) = 1
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
uniform float _Value;

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


float hexDist(float2 a, float2 b){
float2 p = abs(b-a);
float s = 0.5;
float c = 0.8660254;

float diagDist = s*p.x + c*p.y;
return max(diagDist, p.x)/c;
}

float2 nearestHex(float s, float2 st){
float h = 0.5*s;
float r = 0.8660254*s;
float b = s + 2.0*h;
float a = 2.0*r;
float m = h/r;

float2 sect = st/float2(2.0*r, h+s);
float2 sectPxl = fmod(st, float2(2.0*r, h+s));

float aSection = fmod(floor(sect.y), 2.0);

float2 coord = floor(sect);
if(aSection > 0.0){
if(sectPxl.y < (h-sectPxl.x*m)){
coord -= 1.0;
}
else if(sectPxl.y < (-h + sectPxl.x*m)){
coord.y -= 1.0;
}

}
else{
if(sectPxl.x > r){
if(sectPxl.y < (2.0*h - sectPxl.x * m)){
coord.y -= 1.0;
}
}
else{
if(sectPxl.y < (sectPxl.x*m)){
coord.y -= 1.0;
}
else{
coord.x -= 1.0;
}
}
}

float xoff = fmod(coord.y, 2.0)*r;
return float2(coord.x*2.0*r-xoff, coord.y*(h+s))+float2(r*2.0, s);
}


float4 frag (v2f i) : COLOR
{
float2 uv 		= i.texcoord.xy;
float   s 		= _Value * _ScreenResolution.x/160.0;
float2 nearest 	= nearestHex(s, i.texcoord.xy * _ScreenResolution.xy);
float4 texel 	= tex2D(_MainTex, nearest/_ScreenResolution.xy);
float dist 		= hexDist(i.texcoord.xy * _ScreenResolution.xy, nearest);
float luminance = (texel.r + texel.g + texel.b)/3.0;
float interiorSize = s;
float interior = 1.0 - smoothstep(interiorSize-1.0, interiorSize, dist);

return float4(texel.rgb*interior, 1.0);	
}

ENDCG
}

}
}