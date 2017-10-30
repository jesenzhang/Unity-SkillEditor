// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Atmosphere_Snow_8bits" { 
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

inline float2 mod(float2 x,float2 modu) {
return x - floor(x * (1.0 / modu)) * modu;
} 


inline float rand(float2 co)
{

float r;
co = floor(co*_Value2);
r = frac(sin(dot(co.xy,float2(12.9898,78.233))) * 13758.5453);
return r;
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;

float3 col=tex2D(_MainTex,uv).rgb;
float3 col2=col;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif


uv*=2.0;

float mov=_Value3*uv.y*8;
float2 t=float2(_TimeX,_TimeX);
uv.x+=mov*0.2;
float s=rand(mod(uv * 1.01 +t*float2(0.02,0.501),1.0)).r;
col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s * _Value));

s=rand(mod(uv * 1.07 +t*float2(0.02,0.501),1.0)).r;
col=lerp(col,float3(0.1,1.0,1.0),smoothstep(0.9,1.0, s * _Value));

s=rand(mod(uv+t*float2(0.05,0.5),1.0)).r;
col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s * _Value));

s=rand(mod(uv * .9 +t*float2(0.02,0.51),1.0)).r;
col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s * _Value));

s=rand(mod(uv * .75 +t*float2(0.07,0.493),1.0)).r;
col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s  * _Value));

s=rand(mod(uv * .5 +t*float2(0.03,0.504),1.0)).r;
col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.94,1.0, s  * _Value));

s=rand(mod(uv * .3 +t*float2(0.02,0.497),1.0)).r;
col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.95,1.0, s  * _Value));

s=rand(mod(uv * .1 +t*float2(0.0,0.51),1.0)).r;
col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.96,1.0, s  * _Value));

s=rand(mod(uv * .03 +t*float2(0.0,0.523),1.0)).r;
col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.99,1.0, s  * _Value));

col = lerp(col2,col,_Value4);
return  float4(col,1.0);
}
ENDCG
}
}
}
