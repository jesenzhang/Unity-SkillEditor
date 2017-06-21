// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Blur_Bloom" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Amount ("_Amount", Range(0.0, 20.0)) = 5.0
_Glow ("_Glow", Range(0.0, 20.0)) = 5.0
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
uniform float _Glow;

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
float4 frag (v2f i) : COLOR
{
float stepU = (1.0 / _ScreenResolution.x) * _Amount;
float stepV = (1.0 / _ScreenResolution.y) * _Amount;
fixed3 color = tex2D(_MainTex,i.texcoord.xy );

fixed3x3 gaussian = fixed3x3(
1.0,	2.0,	1.0,
2.0,	4.0,	2.0,
1.0,	2.0,	1.0);

float4 result = 0;

for(int u=0;u<3;u++) 
{
for(int j=0;j<3;j++) 
{
float2 texCoord = i.texcoord.xy + float2( float(u-1)*stepU, float(j-1)*stepV );
result += gaussian[u][j] * tex2D(_MainTex,texCoord);
}

}

result /=8;

result.rgb = lerp (color.rgb,result.rgb, _Glow);


return result;	
}

ENDCG
}

}
}