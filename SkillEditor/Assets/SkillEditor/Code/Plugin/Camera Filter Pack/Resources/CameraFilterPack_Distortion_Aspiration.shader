// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Distortion_Aspiration" { 
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

float4 colors(float2 uv)
{
//uv=fmod(uv,1);

float b=0;

if (uv.x>1) { uv.x=1-(uv.x-1); b=uv.x-1; }
if (uv.x<0) { uv.x=1-(uv.x+1); b+=1-(uv.x+1); }
if (uv.y>1) { uv.y=1-(uv.y-1); b+=uv.y-1; }
if (uv.y<0) { uv.y=1-(uv.y+1); b+=1-(uv.y+1); }

float4 c=float4(tex2D( _MainTex, uv ).xyz, 1.0);
b=abs(b*2);
c.rgb-=float3(b,b,b);
return c;
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy ;
float2 target = float2(_Value2,_Value3);
float2 pos = uv + normalize(uv - target) * _Value;

return  colors(pos);
}
ENDCG
}
}
}
