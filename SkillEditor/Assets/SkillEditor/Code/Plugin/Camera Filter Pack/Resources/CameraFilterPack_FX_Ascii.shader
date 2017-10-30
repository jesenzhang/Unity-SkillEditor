// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_Ascii" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
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
uniform float Value;
uniform float Fade;
uniform float _Distortion;
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

float character(float n, float2 p) 
{
p = floor(p*float2(4.0, -4.0) + 2.5);
if (clamp(p.x, 0.0, 4.0) == p.x && clamp(p.y, 0.0, 4.0) == p.y)
{
float c = fmod(n/exp2(p.x + 5.0*p.y), 2.0);
if (int(c) == 1) return 1.0;
}	
return 0.0;
}

float4 frag (v2f i) : COLOR
{
float2 uv  = i.texcoord.xy * _ScreenResolution.xy;

float vn = 8 * Fade;
float3 col = tex2D(_MainTex,floor(uv/vn)*vn/_ScreenResolution.xy).rgb;	


float gray = (col.r + col.b)/Value; 
float n =  65536.0;             // .c
if (gray > 0.2) n = 65600.0;    // :
if (gray > 0.3) n = 332772.0;   // *
if (gray > 0.4) n = 15255086.0; // o 
if (gray > 0.5) n = 23385164.0; // &
if (gray > 0.6) n = 15252014.0; // 8
if (gray > 0.7) n = 13199452.0; // @
if (gray > 0.8) n = 11512810.0; // #

float2 p = fmod(uv/4.0, 2.0) - 1.0;
col = lerp(col, col*character(n, p), Fade);
return float4(col, 1.0);	
}

ENDCG
}

}
}