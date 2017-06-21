// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_Video3D" {
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
#pragma glsl
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

inline float3 mix(float3 a, float3 b, float t)
{
return a * (1.0 - t) + b * t;
}



float terrain(float2 p) {
p*=.4;
return tex2D(_MainTex,p).r*.2;
}
float3 normal(float2 p) {
float2 eps=float2(0,0.004);
float d1=max(.003,terrain(p+eps.xy)-terrain(p-eps.xy));
float d2=max(.003,terrain(p+eps.yx)-terrain(p-eps.yx));
float3 n1=(float3(0.,eps.y*2.,d1));
float3 n2=(float3(eps.y*2.,0.,d2));
return normalize(cross(n1,n2));
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
uv=floor(uv*500.)/500.;
float2 p2=0.;
float height=0.;
for (int l=0; l<80; l++) {
float scan=uv.y-float(l)*0.002;
float2 p=float2(uv.x,scan*2.)+float2(.3,.15)/.4;
float h=terrain(p);
if (scan+h>uv.y) {p2=p;height=h;}
}
float3 col=tex2D(_MainTex,p2*.4).rgb;
col*=max(.2,dot(normal(p2),normalize(float3(1.,0.,-1.))));
col*=1.+pow(max(0.,dot(normal(p2),normalize(float3(1.,0.,-1.)))),6.);
col=mix(col,.8,pow(max(0.,length(float3(uv.x*.8-.4,uv.y,height))),2.));
return float4(col,1.);
}

ENDCG
}

}
}