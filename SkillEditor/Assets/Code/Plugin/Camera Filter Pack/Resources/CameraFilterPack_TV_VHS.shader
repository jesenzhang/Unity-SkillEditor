// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/TV_VHS" { 
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
inline float mod(float x,float modu) 
{
return x - floor(x * (1.0 / modu)) * modu;
} 

float rand(float2 co)
{
return frac(sin(dot(co.xy ,float2(11.9898,75.133))) * 43528.1483);
}

float4 frag (v2f i) : COLOR
{
float4 text= float4(0,0,0,0);
float2 uv = i.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
uv.y = 1-uv.y;
#endif 
float parasite = 98765;
uv.y = mod(uv.y + _TimeX*_Value3, 1.0);
uv.x = uv.x+(rand(float2(_TimeX,i.texcoord.y))-0.5)/_Value;
uv.y = uv.y+(rand(float2(_TimeX,_TimeX))-0.5)/_Value2;
text= text+ (float4(-0.5,-0.5,-0.5,-0.5)+float4(rand(float2(i.texcoord.y,_TimeX)),rand(float2(i.texcoord.y,_TimeX+1.0)),rand(float2(i.texcoord.y,_TimeX+2.0)),0))*0.1;
parasite = rand(float2(floor(uv.y*80.0),floor(uv.x*50.0))+float2(_TimeX,0));
#if UNITY_UV_STARTS_AT_TOP
uv.y = 1-uv.y;
#endif 
text= text+ tex2D(_MainTex,uv);
if (parasite > 11.5-30.0*uv.y || parasite < 1.5-5.0*uv.y) 
{
} 
else
{
text+= float4(1,1,1,1)*_Value4;
}
return text;
}
ENDCG
}
}
}
