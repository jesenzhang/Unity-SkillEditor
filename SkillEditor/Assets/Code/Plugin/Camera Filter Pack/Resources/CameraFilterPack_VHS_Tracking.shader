// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/VHS_Tracking" { 
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
uniform float _Value;


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

float rand(float2 co)
{
return frac(sin(dot(co.xy ,float2(12.98,78.13))) * 43858.5553);
}

float4 frag (v2f i) : COLOR
{  
float2 uv = i.texcoord.xy;
float2 uv_org = float2(uv);
float t = _TimeX;
float t2 = floor(t*0.6);
float x=0;
float y=0;
float yt= abs(cos(t)) * rand(float2(t,t)) * 100.0;
float xt=sin(rand(float2(t,t)))*0.1;
x=uv.x-xt*exp(-pow(uv.y*32.0-yt,2.0)/24.0);
y=uv.y;
uv.x=x;
uv.y=y;
yt = 0.5*cos(yt);
float yr = 0.1*cos(yt);
float3 colrgb = float3(0,0,0);
float colx = 0;
if (uv_org.y > yt && uv_org.y < yt+rand(float2(t2,t))*0.25) 
{
float md = mod(x*100.0,10.0);
if (md*sin(t) > sin(yr*360.0) || rand(float2(md,md))>0.4) 
{
colx = rand(float2(t2,t2))*_Value;
}
}
uv=lerp(uv_org,uv,_Value);
return float4(tex2D(_MainTex, uv).rgb+colx,1.0);


}
ENDCG
}
}
}
