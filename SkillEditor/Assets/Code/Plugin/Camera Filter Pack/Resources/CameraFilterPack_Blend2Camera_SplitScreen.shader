// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Blend2Camera_SplitScreen" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
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
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
uniform float _Value5;
uniform float _Value6;
uniform float _ForceYSwap;
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


float4 frag (v2f i) : COLOR
{
float2 uv=i.texcoord.xy;
float4 tex = tex2D(_MainTex,uv);    
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif
float2 p = uv- float2(_Value3,_Value6);
float a = _Value5;
float c = cos(a); 
float s = sin(a);
uv = float2( p.x*c - p.y*s, p.x*s + p.y*c );
float2 uv2=i.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif
float4 tex2 = tex2D(_MainTex2,uv2);
float3 S1=lerp(tex.rgb,tex2.rgb,_Value2);
float3 S2=lerp(tex.rgb,tex2.rgb,1-_Value2);
float black = 1.0 - smoothstep(0,_Value4, length(float2(0.0,0.0) - max(uv.y,0)));
S1=lerp(S1,S2,black*_Value);

return  float4(S1,1.0);
}
ENDCG
}
}
}
