// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Vision_Blood_Fast" { 
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
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
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
float4 frag (v2f i) : COLOR
{
float k=0.;
float2 uv=i.texcoord.xy;
float3 d =  float3(uv,1.0)-.5;
float3 o = d;
float3 c= float3(0,0,0);
float3 p= float3(0,0,0);

d += float3(tex2D(_MainTex, float2(0.1, 0.5)).rgb) * 0.01;
float xx=sin(_TimeX/2)/2;
for( int i=0; i<3; i++ )
{
p = o+xx;
for (int j = 0; j < 7; j++) { p = abs(p.zyx-0.4) -.9; k += exp(-1.25 * abs(dot(o,p)));}

k/=3.;
o += d *k;
c = c + .1*k*float3(k*k*k*3.173,k*k*-0.041,-0.01);
}
float dist2 = 1.0 - smoothstep(_Value,_Value-0.05-_Value2, length(float2(0.5,0.5) - uv));
c =  .6 *log(1.+c);
c-=0.5;
float2 cc= float2(c.r/2*uv.x,c.r/2*uv.y);
c +=tex2D(_MainTex, cc);
c=lerp(tex2D(_MainTex,uv),c,dist2);
return float4(c,1.0);
}
ENDCG
}
}
}
