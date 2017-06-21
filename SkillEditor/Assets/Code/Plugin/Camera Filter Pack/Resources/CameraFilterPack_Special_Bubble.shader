// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Special_Bubble" { 
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


float Blob(float x,float y,float Bub,float fy,float Bud,float BudS,float t,float offset){

float xx=x+sin(t*Bub*3.0)*BudS;
float yy=y+cos(t*Bub)*Bud;
float overTime = 1.0/(sqrt(xx*xx+yy*yy));

return overTime*sin(t*Bub);
}

float4 frag (v2f i) : COLOR 
{
float time=_TimeX;
float2 p=(i.texcoord.xy/1.0)*2.0-float2(1.0,1.0);
p=p*2.0;
float x=p.x-_Value;
float y=p.y-_Value2;
float a=Blob(x,y,3.0,2.9,0.8,0.15,time,0.1);
a=a+Blob(x,y,1.9,2.0,0.8, 0.2,time,0.4);
a=a+Blob(x,y,0.6,0.9,0.4,0.17,time,0.7);
a=a+Blob(x,y,1.3,2.1,0.6,0.14,time,2.3);
a=a+Blob(x,y,1.8,1.7,0.5,0.14,time,2.8); 
float3 d=float3(a,a-y*32.0,a-y*50.0)/32.0;
float2 v=i.texcoord.xy*0.8;
float b=d.r*_Value3;
float3 r=tex2D(_MainTex,v-float2(b,b));
return  float4(r,1.0);
}
ENDCG
}
}
}
