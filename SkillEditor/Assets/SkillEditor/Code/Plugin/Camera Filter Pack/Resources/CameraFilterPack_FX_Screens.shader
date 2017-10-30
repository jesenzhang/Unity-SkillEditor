// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/FX_Screens" { 
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

inline float modx(float x,float modu) {
return x - floor(x * (1.0 / modu)) * modu;
}  

inline float3 modx(float3 x,float3 modu) {
return x - floor(x * (1.0 / modu)) * modu;
} 

inline float4 modx(float4 x,float4 modu) {
return x - floor(x * (1.0 / modu)) * modu;
} 


float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float4 noise = tex2D(_MainTex, floor(uv * float(_Value)) / float(_Value));
float p = 1.0 - modx(noise.r + noise.g + noise.b + _TimeX * float(_Value2), 1.0);
p = min(max(p * 3.0 - 1.8, 0.1), 2.0);
float2 r = modx(uv * float(_Value), 1.0);
r = float2(pow(r.x - 0.5, 2.0), pow(r.y - 0.5, 2.0))-float2(_Value3,_Value4);
p *= 1.0 - pow(min(1.0, 12.0 * dot(r, r)), 2.0);
float4 cam=float4(0.7, 1.6, 2.8, 1.0) * p;
cam+=tex2D(_MainTex,uv);
return cam;
}
ENDCG
}
}
}
