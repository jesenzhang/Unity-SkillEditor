// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_CompressionFX" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Parasite ("_Parasite", Range(1.0, 10.0)) = 1.0
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
uniform float _Parasite;
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

float2 blockS = floor(uv * float2(24., 19.))*4.0;
float2 blockL = floor(uv * float2(38., 14.))*4.0;

float r = rng2(uv);

float lineNoise = pow(rng2(blockS), 3.0) *_Parasite* pow(rng2(blockL), 3.0);

float4 col1 = tex2D(_MainTex, uv + float2(lineNoise * 0.02 * rng(2.0), 0));

float4 result;
result = float4(float3(col1.x, col1.y, col1.z), 1.0);

return result;


}

ENDCG
}

}
}