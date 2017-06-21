// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_Noise" {
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

float mod289(float x)
{
return x - floor(x * 0.0034602) * 289.0;
}

half4 mod289(half4 x)
{
return x - floor(x * 0.0034602) * 289.0;
}

half4 perm(half4 x)
{
return mod289(((x * 34.0) + 1.0) * x);
}

float noise3d(float3 p)
{
float3 a = floor(p);
float3 d = p - a;
d = d * d * (3.0 - 2.0 * d);

fixed4 b = a.xxyy + fixed4(0.0, 1.0, 0.0, 1.0);
fixed4 k1 = perm(b.xyxy);
fixed4 k2 = perm(k1.xyxy + b.zzww);

fixed4 c = k2 + a.zzzz;
fixed4 k3 = perm(c);
fixed4 k4 = perm(c + 1.0);

fixed4 o1 = frac(k3 * 0.0243902);
fixed4 o2 = frac(k4 * 0.0243902);

fixed4 o3 = o2 * d.z + o1 * (1.0 - d.z);
float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);

return o4.y * d.y + o4.x * (1.0 - d.y);
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float v1 = noise3d(float3(uv * 10.0, 0.0));
float v2 = noise3d(float3(uv * 10.0, 1.0));

fixed4 color  = tex2D(_MainTex, uv + float2(v1, v2) * 0.1*_Distortion);

return color;	
}

ENDCG
}

}
}