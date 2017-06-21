// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Blur_Dithering2x2" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)

_Level ("_Level", Range(1.0, 16.0)) = 4.0
_Distance ("_Distance", Vector) = (30.,0.,0.,0.)
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
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float _Level;
uniform float4 _Distance;
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

#define tex2D(sampler,uvs)  tex2Dlod( sampler , float4( ( uvs ) , 0.0f , 0.0f) )

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;

float2 seed = uv+frac(_TimeX);
float nrnd = nrand(seed);
float srnd = nrnd;

int num_samples = int(_Level);
const float num_samples_f = float(num_samples);

float2 dist = _Distance / _ScreenResolution;
float2 p0 = uv - 0.5*dist;
float2 p1 = uv + 0.5*dist;
float2 stepfloat = (p1-p0)/(num_samples_f-1.0);
float2 ij = floor(fmod( uv * _ScreenResolution.xy, 2.0));
float idx = ij.x + ij.y;
float4 m = step( abs(idx-float4(0,1.,2.,3.)), 0.5) * float4( 0.75, 0.25, 0, 0.5 );
float d = m.x+m.y+m.z+m.w;
float2 p = p0 + d * stepfloat;
float4 sum = tex2D( _MainTex, p);
for(int i=1;i<num_samples;++i)
{
p+=stepfloat;
sum += tex2D( _MainTex, p);
}
sum /= num_samples_f;

return sum;
}

ENDCG
}

}
}