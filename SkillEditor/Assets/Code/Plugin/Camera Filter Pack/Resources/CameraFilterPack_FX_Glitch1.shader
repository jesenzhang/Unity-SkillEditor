// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/FX_Glitch1" { 
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

float sat( float t ) { return clamp( t, 0.0, 1.0 ); }
float2 sat( float2 t ) { return clamp( t, 0.0, 1.0 ); }
float remap( float t, float a, float b ) { return sat( (t - a) / (b - a) ); }
float linterp( float t ) { return sat( 1.0 - abs( 2.0*t - 1.0 ) ); }
float rand( float2 n ) { return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453); }
float srand( float2 n ) { return rand(n) * 2.0 - 1.0; }

float trunc( float x, float num_levels ) { return floor(x*num_levels) / num_levels; }
float2 trunc( float2 x, float2 num_levels ) { return floor(x*num_levels) / num_levels; }

float3 rgb2yuv( float3 rgb )
{
float3 yuv;
yuv.x = dot( rgb, float3(0.299,0.587,0.114) );
yuv.y = dot( rgb, float3(-0.14713, -0.28886, 0.436) );
yuv.z = dot( rgb, float3(0.615, -0.51499, -0.10001) );
return yuv;
}

float3 yuv2rgb( float3 yuv )
{
float3 rgb;
rgb.r = yuv.x + yuv.z * 1.13983;
rgb.g = yuv.x + dot( float2(-0.39465, -0.58060), yuv.yz );
rgb.b = yuv.x + yuv.y * 2.03211;
return rgb;
}

float4 frag (v2f i) : COLOR
{
float _TimeX_s = _TimeX;
float2 uv = i.texcoord.xy;
float ct = trunc( _TimeX_s, 4.0 );
float change_rnd = rand( trunc(uv.yy,float2(16,16)) + 150.0 * ct);
float tf = 16.0*change_rnd;
float t = 5.0 * trunc( _TimeX_s, tf);
float vt_rnd = 0.5 * rand( trunc(uv.yy + t, float2(11,11)));
vt_rnd += 0.5 * rand(trunc(uv.yy + t, float2(7,7)));
vt_rnd = vt_rnd * 2.0 - 1.0;
vt_rnd = sign(vt_rnd) * sat( ( abs(vt_rnd) -0.6) / (0.4) );
float2 uv_nm = uv;
uv_nm = sat( uv_nm + float2(0.1*vt_rnd, 0));
float rn= trunc( _TimeX_s, 8.0 );
float rnd = rand( float2(rn,rn));
uv_nm.y = (rnd>lerp(1.0, 0.975, sat(0.4))) ? 1.0-uv_nm.y : uv_nm.y;
float4 sample = tex2D( _MainTex, uv_nm );
float3 sample_yuv = rgb2yuv( sample.rgb);
sample_yuv.y /= 1.0-3.0 * abs(vt_rnd) * sat( 0.5 - vt_rnd);
sample_yuv.z += 0.125 * vt_rnd * sat( vt_rnd - 0.5);
return  float4( yuv2rgb(sample_yuv), sample.a);
}
ENDCG
}
}
}