// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/AAA_Super_Computer" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Value ("_Value", Range(0.2, 10.0)) = 8.1
_BorderSize ("_BorderSize", Range(-0.5, 0.5)) = 0.0
_BorderColor ("_BorderColor", Color) = (0,0.5,1,1)
_SpotSize ("_SpotSize", Range(0, 1.)) = 0.5
_AlphaHexa ("_AlphaHexa", Range(0.2, 10.0)) = 0.608
_PositionX ("_PositionX", Range(-0.5, 0.5)) = 0.0
_PositionY ("_PositionY", Range(-0.5, 0.5)) = 0.0
_Radius ("_Radius", Range(0, 1.)) = 0.5
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
uniform float _Value;
uniform float _Value2;
uniform float _BorderSize;
uniform float4 _BorderColor;

uniform float _AlphaHexa;

uniform float _PositionX;
uniform float _PositionY;
uniform float _Radius;			
uniform float _SpotSize;			


uniform float time;
uniform float2 mouse;
uniform float2 resolution;

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


float2 rotate(float2 p, float a)
{
return float2(p.x * cos(a) - p.y * sin(a), p.x * sin(a) + p.y * cos(a));
}

float rand(float n)
{
return frac(sin(n) * 43758.5453123);
}

float2 rand2(in float2 p)
{
return frac(float2(sin(p.x * 591.32 + p.y * 154.077 + _TimeX), cos(p.x * 391.32 + p.y * 49.077 + _TimeX)));
}

float noise1(float p)
{
float fl = floor(p);
float fc = frac(p*2345.12);
return lerp(rand(fl), rand(fl + 1.0), fc);
}
float voronoi(in float2 x)
{
float2 p = floor(x);
float2 f = frac(x);
float2 res = float2(8.0,8.0);
for(int j = -1; j <= 1; j ++)
{
for(int i = -1; i <= 1; i ++)
{
float2 b = float2(i, j);
float2 r = float2(b) - f + rand2(p + b)*_BorderSize;
float d = max(abs(r.x), abs(r.y));

if(d < res.x)
{
res.y = res.x;
res.x = d;
}
else if(d < res.y)
{
res.y = d;
}
}
}
return res.y - res.x*_Value2;
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float4 tex=tex2D(_MainTex,uv);
uv = (uv - 0.5) * 2.0;
float2 suv = uv;
//uv.x *= resolution.x / resolution.y;
float flicker = noise1(_TimeX * 1.3) * 0.8 + 0.4;
float v = 0.0;
uv = rotate(uv, sin(0.0 * 0.3) * 1.0);
float a = 0.6, f = 1.0;
for(int i = 0; i < 3; i ++)
{	
float v1 = voronoi(uv * f + 5.0);
float v2 = 0.0;
if(i > 0)
{
v2 = voronoi(uv * f * 0.5 + 50.0 + _TimeX);
float va = 0.0, vb = 0.0;
va = 1.0 - smoothstep(0.0, 0.1, v1);
vb = 1.0 - smoothstep(0.0, 0.08, v2);
v += a * pow(va * (0.5 + vb), 2.0);
}
v1 = 1.0 - smoothstep(0.0, 0.3, v1);
v2 = a * (noise1(v1 * 5.5 + 0.1));
if(i == 0)
v += v2 * flicker;
else
v += v2;
f *= 3.0;
a *= 0.7;
}
v *= exp(-0.6 * length(suv)) * 1.2;
float3 cexp = float3(2.5, 2.5, 2.5);
float3 col = float3(pow(v, cexp.x), pow(v, cexp.y), pow(v, cexp.z)) * _Value;
col*= _BorderColor.rgb;
float2 center = float2(_PositionX,_PositionY);
float dist2 = 1.0 - smoothstep( _Radius,_Radius+0.15*_SpotSize, length(center - uv));
col.rgb=lerp(col.rgb+tex.rgb,tex.rgb,dist2);
col.rgb=lerp(col.rgb,tex.rgb,1-_AlphaHexa);
return float4(col, 1.0);

}

ENDCG
}

}
}