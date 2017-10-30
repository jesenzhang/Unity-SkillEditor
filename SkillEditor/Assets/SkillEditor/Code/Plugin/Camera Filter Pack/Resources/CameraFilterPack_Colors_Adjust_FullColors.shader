// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Colors_Adjust_FullColors" { 
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
uniform float _Red_R;
uniform float _Red_G;
uniform float _Red_B;
uniform float _Green_R;
uniform float _Green_G;
uniform float _Green_B;
uniform float _Blue_R;
uniform float _Blue_G;
uniform float _Blue_B;
uniform float _Red_C;
uniform float _Green_C;
uniform float _Blue_C;

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

float4 col = tex2D( _MainTex, uv );
float3 c_r = float3(_Red_R,_Red_G,_Red_B);
float3 c_g = float3(_Green_R,_Green_G,_Green_B);
float3 c_b = float3(_Blue_R,_Blue_G,_Blue_B);
float3 rgb = float3( dot(col.rgb,c_r)+_Red_C, dot(col.rgb,c_g)+_Green_C, dot(col.rgb,c_b)+_Blue_C );


return  float4( rgb, 1 );
}
ENDCG
}
}
}
