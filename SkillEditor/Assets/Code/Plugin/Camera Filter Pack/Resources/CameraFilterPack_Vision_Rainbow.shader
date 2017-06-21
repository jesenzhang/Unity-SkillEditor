// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////



Shader "CameraFilterPack/Vision_Rainbow" { 
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
uniform float _Value5;
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
float3 hsv2rgb_smooth( in float3 c )
{
float3 rgb = clamp( abs(fmod(c.x*6.0+float3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
rgb = rgb*rgb*(3.0-2.0*rgb);	
return c.z * lerp( float3(1.0,1.0,1.0), rgb, c.y);
}

float3 pal( in float t, in float3 a, in float3 b, in float3 c, in float3 d )
{
return a + b*cos( 6.28318*(c*t+d) );
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy ;
float val = _Value4;
float color = (_TimeX*_Value+20.)+val*atan2((uv.y-_Value3), (uv.x-_Value2));
float3 draw = hsv2rgb_smooth(float3(color, 1.0, 1.0));
float4 src=tex2D(_MainTex,uv);
float dist2 = 1.0 - smoothstep(_Value5,_Value5-0.35, length(float2(_Value2, _Value3) - uv));
draw.rgb=lerp(src.rgb,draw,dist2);

return  float4(draw, 1.0);
}
ENDCG
}
}
}
