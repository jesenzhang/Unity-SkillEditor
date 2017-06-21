// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_ZebraColor" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Value ("_Value", Range(1.0, 10.0)) = 10.0
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
uniform float _Value;
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

inline float mod(float x,float modu) {
return x - floor(x * (1.0 / modu)) * modu;
}   



float4 frag (v2f i) : COLOR
{
float t 	= _Value;
float2 uv 	= i.texcoord.xy ;
float3 txt 	= tex2D(_MainTex,float2(uv.x,uv.y)).rgb;
float3 col 	= txt/length(txt); 


float lum 	= (txt.r+txt.g+txt.b)/3.;
float rg	= atan2(txt.r,txt.g);	

float a,b;
a = 2.*rg;  	
a = floor(t*a/3.14159265359)*3.14159265359/t +3.14159265359/2.;  
b = lum;	
b = floor(t*b)/t;

float2 dir = float2(cos(a),sin(a)); 
a = 2.*3.14159265359*dot(dir,i.texcoord.xy * _ScreenResolution.xy);
col =( .5-.5*cos(.5*b*a))*float3(txt/length(txt));

return float4(col,1.0);	
}

ENDCG
}

}
}