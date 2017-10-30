// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Distortion_Dissipation" { 
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
float3 texCol(float2 p, float b)
{
float3 col = tex2D(_MainTex, p).xyz;
//   col *= (col.x+col.y+col.z);
return col;
}

float2 Perl(float2 p)
{
float2 x = float2(0.0,0.0);
for (int i = 0; i < 6; ++i)
{
float j = pow(2.0, float(i));
x += (tex2D(_MainTex, p * j * 0.001).xy-0.5) / j;
}
return x;
}

float3 smoke(float2 p, float2 o, float t)
{
const int steps = 10;
float3 col = float3(0.0,0.0,0.0);
for (int i = 1; i < steps; ++i)
{
p += Perl(p + o) * t * 0.01 / float(i);
p.y -= t * 0.003; //drift upwards
col += texCol(p, float(steps-i) * t * 0.2);
}
return col.xyz / float(steps);
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float t = frac(_Value / 3.0) * 6.0;
t = max(0.0, t - uv.x - 1.0 + uv.y);
t *= t; 
return  float4(smoke(uv,1.0/2.0, t), 1.0);
}
ENDCG
}
}
}
