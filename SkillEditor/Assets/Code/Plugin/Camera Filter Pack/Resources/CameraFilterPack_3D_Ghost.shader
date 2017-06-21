// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/3D_Ghost" { 
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
uniform sampler2D Texture2;
uniform sampler2D _CameraDepthTexture;
uniform float _TimeX;
uniform float _Value2;


uniform float GhostPosX;
uniform float GhostPosY;
uniform float GhostFade;
uniform float GhostFade2; 
uniform float GhostSize;

uniform float Drop_Near;
uniform float Drop_Far;
uniform float Drop_With_Obj;

uniform float _FixDistance;


uniform float4 _ScreenResolution;
uniform float2 _MainTex_TexelSize;
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
float4 projPos : TEXCOORD1; 
};

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
OUT.color = IN.color;
OUT.projPos = ComputeScreenPos(OUT.vertex);
return OUT;
}



float4 ghost(float2 uv)
{
return tex2D(_MainTex, uv);
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif

float4 txt = tex2D(_MainTex, uv);
float4 s = 0;

s= ghost(uv);
uv /= GhostSize;
GhostPosX += (1 - GhostSize) * 0.5;
GhostPosY += (1 - GhostSize) * 0.5;
uv -= float2(GhostPosX, GhostPosY);
s+= ghost(uv);
uv /= GhostSize;
uv -= float2(GhostPosX, GhostPosY);
s+= ghost(uv);
uv /= GhostSize;
uv -= float2(GhostPosX, GhostPosY);
s+= ghost(uv);
uv /= GhostSize;
uv -= float2(GhostPosX, GhostPosY);
s+= ghost(uv);
s /= GhostFade2;
s -= GhostFade;

float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
depth /= _FixDistance * 10;
depth = saturate(depth);
depth = smoothstep(Drop_Near, Drop_Far, depth);
txt= lerp(txt, txt+s*_Value2, depth *Drop_With_Obj);

return  txt;
}
ENDCG
}
}
}
