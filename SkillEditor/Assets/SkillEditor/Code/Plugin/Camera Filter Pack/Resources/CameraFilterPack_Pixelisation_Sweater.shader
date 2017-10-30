// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Pixelisation_Sweater" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 1.0
_Brightness ("_Brightness", Range(0.0, 1.0)) = 1.5
_Saturation ("_Saturation", Range(0.0, 1.0)) = 3.0
_Contrast  ("_Contrast", Range(0.0, 1.0)) = 3.0
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
uniform sampler2D Texture2;
uniform float _TimeX;
uniform float _Fade;
uniform float _SweaterSize;
uniform float _Intensity;

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
float2 uv = i.texcoord;
float2 uv2 = uv;
float2 uv3 = uv;
float step= _SweaterSize;
uv2 = uv2 * step;

float4 col2 = tex2D(Texture2, uv2);
col2.g = col2.g/step;
uv.y -=col2.g;
uv.y += 1 / (step*2);
uv = floor(uv*step) / step;
float4 col = tex2D(_MainTex, uv);
uv3 = uv3 * step*0.50;
float4 col3 = tex2D(Texture2, uv3);
float step1 = 1 / step;
col = col*col2.r + (col3.b*col);
col = col*col2.r;
float4 tcol = tex2D(_MainTex, i.texcoord);
return lerp(tcol, col*_Intensity,_Fade);
}

ENDCG
}

}
}