// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////
Shader "CameraFilterPack/TV_Vignetting" 
{

Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
VHS ("Base (RGB)", 2D) = "white" {}
VHS2 ("Base (RGB)", 2D) = "white" {}
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
uniform sampler2D Vignette;
uniform float _Vignetting;
uniform float _Vignetting2;
uniform float _VignettingDirt;
uniform float4 _VignettingColor;

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
float4 t1 = tex2D(_MainTex, uv);
float4 t2 = tex2D(Vignette, uv)*_VignettingColor.a;
t1 = lerp(t1, _VignettingColor, t2.r * _Vignetting);
t1 = lerp(t1, _VignettingColor, t2.g * _VignettingDirt*2);
t1 = lerp(t1, _VignettingColor, t2.b * _Vignetting2);
return t1;
}

ENDCG
}
}
}