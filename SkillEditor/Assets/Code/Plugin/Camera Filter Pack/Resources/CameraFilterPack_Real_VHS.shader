// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////
Shader "CameraFilterPack/Real_VHS" 
{

Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
VHS ("Base (RGB)", 2D) = "white" {}
VHS2 ("Base (RGB)", 2D) = "white" {}
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
uniform sampler2D VHS;
uniform sampler2D VHS2;
uniform float TRACKING;
uniform float CONTRAST;


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

float3 YUV2RGB(float Y,float U, float V)
{
float R=Y+1.140*V;
float G=Y-0.395*U-0.581*V;
float B=Y+2.032*U;
return float3(R,G,B); 
}

float3 RGB2YUV(float R,float G, float B)
{
float Y=0.299*R +0.587* G + 0.114* B;
float U=-0.147*R - 0.289* G + 0.436* B;
float V= 0.615*R - 0.515* G - 0.100* B;
return float3(Y,U,V); 
} 

float hardLight( float s, float d )
{
return (s < 0.5) ? 2.0 * s * d : 1.0 - 2.0 * (1.0 - s) * (1.0 - d);
}

float3 hardLight( float3 s, float3 d )
{
float3 c;
c.x = hardLight(s.x,d.x);
c.y = hardLight(s.y,d.y);
c.z = hardLight(s.z,d.z);
return c;
}

float rand(float2 co)
{
float a = 12.9898;
float b = 78.233;
float c = 43758.5453;
float dt= dot(co.xy ,float2(a,b));
float sn= fmod(dt,3.14);
float t=frac(sin(sn) * c);
return t;
}

float4 frag (v2f i) : COLOR
{  			
float2 uv = i.texcoord.xy;
if (uv.y<0.025) uv.x+=(uv.y-0.05)*(sin(uv.y*512+_Time*12));
if (uv.y<0.015) uv.x+=(uv.y-0.05)*(sin(uv.y*512+_Time*64));
float fx=sin(uv.y*150+_Time*48)/64;
float time=_Time;
float rnd=rand(time*20)*15;
float vr=smoothstep(rnd,rnd+0.03,uv.y);
vr-=smoothstep(rnd+0.06,rnd+0.09,uv.y);
uv.x+=lerp(0,fx,vr);
float uvx=0;
float uvy=floor(uv.y*288)/288;
uvx = rand(float2(time*0.013,uvy*0.42)) * 0.004;
uvx += sin(rand(float2(time*0.4, uvy)))* 0.0050;
uv.x+=(uvx*(1-uv.x));
float3 col = tex2D(_MainTex,uv).rgb;
col=clamp(col,0.08,0.95);
float3 yuv=RGB2YUV(col.r,col.g,col.b);
float s=sin(_Time*128)/128;
float c=cos(_Time*128)/128;
uv.x=floor(uv.x*52)/52;
uv.y=floor(uv.y*288)/288;
col = tex2D(_MainTex,uv+float2(-0.01+s,0+c)*s).rgb;
float3 yuv2=RGB2YUV(col.r,col.g,col.b)/(CONTRAST+1);
float wave=max(sin(uv.y*24+_Time*64),0);
wave+=max(sin(uv.y*14+_Time*16),0);
wave/=2;
col=YUV2RGB(yuv.r,yuv2.g*(wave+0.5),yuv2.b*(wave+0.5)); 
col=clamp(col,0.08,0.95);
col*=1.05;
uv = i.texcoord.xy/8;
float tm=_Time*30;
uv.x += floor(fmod(tm, 1.0)*8)/8;
uv.y += (1-floor(fmod(tm/8, 1.0)*8)/8);
fixed4 t2 =  tex2D(VHS, uv);
col=hardLight(col,t2);
uv = i.texcoord.xy/8;
uv.y=1-uv.y;
tm=_Time*30;
uv.x += floor(fmod(tm, 1.0)*8)/8;
uv.y += (1-floor(fmod(tm/8, 1.0)*8)/8);
t2 =  tex2D(VHS2, uv);
uv = i.texcoord.xy;
col=lerp(col,col+t2,TRACKING*(1-uv.y));
return float4(col,1.0);
}

ENDCG
}

}
}