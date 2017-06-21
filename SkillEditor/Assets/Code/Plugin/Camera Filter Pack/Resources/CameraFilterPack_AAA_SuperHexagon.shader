// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/AAA_Super_Hexagon" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Value ("_Value", Range(0.2, 10.0)) = 1
_HexaColor ("_HexaColor", Color) = (1,1,1,1)
_BorderSize ("_BorderSize", Range(-0.5, 0.5)) = 0.0
_BorderColor ("_BorderColor", Color) = (1,1,1,1)
_SpotSize ("_SpotSize", Range(0, 1.)) = 0.5
_AlphaHexa ("_AlphaHexa", Range(0.2, 10.0)) = 1
_PositionX ("_PositionX", Range(-0.5, 0.5)) = 0.0
_PositionY ("_PositionY", Range(-0.5, 0.5)) = 0.0
_Radius ("_Radius", Range(0, 1.)) = 0.5
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
uniform float _Value;
uniform float _BorderSize;
uniform float4 _BorderColor;
uniform float4 _HexaColor;

uniform float _AlphaHexa;

uniform float _PositionX;
uniform float _PositionY;
uniform float _Radius;			
uniform float _SpotSize;			

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


float hexDist(float2 a, float2 b){
float2 p = abs(b-a);
float s = 0.5;
float c = 0.8660254;

float diagDist = s*p.x + c*p.y;
return max(diagDist, p.x)/c;
}

float2 nearestHex(float s, float2 st){
float h = 0.5*s;
float r = 0.8660254*s;
float b = s + 2.0*h;
float a = 2.0*r;
float m = h/r;

float2 sect = st/float2(2.0*r, h+s);
float2 sectPxl = fmod(st, float2(2.0*r, h+s));

float aSection = fmod(floor(sect.y), 2.0);

float2 coord = floor(sect);
if(aSection > 0.0){
if(sectPxl.y < (h-sectPxl.x*m)){
coord -= 1.0;
}
else if(sectPxl.y < (-h + sectPxl.x*m)){
coord.y -= 1.0;
}

}
else{
if(sectPxl.x > r){
if(sectPxl.y < (2.0*h - sectPxl.x * m)){
coord.y -= 1.0;
}
}
else{
if(sectPxl.y < (sectPxl.x*m)){
coord.y -= 1.0;
}
else{
coord.x -= 1.0;
}
}
}

float xoff = fmod(coord.y, 2.0)*r;
return float2(coord.x*2.0*r-xoff, coord.y*(h+s))+float2(r*2.0, s);
}



inline float2 screenDistort(float2 uv)
{
uv -= float2(.5,.5);
uv = uv*1.2*(1.0/1.2+2.*uv.x*uv.x*uv.y*uv.y);
uv += float2(.5,.5);
return uv;
}

float noise2(float n)
{
return lerp(0,0.5,smoothstep(0.0,0.5,+fmod(n+0.5,1.0)));
}


float4 frag (v2f i) : COLOR
{
float2 uv 		= i.texcoord.xy;
float   s 		= _Value * _ScreenResolution.x/160.0;
float2 nearest 	= nearestHex(s, i.texcoord.xy * _ScreenResolution.xy);
float4 texel 	= tex2D(_MainTex, nearest/_ScreenResolution.xy);
float4 texel2 	= tex2D(_MainTex, uv);
uv = screenDistort(uv);
float2 uv2;
uv2 = uv.yy+float2(0.1*sin(_TimeX/3)*5,0.1*sin(_TimeX/5)*5);
float3 video = tex2D(_MainTex,uv2).rgb;
float vigAmt = 2.+.3*sin(15+5.*cos(5.));
float vignette = (1.-vigAmt*(uv.y-.5)*(uv.y-.5))*(1.-vigAmt*(uv.x-.5)*(uv.x-.5));
video += (12.+fmod(uv.y*10.+_TimeX,1.))/13.;
float2 center = float2(_PositionX,_PositionY);
float dist2 = 1.0 - smoothstep( _Radius,_Radius+0.15*_SpotSize, length(center - uv));
float dist 		= hexDist(i.texcoord.xy * _ScreenResolution.xy, nearest);
float luminance = (texel.r + texel.g + texel.b)/3.0;
float interiorSize = s*_BorderSize;//*(1-dist2*2);
float interior = 1.0 - smoothstep(interiorSize-1.0, interiorSize, dist);
float4 result;
result.rgb=lerp(_BorderColor.rgb,texel.rgb,interior);
float mem=1-noise2(_TimeX/4+uv.x);
result.rgb+=float3(mem,mem,mem);
result.rgb*=_HexaColor.rgb;
result.rgb/=video.rgb/2;
result /= vignette*2;
result.rgb=lerp(result.rgb,texel2.rgb,dist2);
result.rgb=lerp(result.rgb,texel2.rgb,1-_AlphaHexa);
result.a=1.0;
return float4(result);	
}

ENDCG
}

}
}