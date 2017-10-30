// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Vision_Drost" { 
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
uniform float _Value2;
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
float4 texturex(float2 uv) {
return float4(tex2D(_MainTex,uv).rgb,float(uv.x >= 0. && uv.y >= 0. && uv.x <= 1. && uv.y <= 1.));
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy ;
#define PI 3.1415
float atans = (atan2(uv.x-0.5,uv.y-0.5)+PI)/(PI*2.);
float3 draw = float3(0.,0.,0.);
float time = _TimeX*_Value2;
for (float i = 0.; i < 4.; i++)
{ 
float2 uvi = float2(((uv-0.5)*( i-frac(time+(atans*1.)) ))+0.5);
draw = lerp(draw,texturex(uvi).rgb,texturex(uvi).a);
}
float4 src=tex2D(_MainTex,uv);
float dist2 = 1.0 - smoothstep(_Value,_Value-0.25, length(float2(0.5,0.5) - uv));
draw.rgb=lerp(src.rgb,draw,dist2);

return  float4(draw,1.0);
}
ENDCG
}
}
}
