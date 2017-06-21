// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Glitch_Mozaic" { 
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
float2 uv = i.texcoord.xy ;
float x = uv.x;
float y = uv.y;
float glitchStrength = _Value;
float psize = 0.04 * glitchStrength;
float psq = 1.0 / psize;
float px = floor( x * psq + 0.5) * psize;
float py = floor( y * psq + 0.5) * psize;
float4 colSnap = tex2D( _MainTex, float2( px,py) );
float lum = pow( 1.0 - (colSnap.r + colSnap.g + colSnap.b) / 3.0, glitchStrength ); // remove the minus one if you want to invert luma
float qsize = psize * lum;
float qsq = 1.0 / qsize;
float qx = floor( x * qsq + 0.5) * qsize;
float qy = floor( y * qsq + 0.5) * qsize;
float rx = (px - qx) * lum + x;
float ry = (py - qy) * lum + y;
float4 colMove = tex2D( _MainTex, float2( rx,ry) );

return  colMove;
}

ENDCG
}
}
}
