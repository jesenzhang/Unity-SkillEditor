// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Color_Switching" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 8.0)) = 8.0
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

fixed4 col=fixed4(1.0,1.0,1.0,1.0);
if (_Distortion==0) col.rgb = tex2D(_MainTex, i.texcoord.xy).rgb;
if (_Distortion==1) col.rgb = tex2D(_MainTex, i.texcoord.xy).rbg;
if (_Distortion==2) col.rgb = tex2D(_MainTex, i.texcoord.xy).gbr;
if (_Distortion==3) col.rgb = tex2D(_MainTex, i.texcoord.xy).grb;
if (_Distortion==4) col.rgb = tex2D(_MainTex, i.texcoord.xy).bgr;
if (_Distortion==5) col.rgb = tex2D(_MainTex, i.texcoord.xy).brg;

return col;

}

ENDCG
}

}
}