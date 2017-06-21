// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Alien_Vision" { 
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
float4 frag (v2f i) : COLOR
{

float2 uv = i.texcoord.xy;
float d = length(uv - float2(0.5,0.5));	
float blur = 0.0;	
blur = (1.0 + sin(_TimeX*6.0*_Value2)) * 0.5;
blur *= 1.0 + sin(_TimeX*16.0*_Value2) * 0.5;
blur = pow(blur, 3.0);
blur *= 0.05;
blur *= d;
float3 col;
col.r = tex2D( _MainTex, float2(uv.x+blur,uv.y) ).r;
col.g = tex2D( _MainTex, uv ).g;
col.b = tex2D( _MainTex, float2(uv.x-blur,uv.y) ).b;
float scanline = sin(uv.y*800.0)*0.04;
col -= scanline;
col *= 1.0 - d * 0.5;

float4 pixcol = float4(col,1.0);

// float4 pixcol = tex2D(_MainTex, i.texcoord.xy);
float4 colors[3];
colors[0] = float4(0.0,0.5+sin(_TimeX*1)/4.14,0.0,1.0);
colors[1] = float4(0.1,1.0+cos(_TimeX*2)/4.14,0.1,1.0);
colors[2] = float4(0.1,0.5+sin(_TimeX*4)/4.14,0.0,1.0);
float lum = (pixcol.r+pixcol.g+pixcol.b)/3.;
float4 thermal;
if (lum<_Value) thermal = lerp(colors[0],colors[2],(lum-float(0)*_Value)/_Value);
if (lum>=_Value) thermal = lerp(colors[1],colors[2],(lum-float(1)*_Value)/_Value);

return  float4(thermal.rgb,1.0);

}
ENDCG
}
}
}
