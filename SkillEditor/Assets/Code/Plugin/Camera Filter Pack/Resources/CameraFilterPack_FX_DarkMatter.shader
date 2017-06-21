// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/FX_DarkMatter" { 
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
uniform float _Value5;
uniform float _Value6;

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
float4 r(float t,float c,float2 uv)
{
float2 pos=(uv*0.5)*2.0;
float2 p=float2(sin(t*1.0)*cos(t*1.52),cos(t*1.1)*sin(t*1.52));
float cv=pow(0.77,10.0*distance(pos,p));
return float4(cv,cv,cv,cv)*sin(c);
}

float4 frag (v2f i) : COLOR
{
float4 color=float4(1.0,1.0,1.0,1.0);
float2 uv=(i.texcoord-float2(_Value3,_Value4))/_Value5;
float time=_TimeX*_Value;
for (float c=0.0; c<10.0; c++) color*=(1.+r(time+c,c*0.25,uv))*0.85;
float cr=color.r*0.3;
cr*=_Value2;
float cruv=lerp(cr,cr-0.06,_Value2);
float4 txt=tex2D(_MainTex,i.texcoord+float2(cruv,cruv));
txt-=cr*_Value6;
return txt;
}
ENDCG
}
}
}
