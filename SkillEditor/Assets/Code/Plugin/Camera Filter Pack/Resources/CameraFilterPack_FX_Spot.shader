// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_Spot" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_PositionX ("_PositionX", Range(-0.5, 0.5)) = 0.0
_PositionY ("_PositionY", Range(-0.5, 0.5)) = 0.0
_Radius ("_Radius", Range(0, 1.)) = 0.5

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
uniform float _PositionX;
uniform float _PositionY;
uniform float _Radius;			
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
float2 uv = i.texcoord.xy;
float4 tex = tex2D(_MainTex, uv);
float2 center = float2(_PositionX,_PositionY);
float dist = 1.0 - smoothstep( _Radius,_Radius+0.15, length(center - uv) );

return fixed4( dist * tex.rgb, 1.0 );	
}

ENDCG
}

}
}