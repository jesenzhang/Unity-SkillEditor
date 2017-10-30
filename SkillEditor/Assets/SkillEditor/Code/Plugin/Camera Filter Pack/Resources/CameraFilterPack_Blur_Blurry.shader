// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Blur_Blurry" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Amount ("_Amount", Range(0.0, 20.0)) = 5.0
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
uniform float _Amount;

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
float stepU = (1.0 / _ScreenResolution.x) * _Amount;
float stepV = (1.0 / _ScreenResolution.y) * _Amount;

fixed3x3 gaussian = fixed3x3(
1.0,	2.0,	1.0,
2.0,	4.0,	2.0,
1.0,	2.0,	1.0);

float3 result = 0;

for(int u=0;u<3;u++) 
{
for(int j=0;j<3;j++) 
{
float2 texCoord = i.texcoord.xy + float2( float(u-1)*stepU, float(j-1)*stepV );
result += gaussian[u][j] * tex2D(_MainTex,texCoord);
}

}

return float4(result / 16.0,1.0);	
}

ENDCG
}

}
}