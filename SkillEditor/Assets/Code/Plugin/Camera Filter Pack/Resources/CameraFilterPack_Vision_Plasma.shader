// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////



Shader "CameraFilterPack/Vision_Plasma" { 
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
uniform float _Value3;
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

const float3 one = float3(1.0,1.0,1.0);
float time = _TimeX * 0.33;
float2 uv = i.texcoord.xy;
float2 p0 = uv - float2(0.5 + 0.5 * sin(1.4 * 6.28 * uv.x + 2.8 * time), 0.5);
float3 wave = float3(0.5 * (cos(sqrt(dot(p0, p0)) * 5.6) + 1.0),cos(4.62 * dot(uv, uv) + time),cos(distance(uv, float2(1.6 * cos(time * 2.0), 1.0 * sin(time * 1.7))) * 1.3));
float color = dot(wave, one) / _Value3;
float dist2 = 1.0 - smoothstep(_Value,_Value-0.05-_Value2, length(float2(0.5,0.5) - uv));
float3 dest=float3(
0.5 * (sin(6.28 * color + time * 3.45) + 1.0),
0.5 * (sin(6.28 * color + time * 3.15) + 1.0),
0.4 * (sin(6.28 * color + time * 1.26) + 1.0)
);
float3 ret=lerp(tex2D(_MainTex,uv),dest,dist2);
return float4(ret,1.0);
}
ENDCG
}
}
}
