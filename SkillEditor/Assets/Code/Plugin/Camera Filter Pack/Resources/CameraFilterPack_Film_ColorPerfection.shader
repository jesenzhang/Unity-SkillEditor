// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Film_ColorPerfection" { 
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
float Cubic(float value) 
{
if (value < 0.5) { return value * value * value * 4.0; }
value -= 1.0; 
return value * value * value * 4.0 + 1.0;
}

float Sigmoidal (float x) 
{
return 1.0 / (1.0 + (exp(-(x - 0.5) * 14.0))); 
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float4 C = tex2D(_MainTex, uv);
float4 A = C; 
C = float4(Sigmoidal (C.r), Sigmoidal (C.g),Sigmoidal (C.b), 1.0); 
C = float4(pow(C.r, _Value), pow(C.g, _Value), pow(C.b,_Value), 1.0); 
return  C;
}
ENDCG
}
}
}
