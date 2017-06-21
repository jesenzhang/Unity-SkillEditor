// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/TV_VHS_Rewind" { 
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
float remap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
{
return (value - inputMin) * ((outputMax - outputMin) / (inputMax - inputMin)) + outputMin;
}

float f1(float x)
{
return -4.0 * pow(x - 0.5, 1.0) + 2.0;
}

fixed4 frag (v2f i) : COLOR
{
float2 q = i.texcoord.xy;
float2 uv = 0.5 + (q-0.5)*(0.9 + 0.1*sin(0.2*_TimeX));
float Effect = abs(uv.x - 0.5) * _Value;
float3 aberration = float3(0.019, 0, -0.019);
aberration *= Effect;
float3 col;
uv = q; 
float warpLine = frac(-_TimeX * _Value2);
float warpLine2 = frac(-_TimeX * _Value3); 
float wide = _ScreenResolution.x / _ScreenResolution.y;
float2 position = float2(uv.x * wide, uv.y*_Value);
float warpArg = remap(clamp((position.y - warpLine) - 0.05, 0.0, 0.1), 0.0, 0.1, 0.0, 1.0);
float warpArg2 = remap(clamp((position.y - warpLine2) - 0.05, 0.0, 0.1), 0.0, 0.1, 0.0, 1.0);
float offset = sin(warpArg * 10.0)  * f1(warpArg);
float offset2 = sin(warpArg2 * 10.0 * _Value3)  * f1(warpArg);
uv= uv + float2(offset * 0.02, 0.0); 

float3 text = float3(1.0,1.0,1.0);
text.r = tex2D(_MainTex,float2(uv.x+aberration.x,uv.y)).x;
text.g = tex2D(_MainTex,float2(uv.x+aberration.y,uv.y)).y;
text.b = tex2D(_MainTex,float2(uv.x+aberration.z,uv.y)).z;

text.rgb=lerp(text.rgb,float3(text.r,text.r,text.r),warpArg2*(1.0-uv.y));
text.rgb=lerp(text.rgb,float3(text.r,text.r,text.r),warpArg*(1.0-uv.y));


return fixed4(text.rgb,1.0);
}
ENDCG
}
}
}
