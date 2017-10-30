// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_8bits" {
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

float hash(float2 p) 
{ 
return frac(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x)))); 
}


float compare(float3 a, float3 b) {
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

float2 c = q ;
float2 coord = c ;

float3 src = tex2D(_MainTex, coord).rgb;

float3 dst0 = 0;
float3 dst1 = 0;
float best0 = 1e3;
float best1 = 1e3;

src = src+_Distortion;

#define TCOLOR(R, G, B) { const float3 tst = float3(R, G, B); float err = compare(src, tst); if (err < best0) { best1 = best0; dst1 = dst0; best0 = err; dst0 = tst; } }

TCOLOR(0.,0.,0.);
TCOLOR(0.62890625,0.30078125,0.26171875);
TCOLOR(0.4140625,0.75390625,0.78125);
TCOLOR(0.6328125,0.33984375,0.64453125);
TCOLOR(0.359375,0.67578125,0.37109375);
TCOLOR(0.30859375,0.265625,0.609375);
TCOLOR(0.79296875,0.8359375,0.53515625);
TCOLOR(0.63671875,0.40625,0.2265625);
TCOLOR(0.4296875,0.32421875,0.04296875);
TCOLOR(0.796875,0.49609375,0.4609375);
TCOLOR(0.38671875,0.38671875,0.38671875);
TCOLOR(0.54296875,0.54296875,0.54296875);
TCOLOR(0.60546875,0.88671875,0.61328125);
TCOLOR(0.5390625,0.49609375,0.80078125);
TCOLOR(0.68359375,0.68359375,0.68359375);
TCOLOR(1.,1.,1.);
#undef TRY	

float4 FragColor = float4(mod(q.x + q.y, 2.0) > (hash(q * 0.5 ) * 0.75) + (best1 / (best0 + best1)) ? dst1 : dst0, 1.0);
return FragColor;

}

ENDCG
}

}
}