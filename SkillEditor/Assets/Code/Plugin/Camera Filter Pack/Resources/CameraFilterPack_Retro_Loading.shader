// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Retro_Loading" { 
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
uniform float _Value2;
uniform float _Value3;
uniform float _Value4;
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
float random (in float x) { return frac(sin(x)*1e4); }
float random (in float2 _st) { return frac(sin(dot(_st.xy, float2(12.9898,78.233)))* 43758.5453123);}

float4 frag (v2f i) : COLOR 
{
float2 st = i.texcoord.xy ;
st.x *= 1.0/1.0;

float2 grid = float2(50.0,30.);
st *= grid;
float time=_TimeX*_Value;
float2 ipos = floor(st);  
float2 vel = floor(float2(time*10.,_TimeX*10.)); 
vel *= float2(-1.,0.); 
vel *= (step(1., fmod(ipos.y,2.0))-0.5)*2.; 
vel *= random(ipos.y); 

float totalCells = grid.x*grid.y;
float t = fmod(time*max(grid.x,grid.y)+floor(1.0+_TimeX),totalCells);
float2 head = float2(fmod(t,grid.x), floor(t/grid.x));

float2 offset = float2(0.1,0.);
float3 color = float3(1.0,1.0,1.0);

color *= step(grid.y-head.y,ipos.y);                               
color += (1.0-step(head.x,ipos.x))*step(grid.y-head.y,ipos.y+1.);   
color = clamp(color,float3(0,0,0),float3(1,1,1));

color.r *= random(floor(st+vel+offset));
color.g *= random(floor(st+vel));
color.b *= random(floor(st+vel-offset));

color = smoothstep(0.,.5/1.0*.5,color*color); 
color = step(0.5/1.0*0.5,color); 
color *= step(.1,frac(st.x+vel.x))*step(.1,frac(st.y+vel.y));

float4 v=tex2D(_MainTex,i.texcoord);

return  float4(v.rgb+color,1.0);
}
ENDCG
}
}
}
