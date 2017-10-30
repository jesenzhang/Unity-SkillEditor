// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_Chromatical" {
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
float2 uv = i.texcoord.xy;
float d = length(uv - float2(0.5,0.5));
float blur = 0.0;	
blur = (1.0 + sin(_TimeX*6.0)) * 0.5;
blur *= 1.0 + sin(_TimeX*16.0) * 0.5;
blur = pow(blur, 3.0);
blur *= 0.05;
blur *= d;
float3 col;
col.r = tex2D( _MainTex, float2(uv.x+blur,uv.y) ).r;
col.g = tex2D( _MainTex, uv ).g;
col.b = tex2D( _MainTex, float2(uv.x-blur,uv.y) ).b;
float scanline = sin(uv.y*800.0)*0.04;
col -= scanline;
col *= 1.0 - d * 0.5;
return float4(col,1.0);

}

ENDCG
}

}
}