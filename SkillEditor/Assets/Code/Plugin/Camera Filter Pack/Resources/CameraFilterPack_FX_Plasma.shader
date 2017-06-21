// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/FX_Plasma" { 
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
float v_TimeX1 = 1000.0 + sin(_TimeX * 0.11) * 20.0;
float v_TimeX2 = 800.0  + sin(_TimeX * 0.15) * 22.0;

float2 uv = i.texcoord.xy;

float4 t = tex2D(_MainTex, 
float2(
sin(uv.x + v_TimeX1 * 0.005) * cos(v_TimeX1 * 0.01), 
cos(uv.y + v_TimeX1 * 0.001) * cos(v_TimeX1 * 0.02)
) * 5.0 
);

float4 t2 = tex2D(_MainTex, 
float2(
sin(uv.x + v_TimeX1 * 0.001), 
cos(uv.y + v_TimeX1 * 0.005)
) * 1.0 
);
float4 origine=tex2D(_MainTex,uv);
float4 col= float4(
0.0
,
(
t.r *
(sin(
v_TimeX1 * 
(
sin(uv.y * 0.5) + 
0.01 * sin(uv.x * 5.0 + v_TimeX2)
)
)) *
sin(
v_TimeX1 * 0.1 * t2.r *
(uv.x - sin(v_TimeX2 * 0.05)) 
* 
sin( (uv.y-sin(v_TimeX1 * 0.035)) * 5.0 ) + 
sin(0.1 * v_TimeX2)
) * 0.5 

+ t2.r * abs(sin(
v_TimeX1 * (uv.x-0.5) * sin(uv.y+0.5))
) * 0.5

+ t.r * 
(
sin( 
v_TimeX1 * (uv.x - sin(v_TimeX1 * 0.1)) * 
sin( uv.y - sin(v_TimeX1 * 0.1)) * 0.2 
)
) * 0.1
)
,

(
t.r *
(sin(
v_TimeX2 * 
(
sin(uv.y * 0.25) + 
0.01 * sin(uv.x * 3.0 + v_TimeX2)
)
)) *
abs(sin(
v_TimeX1 * 0.09 * t2.r *
(uv.x - sin(v_TimeX2 * 0.04)) 
* 
sin( (uv.y-sin(v_TimeX1 * 0.035)) * 5.0 ) + 
sin(0.1 * v_TimeX2)
)) * 0.5 

+ t2.r * abs(sin(
v_TimeX1 * (uv.x-0.5) * sin(uv.y+0.5))
) * 0.5

+ t.r * abs(sin( v_TimeX1 * (uv.x-sin(v_TimeX1 * 0.1)) * sin(uv.y-sin(v_TimeX1 * 0.1)) * 0.2 )) * 0.1
)
,
1.0
);
col=col+origine;
return col;
}
ENDCG
}
}
}
