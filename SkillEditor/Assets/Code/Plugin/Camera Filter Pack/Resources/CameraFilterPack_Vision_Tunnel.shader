// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Vision_Tunnel" { 
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
float mod(float x, float y) 
{ return x - y * floor(x/y); }
float4 frag (v2f i) : COLOR {

float2 p = -1.0 + 2.0 * i.texcoord.xy;
float2 uv;

float r = sqrt(dot(p,p));
float a = atan2(p.y,p.x);

uv.x = .1*8 - .1 / r;
uv.y = mod(1. * a / 3.1416,_Value3); 
float3 col = tex2D(_MainTex,uv).xyz;
col *= smoothstep( 0., .5, r );
float dist2 = 1.0 - smoothstep(_Value,_Value-0.05-_Value2, length(float2(0.5,0.5) - i.texcoord.xy));
float3 result=lerp(tex2D(_MainTex,i.texcoord.xy),col,dist2);
return float4(result,1.0);

}
ENDCG
}
}
}
