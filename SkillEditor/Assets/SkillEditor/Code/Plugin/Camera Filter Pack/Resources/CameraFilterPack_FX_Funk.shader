// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_Funk" {
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



float3 rainbow(float h) {
h = fmod(h, 1.0);
float h6 = h * 6.0;
float r = clamp(h6 - 4.0, 0.0, 1.0) + clamp(2.0 - h6, 0.0, 1.0);
float g = h6 < 2.0 ? clamp(h6, 0.0, 1.0) 	 	: clamp(4.0 - h6, 0.0, 1.0);
float b = h6 < 4.0 ? clamp(h6 - 2.0, 0.0, 1.0) 	: clamp(6.0 - h6, 0.0, 1.0);
return float3(r, g, b);
}

float3 plasma(float2 uv)
{

float2 tuv = uv;

uv *= 2.5;

float a = 1.099609 + _TimeX * 2.70440625;
float b = 0.455078 + _TimeX * 2.12428125;
float c = 8.447266 + _TimeX * 1.90246875;
float d = 610.460939 + _TimeX * 2.4399375;

float n = 	sin(a + 3.0 * uv.x) +
sin(b - 4.0 * uv.x) +
sin(c + 2.0 * uv.y) +
sin(d + 5.0 * uv.y);

n = fmod(((4.0 + n) / 4.0), 1.0);

float4 col = tex2D(_MainTex, tuv);

n += 	col.r*0.2 + col.g * 0.4 + col.b *0.2;

return rainbow(n);
}



float4 frag (v2f i) : COLOR
{
return float4(plasma(i.texcoord.xy),1.0);	
}

ENDCG
}

}
}