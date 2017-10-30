// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/NightVisionFX" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
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
uniform float _OnOff;
uniform float _Vignette;
uniform float _Vignette_Alpha;
uniform float4 _ScreenResolution;
uniform float _Greenness;
uniform float _Distortion;
uniform float _Noise;
uniform float _Intensity;
uniform float _Light;
uniform float _Light2;
uniform float _Line;
uniform float _Color_R;
uniform float _Color_G;
uniform float _Color_B;
uniform float _Size;
uniform float _Dist;
uniform float _Smooth;

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

inline float noise(float2 p)
{
float sample = tex2D(_MainTex,float2(1.,2.*cos(_TimeX))*_TimeX*8. + p*1.).x;
sample *= sample;
return sample;
}

inline float rand(float2 co){
return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
}

inline float2 screenDistort(float2 uv)
{
uv -= float2(.5,.5);
uv = uv*1.2*(0.83+2.*uv.x*uv.x*uv.y*uv.y);
uv += float2(.5,.5);
return uv;
}

float4 frag (v2f i) : COLOR
{

float2 uv = i.texcoord;
float3 col=float3(0,0,0);
uv = lerp(uv,screenDistort(uv),_Distortion);
col = tex2D(_MainTex,uv).rgb;
col.rgb+=lerp(0.0,rand(uv*floor(_TimeX*8))/2,_Noise);
float vignette=smoothstep( 0.15+_Vignette,0.5+_Vignette, length(float2(0.5,0.5)-i.texcoord));
col -=(vignette*_Vignette_Alpha);
float vignette2=smoothstep( 0.25+_Vignette,0.0+_Vignette, length(float2(0.5,0.5)-i.texcoord));
col +=(vignette2*_Light2);
col +=_Intensity;
float3 colm=col;
col *= float3(0.55,1.55+_Greenness/4,0.55);
col *= lerp(1.0,0.8+0.1*sin(10.0*_TimeX+uv.y*300.0*_Light),_Line);
col.r+=_Color_R;
col.g+=_Color_G;
col.b+=_Color_B;
uv = i.texcoord;
uv.x=uv.x/0.72;
float dif=_Size-_Smooth;
float dist2 = 1.0 - smoothstep(_Size,dif, length(float2(0.694+_Dist,0.5) - uv));
dist2 *= 1.0 - smoothstep(_Size,dif, length(float2(0.694-_Dist,0.5) - uv));
float3 black=float3(0.0,0.0,0.0);
col=lerp(col,colm,_OnOff);
col=lerp(col,black,dist2);

return float4(col,1.0);

}

ENDCG
}

}
}