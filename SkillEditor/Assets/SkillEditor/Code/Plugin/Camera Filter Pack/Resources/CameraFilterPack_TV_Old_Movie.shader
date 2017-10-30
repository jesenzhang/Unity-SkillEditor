// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_Old_Movie" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(1.0, 10.0)) = 1.0
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
uniform float _TimeX;
uniform float _Distortion;

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

float rand(float2 co){
return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
}

float rand(float c){
return rand(float2(c,1.0));
}

float randomLine(float seed, float2 uv)
{
float aa = rand(seed+1.0);
float b = 0.01 * aa;
float c = aa - 0.5;
float l;

if ( aa > 0.2)
l = pow(  abs(aa * uv.x + b * uv.y + c ), 0.125);
else
l = 2.0 - pow( abs(aa * uv.x + b * uv.y + c), 0.125 );				

return lerp(0.5, 1.0, l);
}

float randomBlotch(float seed, float2 uv)
{
float x = rand(seed);
float y = rand(seed+1.0);
float s = 0.01 * rand(seed+2.0);
float2 p = float2(x,y) - uv;
p.x *= 1;
float aa = atan(p.y/p.x);
float v = 1.0;
float ss = s*s * (sin(6.2831*aa*x)*0.1 + 1.0);
if ( dot(p,p) < ss ) v = 0.2;
else v = pow(dot(p,p) - ss, 1.0/16.0);
return lerp(0.3 + 0.2 * (1.0 - (s / 0.02)), 1.0, v);
}


float4 frag (v2f i) : COLOR
{
float2 uv;
uv  = i.texcoord;
float t = float(int(_TimeX * 15.0));
float2 suv = uv + 0.002 * float2( rand(t), rand(t + 23.0));
float3 image = tex2D( _MainTex, float2(suv.x, suv.y) );
float luma = dot( float3(0.2126, 0.7152, 0.0722), image );
float3 oldImage = luma * float3(0.7, 0.7, 0.7);
float randx=rand(t + 8.);
float vI = 16.0 * (uv.x * (1.0-uv.x) * uv.y * (1.0-uv.y));
vI *= lerp( 0.7, 1.0, randx+.5);
vI += 1.0 + 0.4 *randx;
vI *= pow(16.0 * uv.x * (1.0-uv.x) * uv.y * (1.0-uv.y), 0.4);
int l = int(8.0 * randx);
if ( 0 < l ) vI *= randomLine( t+6.0+17.* float(0),uv);
if ( 1 < l ) vI *= randomLine( t+6.0+17.* float(1),uv);
int s = int( max(8.0 * rand(t+18.0) -2.0, 0.0 ));
if ( 0 < s ) vI *= randomBlotch( t+6.0+19.* float(0),uv);
if ( 1 < s ) vI *= randomBlotch( t+6.0+19.* float(1),uv);
return float4(oldImage * vI, 1.0);

}

ENDCG
}

}
}