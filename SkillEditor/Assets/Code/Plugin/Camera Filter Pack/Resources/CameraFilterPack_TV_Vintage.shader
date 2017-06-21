// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_Vintage" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(1.0, 10.0)) = 1.0
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
float2 uv = 0.5 + (i.texcoord-0.5)*(0.9 + 0.1*sin(0.1*_TimeX));
float3 col;

col.r = tex2D(_MainTex,float2(uv.x+0.003*_Distortion,uv.y)).x;
col.g = tex2D(_MainTex,float2(uv.x+0.000*_Distortion,uv.y)).y;
col.b = tex2D(_MainTex,float2(uv.x-0.003*_Distortion,uv.y)).z;
col = clamp(col*0.5+0.5*col*col*1.2,0.0,1.0);
col *= 0.5 + 8.0*uv.x*uv.y*(1.0-uv.x)*(1.0-uv.y);
col *= float3(0.95,1.05,0.95);
col *= 0.9+0.1*sin(10.0*_TimeX+uv.y*1000.0);
col *= 0.99+0.01*sin(110.0*_TimeX);

return float4(col,1.0);

}

ENDCG
}

}
}