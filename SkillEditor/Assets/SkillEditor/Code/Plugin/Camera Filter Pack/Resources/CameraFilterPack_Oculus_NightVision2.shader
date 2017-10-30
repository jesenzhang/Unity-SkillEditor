// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Oculus_NightVision2" { 
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
uniform float _Red_R;
uniform float _Red_G;
uniform float _Red_B;
uniform float _Green_R;
uniform float _Green_G;
uniform float _Green_B;
uniform float _Blue_R;
uniform float _Blue_G;
uniform float _Blue_B;
uniform float _Red_C;
uniform float _Green_C;
uniform float _Blue_C;
uniform float _FadeFX;

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
inline float mod(float x,float modu) 
{
return x - floor(x * (1.0 / modu)) * modu;
}    

inline float noise(float2 p)
{
float sample = tex2D(_MainTex,float2(1.,2.*cos(_TimeX))*_TimeX*8. + p*1.).x;
sample *= sample;
return sample;
}
float rand(float2 n, float time)
{
return 0.5 + 0.5 * frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453 + time);
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


float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy ;
float4 col = tex2D( _MainTex, uv );
float3 c_r = float3(_Red_R,_Red_G,_Red_B);
float3 c_g = float3(_Green_R,_Green_G,_Green_B);
float3 c_b = float3(_Blue_R,_Blue_G,_Blue_B);
float3 rgb = float3( dot(col.rgb,c_r)+_Red_C, dot(col.rgb,c_g)+_Green_C, dot(col.rgb,c_b)+_Blue_C );
float noise = rand(uv * float2(0.1, 1.0), _TimeX * 1.0);
noise = 1.0 -noise * 0.5;
rgb=lerp(col,rgb*float3(noise,noise,noise),_FadeFX);
rgb += stripes(uv);
rgb *= (12.+mod(uv.y*30.+_TimeX,1.))/13.;
rgb *= 0.5 + 6.0*uv.x*uv.y*(1.5-uv.x)*(1.5-uv.y)*2;
rgb *= 0.9+0.1*sin(10.0*_TimeX+uv.y*300.0);
rgb *= 0.79+0.01*sin(110.0*_TimeX);
return  float4( rgb, 1 );
}
ENDCG
}
}
}
