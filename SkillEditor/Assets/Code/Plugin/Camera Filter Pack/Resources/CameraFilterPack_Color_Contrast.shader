// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Color_Contrast" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Contrast  ("_Contrast", Range(0.0, 10.0)) = 1.0
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
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float _Brightness;
uniform float _Saturation;
uniform float _Contrast ;    

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


fixed4 frag (v2f i) : COLOR
{
float4 col = float4(0.,0.,0.,1.);
float3 brtColor 	= tex2D(_MainTex, i.texcoord.xy);
float intensityf 	= dot(brtColor, float3(0.2125,0.7154,0.0721));
col.rgb				= lerp(float3(0.5,0.5,0.5), lerp(float3(intensityf, intensityf, intensityf), brtColor, 1.0), _Contrast);
return col;	
}

ENDCG
}

}
}