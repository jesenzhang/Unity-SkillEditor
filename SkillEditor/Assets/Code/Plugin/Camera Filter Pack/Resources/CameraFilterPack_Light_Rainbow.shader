// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Light_Rainbow" { 
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

inline float modx(float x,float modu) {
return x - floor(x * (1.0 / modu)) * modu;
}  

inline float3 modx(float3 x,float3 modu) {
return x - floor(x * (1.0 / modu)) * modu;
} 

float3 hsv2rgb( in float3 c )
{
float3 rgb = clamp( abs(modx(c.x*6.0+float3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
return c.z * lerp( float3(1.0,1.0,1.0), rgb, c.y);
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
uv -= float2(0.5,0.5);
uv.y *= _Value;    
uv += sin(uv.x * 10. * (uv.y * 1.11)  + _TimeX) * 0.15;
float m = clamp((.7 - abs( uv.y )) * 3.,0.,1.);    
float3 V = hsv2rgb( float3((uv.x * 0.1) + _TimeX * 0.25 ,1.,1.));
V *=  m;    
V *= 1. - (sin( uv.y * uv.y * 30. ) * .26);
V+=tex2D(_MainTex, i.texcoord.xy)/2;
return  float4(V,1.0);
}
ENDCG
}
}
}
