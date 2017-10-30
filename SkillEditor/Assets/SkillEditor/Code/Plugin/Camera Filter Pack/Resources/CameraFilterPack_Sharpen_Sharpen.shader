// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Sharpen_Sharpen" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Value ("Value", Range(0.0, 1.0)) = 1.0
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
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Value;
uniform float _Value2;
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

inline float4 sharp(float2 uv)
{
float r = _Value2/_ScreenResolution.x; 
float strength = 9.0 * _Value;

float4 c0 = tex2D(_MainTex,uv);
float4 c1 = tex2D(_MainTex,uv-float2(r,.0));
float4 c2 = tex2D(_MainTex,uv+float2(r,.0));
float4 c3 = tex2D(_MainTex,uv-float2(.0,r));
float4 c4 = tex2D(_MainTex,uv+float2(.0,r));
float4 c5 = c0+c1+c2+c3+c4; c5*=0.2;
float4 mi = min(c0,c1); mi = min(mi,c2); mi = min(mi,c3); mi = min(mi,c4);
float4 ma = max(c0,c1); ma = max(ma,c2); ma = max(ma,c3); ma = max(ma,c4);
return clamp(mi,(strength+1.0)*c0-c5*strength,ma);
}

float4 frag (v2f i) : COLOR
{
return sharp(i.texcoord.xy);	
}

ENDCG
}

}
}