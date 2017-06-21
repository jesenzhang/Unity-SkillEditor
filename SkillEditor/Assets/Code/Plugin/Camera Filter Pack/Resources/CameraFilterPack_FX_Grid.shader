// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_Grid" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 5.0)) = 1.0
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

float4 frag (v2f i) : COLOR
{

float2 scale = float2(60.0, 35.3);
float width = 0.3*_Distortion;
float2 uv = i.texcoord;
float2 pos = frac(uv * scale);
float2 coord = floor(uv * scale) / scale;
float xb = dot(tex2D(_MainTex, float2(coord.x, uv.y)).xyz, 0.33);
float yb = dot(tex2D(_MainTex, float2(uv.x, coord.y)).xyz, 0.33);
float lit = float(abs(pos.y - width / 2.0 - (1.0 - width) * yb) < width / 2.0 || abs(pos.x - width / 2.0 - (1.0 - width) * xb) < width / 2.0);
float b = (yb + xb) / 2.0;

return float4(0.0, lit * b + (1.0 - lit) * b * 0.3, 0.0, 1.0);

}

ENDCG
}

}
}