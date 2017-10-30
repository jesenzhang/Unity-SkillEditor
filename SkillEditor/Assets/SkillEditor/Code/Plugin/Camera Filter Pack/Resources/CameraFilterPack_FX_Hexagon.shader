// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_Hexagon" {
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



inline float2 hexCoord(float2 hexIndex) {

float i = hexIndex.x;
float j = hexIndex.y;	
float2 r;
r.x = i * ((3./2.) * 0.032/sqrt(3.));
r.y = j * 0.032 + (fmod(i,2.0)) * 0.032/2.;
return r;
}

inline float2 hexIndex(float2 coord) {

float2 r;
float x 	= coord.x;
float y 	= coord.y;
float it 	= float(floor(x/0.027714));
float yts 	= y - (fmod(it,2.0)) * 0.016;
float jt 	= float(floor(31.25 * yts));
float xt 	= x - it * 0.027714;
float yt 	= yts - jt * 0.032;
float deltaj= (yt > 0.016)? 1.0:0.0;
float fcond = 0.027714 * 0.66667 * abs(0.5 - yt/0.032);

if (xt > fcond) {
r.x = it;
r.y = jt;
}
else {
r.x = it - 1.0;
r.y = jt - (fmod(r.x,2.0)) + deltaj;
}

return r;
}

fixed4 frag (v2f i) : COLOR
{
float2 hexIx = hexIndex(i.texcoord.xy);
float2 hexXy = hexCoord(hexIx);

return tex2D(_MainTex, hexXy);	
}

ENDCG
}

}
}