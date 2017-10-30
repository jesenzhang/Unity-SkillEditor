// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////



Shader "CameraFilterPack/Vision_Warp2" { 
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

float sinc(float r, float width)
{
width *= 10.0;
float N = 1.1;
float numer = sin(r / width);
float denom = (r /width);
if(abs(denom) <= 0.1) return 1.0;
else return abs(numer / denom);
} 

float expo(float r, float dev)
{
return 1.0 * exp(- r*r / dev);
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float2 cdiff = abs(uv - 0.5);
float myradius = length(cdiff);
float radius = _TimeX/3.0; 
float r = sin((myradius - radius) * 5.0);
r = r*r;  
float s=sinc(r, 0.001);
float4 fColor = float4(s,s,s, 1.0);
float dist2 = 1.0 - smoothstep(_Value,_Value-0.05-_Value2, length(float2(0.5,0.5) - uv));
//fColor.rgb=lerp(tex2D(_MainTex,uv),fColor.rgb,dist2).rgb;
fColor.rgb=lerp(float3(0,0,0),fColor.rgb,dist2).rgb;
float3 fc=tex2D(_MainTex,uv-float2(fColor.r*_Value3,fColor.r*_Value3)).rgb;

return float4(fc.rgb,1.0);
}
ENDCG
}
}
}
