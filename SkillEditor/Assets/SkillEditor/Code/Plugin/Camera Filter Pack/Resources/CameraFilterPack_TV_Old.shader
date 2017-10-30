// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_Old" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(1.0, 10.0)) = 1.0
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


float rand(float2 co){
return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
}

float4 frag (v2f i) : COLOR
{

float2 uv  = i.texcoord;
float screenRatio = uv.x / uv.y;
float3 textur = tex2D(_MainTex, uv).rgb;
float blurBar = clamp(sin(uv.y * 6.0 + _TimeX * 5.6) + 1.25, 0.0, 1.0);
float3 color = clamp(rand(float2(floor(uv.x * 200.0 * screenRatio), floor(uv.y * 200.0)) * _TimeX / 1000.) + 0.50, 0.0, 1.0);
color = lerp(color -  3 * 0.25, color, blurBar);
color = (float)lerp(0.0, textur, color);
color.b += 0.052;
color *= 1.0 - pow(distance(uv, float2(0.5, 0.5)), 2.1) * 2.8;
return float4(color, 1.0);

}

ENDCG
}

}
}