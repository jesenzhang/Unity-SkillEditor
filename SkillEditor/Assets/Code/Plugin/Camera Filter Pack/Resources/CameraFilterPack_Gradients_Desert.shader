// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Gradients_Desert" { 
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
float square(float s) { return s * s; }
float3 square(float3 s) { return s * s; }

float3 desertGradient(float t) 
{
float s = sqrt(clamp(1.0 - (t - 0.4) / 0.6, 0.0, 1.0));
float3 sky = sqrt(lerp(float3(1, 1, 1), float3(0, 0.8, 1.0), smoothstep(0.4, 0.9, t)) * float3(s, s, 1.0));
float3 land = lerp(float3(0.7, 0.3, 0.0), float3(0.85, 0.75 + max(0.8 - t * 20.0, 0.0), 0.5), square(t / 0.4));
return clamp((t > 0.4) ? sky : land, 0.0, 1.0) * clamp(1.5 * (1.0 - abs(t - 0.4)), 0.0, 1.0);
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float4 tc = tex2D(_MainTex,uv);    
float b = (0.2126*tc.r + 0.7152*tc.g + 0.0722*tc.b);
b=lerp(b,1-b,_Value);
float3 map=lerp(tc,desertGradient(b),_Value2);
tc=float4(map,1.0);
return  tc;
}
ENDCG
}
}
}
