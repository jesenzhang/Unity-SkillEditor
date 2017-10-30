// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/AAA_Blood" {
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
uniform float _Value4;
uniform float _Value5;
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


uv =(i.texcoord-float2(0.5,0.5))*0.8;
float total=_Value2+_Value3+_Value4+_Value5;

uv=uv+float2(0.5,0.5);
uv/=2+total/1000;
uv+=float2(0,0);

float3 col = (tex2D(_MainTex2,uv).rgb*_Value4);
uv+=float2(0.5,0);

col += (tex2D(_MainTex2,uv).rgb*_Value3);
uv+=float2(0,0.5);
col += (tex2D(_MainTex2,uv).rgb*_Value5);
uv-=float2(0.5,0.0);
col += (tex2D(_MainTex2,uv).rgb*_Value2);


uv2+=float2(col.r,col.r)/512;
float3 col2 = tex2D(_MainTex,uv2).rgb;
col2=col2+(col*_Value);

col2.r=col2.r*(1+col);

return fixed4(col2, 1.0);
}

ENDCG
}

}
}