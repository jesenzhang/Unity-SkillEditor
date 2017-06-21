// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_Vcr" {
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
return ramp(mod(uv.y*4. + _TimeX/2.+sin(_TimeX + sin(_TimeX*0.63)),1.),0.5,0.6)*noi;
}

inline float3 getVideo(float2 uv)
{
float2 look = uv;
float window = 1./(1.+20.*(look.y-mod(_TimeX/4.,1.))*(look.y-mod(_TimeX/4.,1.)));
look.x = look.x + sin(look.y*10. + _TimeX)/50.*onOff(4.,4.,.3)*(1.+cos(_TimeX*80.))*window;
float vShift = 0.4*onOff(2.,3.,.9)*(sin(_TimeX)*sin(_TimeX*20.) + 
(0.5 + 0.1*sin(_TimeX*200.)*cos(_TimeX)));
look.y = mod(look.y + vShift, 1.);
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
float2 uv = i.texcoord.xy;
uv = screenDistort(uv);
float3 video = getVideo(uv);
float vigAmt = 3.+.3*sin(_TimeX + 5.*cos(_TimeX*5.));
float vignette = (1.-vigAmt*(uv.y-.5)*(uv.y-.5))*(1.-vigAmt*(uv.x-.5)*(uv.x-.5));

video += stripes(uv);
video += noise(uv*2.)/2.;
video *= vignette;
video *= (12.+mod(uv.y*30.+_TimeX,1.))/13.;

return float4(video,1.0);


}

ENDCG
}

}
}