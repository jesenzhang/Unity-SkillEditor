// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_Artefact" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Colorisation ("_Colorisation", Range(1.0, 10.0)) = 1.0
_Parasite ("_Parasite", Range(1.0, 10.0)) = 1.0
_Noise ("_Noise", Range(1.0, 10.0)) = 1.0
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
uniform float _Colorisation;
uniform float _Parasite;
uniform float _Noise;
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



float rng2(float2 seed)
{
return frac(sin(dot(seed * floor(_TimeX * 12.), float2(127.1,311.7))) * 43758.5453123);
}

float rng(float seed)
{
return rng2(float2(seed, 1.0));
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;

float2 blockS = floor(uv * float2(24., 9.));
float2 blockL = floor(uv * float2(8., 4.));

float r = rng2(uv);
float3 noise = (float3(r, 1. - r*_Colorisation, r / 2. + 0.5) * 1.0 *_Noise- 2.0) * 0.08;

float lineNoise = pow(rng2(blockS), 8.0) *_Parasite* pow(rng2(blockL), 3.0) - pow(rng(7.2341), 17.0) * 2.;

float4 col1 = tex2D(_MainTex, uv);
float4 col2 = tex2D(_MainTex, uv + float2(lineNoise * 0.05 * rng(5.0), 0));
float4 col3 = tex2D(_MainTex, uv - float2(lineNoise * 0.05 * rng(31.0), 0));

float4 result;
result = float4(float3(col1.x, col2.y, col3.z) + noise, 0.2);

return result;


}

ENDCG
}

}
}