// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_ARCADE_Fast" {
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
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float2 _MainTex_TexelSize;
uniform sampler2D _MainTex2;

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



fixed4 frag (v2f i) : COLOR
{
float2 q = i.texcoord.xy;
float2 uv = q;
uv.y   *= 1.0 + pow((abs(uv.x) * 0.25), 2.0);
float4 t2 = tex2D(_MainTex2,q);

q.y=t2.g;
float a=fmod(_TimeX*_Value2,1.5);
float c= smoothstep(q.y,q.y+0.02,a);
c += smoothstep(q.y,q.y-0.02,a);
c += smoothstep(q.y+0.04,q.y+0.06,a);
c += smoothstep(q.y+0.04,q.y+0.02,a);
c*=0.5;
uv.x+=(1-c)*_Value;

float4 text = tex2D(_MainTex, uv);
float3 col = text.xyz + 0.05;

text = tex2D( _MainTex, 0.75 * float2( 0.025, -0.02) + uv);
col.rgb += text.rgb * float3( 0.18, 0.15, 0.18);

col *= t2.b * _Value3;
col *= float3( 3.46, 3.94, 3.46);

float scans = 0.35 * sin( _TimeX *2+ uv.y * _ScreenResolution.y * 1.8);
col = col * (0.4 + 0.5 * scans) ;



return fixed4(col,1.0);
}

ENDCG
}

}
}