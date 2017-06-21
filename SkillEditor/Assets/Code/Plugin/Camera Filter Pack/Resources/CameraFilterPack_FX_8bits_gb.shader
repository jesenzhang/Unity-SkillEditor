// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_8bits_gb" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenX ("Time", Range(0.0, 2000.0)) = 1.0
_ScreenY ("Time", Range(0.0, 2000.0)) = 1.0
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
uniform float _ScreenX;
uniform float _ScreenY;
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

float hash(float2 p) 
{ 
return frac(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x)))); 
}


float compare(float3 a, float3 b) 
{
a*=a*a;
b*=b*b;
float3 diff = (a - b);
return dot(diff, diff);
}

inline float mod(float x,float modu) 
{
return x - floor(x * (1.0 / modu)) * modu;
}

float4 frag (v2f i) : COLOR
{

float2 q  = i.texcoord;
float2 pixelSize;
pixelSize.x = 0.00625;
pixelSize.y = 0.00694;
float2 c = q / pixelSize;
float2 coord = c * pixelSize;

float3 src = tex2D(_MainTex, coord).rgb;

float3 dst0 = 0;
float3 dst1 = 0;
float best0 = 1e3;
float best1 = 1e3;

src = src+_Distortion;

#define TRY(R, G, B) { const float3 tst = float3(R, G, B); float err = compare(src, tst); if (err < best0) { best1 = best0; dst1 = dst0; best0 = err; dst0 = tst; } }

TRY(0.03, 0.16, 0.33);
TRY(0.13, 0.43, 0.37);
TRY(0.47, 0.69, 0.42);
TRY(0.68, 0.79, 0.27);


#undef TRY	

float4 FragColor = float4(mod(q.x + q.y, 2.0) > (hash(q * 0.5 + frac(sin(float2(floor(_TimeX), floor(_TimeX))))) * 0.75) + (best1 / (best0 + best1)) ? dst1 : dst0, 1.0);

if (FragColor.r==0) FragColor.rgb=float3(0.03, 0.16, 0.33);

return FragColor;

}

ENDCG
}

}
}