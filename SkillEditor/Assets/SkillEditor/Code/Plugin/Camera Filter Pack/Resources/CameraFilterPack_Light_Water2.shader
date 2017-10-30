// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Light_Water2" { 
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
uniform float _Value4;
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
float col(float2 coord)
{
float time = _TimeX*1.3;
float delta_theta = 0.897597901025655210989326680937;
float col = 0.0;
float theta = 0.0;
for (int i = 0; i < 8; i++)
{
float2 adjc = coord;
theta = delta_theta*float(i);
adjc.x += cos(theta)*time*_Value + time * _Value2;
adjc.y -= sin(theta)*time*_Value - time * _Value3;
col = col + cos( (adjc.x*cos(theta) - adjc.y*sin(theta))*6.0)*_Value4;
}
return cos(col);
}

float4 frag (v2f i) : COLOR
{
float2 p = i.texcoord.xy, c1 = p, c2 = p;
float cc1 = col(c1);
c2.x += 8.53;
float dx =  0.50*(cc1-col(c2))/60;
c2.x = p.x;
c2.y += 8.53;
float dy =  0.50*(cc1-col(c2))/60;
c1.x += dx*2.;
c1.y = (c1.y+dy*2.);
float alpha = 1.+dot(dx,dy)*700;
float ddx = dx - 0.012;
float ddy = dy - 0.012;
if (ddx > 0. && ddy > 0.) alpha = pow(alpha, ddx*ddy*200000);
float4 col = tex2D(_MainTex,c1)*(alpha);
return  col;
}
ENDCG
}
}
}
