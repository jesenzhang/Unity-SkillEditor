// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/OldFilm_Cutting2" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Base (RGB)", 2D) = "white" {}
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
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform sampler2D _MainTex2;
uniform float _TimeX;
uniform float _Speed;
uniform float _Value;
uniform float _Value2;
uniform float _Value3;
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
};   

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;

return OUT;
}

float nrand(float2 n) {

return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
}
float3 linearLight( float3 s, float3 d )
{
return 2.0 * s + d - 1.0*_Value;
}
fixed4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float t = float(int(_TimeX * 15.0));
float2 suv = uv + 0.004* float2( nrand(t)*-2, nrand(t + 23.0));
float3 col = tex2D(_MainTex,suv).rgb;
col=lerp(col,1-col,_Value3);
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv = 1-uv;
#endif
suv = uv + 0.010 * float2( nrand(t), nrand(t + 23.0));
uv.y=suv.y;
uv.x+=_TimeX*_Speed;
float3 oldfilm = tex2D(_MainTex2,uv).rgb;
uv = i.texcoord.xy;
col*=pow(16.0 * uv.x * (1.0-uv.x) * uv.y * (1.0-uv.y), 0.4)*1+_Value2;
col = dot( float3(0.2126, 0.7152, 0.0722), col);
col=linearLight(oldfilm,col);


return fixed4(col, 1.0);
}

ENDCG
}

}
}