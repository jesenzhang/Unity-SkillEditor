// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Oculus_NightVision3" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_BinocularSize ("_BinocularSize", Range(0.0, 1.0)) = 0.5
_BinocularDistance ("_BinocularDistance", Range(0.0, 1.0)) = 0.5
_Greenness ("_Greenness", Range(0.0, 1.0)) = 0.4
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
uniform float _Greenness;

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



inline float mod(float x,float modu) {
return x - floor(x * (1.0 / modu)) * modu;
}    

inline float noise(float2 p)
{
float sample = tex2D(_MainTex,float2(1.,2.*cos(_TimeX))*_TimeX*8. + p*1.).x;
sample *= sample;
return sample;
}


inline float onOff(float a, float b, float c)
{
return step(c, sin(_TimeX + a*cos(_TimeX*b)));
}

inline float ramp(float y, float start, float end)
{
float inside = step(start,y) - step(end,y);
float fact = (y-start)/(end-start)*inside;
return (1.-fact) * inside;

}

inline float stripes(float2 uv)
{

float noi = noise(uv*float2(0.5,1.) + float2(1.,3.));
return ramp(mod(uv.y*2. + _TimeX/4.+sin(_TimeX + sin(_TimeX*0.23)),1.),0.4,0.6)*noi;
}

inline float3 getVideo(float2 uv)
{
float2 look = uv;
look.x = look.x + sin(look.y*20. + _TimeX)/250.*onOff(2.,2.,0.9)*(1.+cos(_TimeX*80.));

float3 video = tex2D(_MainTex,look);
return video;
}

inline float2 screenDistort(float2 uv)
{
uv -= float2(.5,.5);
uv = uv*1.2*(1./1.2+2.*uv.x*uv.x*uv.y*uv.y);
uv += float2(.5,.5);
return uv;
}

float4 frag (v2f i) : COLOR
{
float2 uv = 0.5 + (i.texcoord-0.5);
float3 col;
col = getVideo(uv);
col.rgb = col.rrr;
col += stripes(uv);


col *= (12.+mod(uv.y*30.+_TimeX,1.))/13.;
col *= 0.5 + 6.0*uv.x*uv.y*(1.5-uv.x)*(1.5-uv.y)*_Greenness;
col *= float3(0.55,1.55+_Greenness/4,0.55)*_Greenness;
col *= 0.9+0.1*sin(10.0*_TimeX+uv.y*300.0);
col *= 0.79+0.01*sin(110.0*_TimeX);
return float4(col,1.0);

}

ENDCG
}

}
}