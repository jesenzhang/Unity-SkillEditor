// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_PlanetMars" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(1.0, 10.0)) = 1.0
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
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Distortion;
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


float3 PlanetMars(float _t)
{

float u = (0.860117757 + 1.54118254e-4*_t + 1.28641212e-7*_t*_t)
/ (1.0 + 8.42420235e-4*_t + 7.08145163e-7*_t*_t);

float v = (0.317398726 + 4.22806245e-5*_t + 4.20481691e-8*_t*_t)
/ (1.0 - 2.89741816e-5*_t + 1.61456053e-7*_t*_t);

float x = 3.0 * u / (2.0 * u - 8.0 * v + 4.0);
float y = 2.0 * v / (2.0 * u - 8.0 * v + 4.0);
float z = 1.0 - x - y;

float Y = 1.0;
float X = (Y/y) * x;
float Z = (Y/y) * z;

float3 RGB = float3(X,Y,Z); 

RGB.x = RGB.x * pow(0.0006*_t*_Distortion, 4.0)/_Distortion;
RGB.y = RGB.y * pow(0.0004*_t*_Distortion, 4.0)/_Distortion;
RGB.z = RGB.z * pow(0.0004*_t*_Distortion, 4.0)*_Distortion;

return RGB;
}


float4 frag (v2f i) : COLOR
{
float2 uv 		=  i.texcoord.xy ;

float3 noise = tex2D(_MainTex, uv).xyz;
float lum = dot(noise, float3 (0.2126, 0.7152, 0.0722));

float maxTemp = 4000.0;
float tempScale = maxTemp;
float3 c = PlanetMars(lum * tempScale);

return float4(c,1.0);


}

ENDCG
}

}
}