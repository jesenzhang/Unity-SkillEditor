// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Gradients_Rainbow" { 
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

float square(float s) { return s * s; }
float3 square(float3 s) { return s * s; }

float3 rainbowGradient(float t) {
float a = 1.0 - pow(abs(float3(t,t,t) - float3(0.65, 0.5, 0.2)) * float3(3.0, 3.0, 5.0), float3(1.5, 1.3, 1.7));
float3 c=float3(a,a,a);
c.r = max((0.15 - square(abs(t - 0.04) * 5.0)), c.r);
c.g = (t < 0.5) ? smoothstep(0.04, 0.45, t) : c.g;
return clamp(c, 0.0, 1.0); 
}

float4 frag (v2f i) : COLOR
{
float t = i.texcoord.x;
float j = t + (frac(sin(i.texcoord.y * 7.5e2 + i.texcoord.x * 6.4) * 1e2) - 0.5) * 0.005;
float2 uv = i.texcoord.xy;
float4 tc = tex2D(_MainTex,uv);    
float b = (0.2126*tc.r + 0.7152*tc.g + 0.0722*tc.b);
b=lerp(b,1-b,_Value);
float3 map=lerp(tc,rainbowGradient(b),_Value2);
tc=float4(map,1.0);
return  tc;
}
ENDCG
}
}
}
