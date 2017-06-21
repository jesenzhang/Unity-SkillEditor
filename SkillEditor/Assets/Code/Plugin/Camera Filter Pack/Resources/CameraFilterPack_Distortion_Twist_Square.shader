// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_Twist_Square" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)	
_CenterX ("_CenterX", Range(-1.0, 1.0)) = 0
_CenterY ("_CenterY", Range(-1.0, 1.0)) = 0
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
uniform float4 _ScreenResolution;
uniform float _CenterX;
uniform float _CenterY;
uniform float _Size;

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

float4 twist(sampler2D tex, float2 uv, float time)
{
float radius = 0.5*_Size;
float2 center = float2(_CenterX,_CenterY);
float2 tc = uv - center;
float dist = length(tc*tc);
if (dist < radius)
{
float percent = (radius - dist) / radius;
float theta = percent * percent * (2.0 * sin(time)) * 8.0;
float s = sin(theta);
float c = cos(theta);
tc = float2(dot(tc, float2(c, -s)), dot(tc, float2(s, c)));
}
tc += center;
float4 color = tex2D(tex, tc);
return color;
}

float4 frag (v2f i) : COLOR
{

float2 uv = i.texcoord;
float4 finalColor = twist(_MainTex, uv, _Distortion);
return finalColor;

}

ENDCG
}

}
}