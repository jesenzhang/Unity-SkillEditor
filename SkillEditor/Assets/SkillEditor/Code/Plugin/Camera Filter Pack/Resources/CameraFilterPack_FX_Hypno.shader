// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/FX_Hypno" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
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
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
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
inline float mod(float x,float modu) {
return x - floor(x * (1.0 / modu)) * modu;
} 


float cercle(float val, float s1, float s2, float e1, float e2) {
return smoothstep(s1, s2, val)*(1. - smoothstep(e1, e2, val));
}

float3 t(float2 uv, float gugguu) {
float x = mod(uv.x, 0.6);
float y = mod(uv.y, 0.5);
return float3(x, y, 1.0);	
}

float3 at(float2 uv){
float v = mod(uv.x, 0.15);
float a = mod(uv.y, 0.15);
float g = cercle(v, 0.02, 0.05, 0.06, 0.08)+cercle(a, 0.02, 0.05, 0.06, 0.08);
float3 tt=float3(t(uv,g)*g);
tt.r*=_Value2;
tt.g*=_Value3;
tt.b*=_Value4;
return tt;

}

float2 rotate(float2 v, float a) {
return float2(cos(a)*v.x-sin(a)*v.y, sin(a)*v.x+cos(a)*v.y);
}

float4 frag (v2f i) : COLOR
{

float2 uv = i.texcoord.xy;
uv = uv*2. - 1.;
uv*=uv;
float tx=_TimeX*_Value;
uv = rotate(uv, sin(tx)+tx);
float3 antti = at(uv*(sin(tx*3.)+2.));
float3 res= tex2D( _MainTex, float2(i.texcoord));

res+=float3(abs(antti));
return  float4(res, 1.0);
}
ENDCG
}
}
}
