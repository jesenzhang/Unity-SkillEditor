// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_BrokenGlass2" {
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
uniform float _Bullet_1;
uniform float _Bullet_2;
uniform float _Bullet_3;
uniform float _Bullet_4;
uniform float _Bullet_5;
uniform float _Bullet_6;
uniform float _Bullet_7;
uniform float _Bullet_8;
uniform float _Bullet_9;
uniform float _Bullet_10;
uniform float _Bullet_11;
uniform float _Bullet_12;
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

fixed4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float2 uv2 = uv;

#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif


uv/=2;
float col = 0;

col = (tex2D(_MainTex2,uv).rgb*_Bullet_1).r;
uv+=float2(0.5,0);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_2).r;
uv+=float2(0,0.5);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_3).r;
uv-=float2(0.5,0.0);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_4).r;

col += (tex2D(_MainTex2,uv).rgb*_Bullet_5).g;
uv+=float2(0.5,0);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_6).g;
uv+=float2(0,0.5);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_7).g;
uv-=float2(0.5,0.0);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_8).g;

col += (tex2D(_MainTex2,uv).rgb*_Bullet_9).b;
uv+=float2(0.5,0);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_10).b;
uv+=float2(0,0.5);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_11).b;
uv-=float2(0.5,0.0);
col += (tex2D(_MainTex2,uv).rgb*_Bullet_12).b;

uv2+=float2(col,col)/4;

float3 col2 = tex2D(_MainTex,uv2).rgb;
float c=col;
col2=col2+float3(c,c,c);

return fixed4(col2, 1.0);
}

ENDCG
}

}
}