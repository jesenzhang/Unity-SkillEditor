// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Edge_Neon" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_EdgeWeight ("_EdgeWeight", Range(1.0, 10.0)) = 1.0
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
uniform float _EdgeWeight;

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

float offset = 1.0 / (_EdgeWeight * 100);
float3 o = float3(-offset, 0.0, offset);
fixed4 gx = 0.0;
fixed4 gy = 0.0;
float4 t;

gx += tex2D(_MainTex, uv + o.xz);
gy += gx;
gx += 2.0*tex2D(_MainTex, uv + o.xy);
t = tex2D(_MainTex, uv + o.xx);

gx += t;
gy -= t;
gy += 2.0*tex2D(_MainTex, uv + o.yz);
gy -= 2.0*tex2D(_MainTex, uv + o.yx);
t = tex2D(_MainTex, uv + o.zz);

gx -= t;
gy += t;
gx -= 2.0*tex2D(_MainTex, uv + o.zy);
t = tex2D(_MainTex, uv + o.zx);
gx -= t;
gy -= t;
fixed4 grad = sqrt(gx*gx + gy*gy);

return float4(grad);
}

ENDCG
}

}
}