// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Vision_Crystal" { 
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


float4 frag (v2f i) : COLOR{

float2 rv=i.texcoord.xy+float2(_Value2,_Value3);
float3 c=float3(0,0,0);
float z=_Time*20;
float l=z;
float2 uv=rv;
float2 p=uv;
p-=.5;
z+=.07;
l=length(p);
uv+=p/l*(sin(z)+_Value)*abs(sin(l*9.-z*2.));
c.r=.01/length(abs(fmod(uv,1.)-.5));
uv=rv;
p=uv;
p-=.5;
z+=.07;
l=length(p);
uv+=p/l*(sin(z)+_Value)*abs(sin(l*9.-z*2.));
c.g=.01/length(abs(fmod(uv,1.)-.5));
uv=rv;
p=uv;
p-=.5;
z+=.07;
l=length(p);
uv+=p/l*(sin(z)+_Value)*abs(sin(l*9.-z*2.));
c.b=.01/length(abs(fmod(uv,1.)-.5));

c+=tex2D(_MainTex,i.texcoord.xy);
return float4(c/l,1.0);
}
ENDCG
}
}
}
