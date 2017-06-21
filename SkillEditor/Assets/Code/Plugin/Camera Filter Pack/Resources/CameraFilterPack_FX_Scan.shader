// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/FX_Scan" { 
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
float2 uv = i.texcoord.xy ;

float4 frg=float4(0,0,0,1);

float r = tex2D(_MainTex, uv).r;
float g = (uv.x -0.4- fmod(sin(_TimeX*_Value2)/1.5,1.0))*4.0;
if(g-_Value>r)
frg=  tex2D(_MainTex, uv);    
else if(g+_Value>r)
{
frg=  tex2D(_MainTex, float2(uv.x+sin(_TimeX*_Value2*9.0),uv.y+_TimeX*5.0));
frg.r+=1;
frg.g-=1;
frg.b-=1;

}
else        
frg=  tex2D(_MainTex, uv);

return frg;
}
ENDCG
}
}
}
