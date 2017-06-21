// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/FX_DigitalMatrixDistortion" { 
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
uniform float _Value5;
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
#define R(v) fmod(4e4*sin(dot(ceil(v),float2(12,7))),10.)

float4 frag (v2f a) : COLOR
{
float2 u=a.texcoord;
float time=_TimeX*_Value5;

float2 p = 6.*frac(u *= 24./_Value) - .5;
u.y += ceil(time*2.*R(u.xx));
float4 o=float4(0,0,0,0);
int i=int(p.y);
i = ( abs(p.x-1.5)>1.1 ? 0: i==5? 972980223: i==4? 690407533: i==3? 704642687: i==2? 696556137:i==1? 972881535: 0 
) / int(exp2(30.-ceil(p.x)-3.*floor(R(u))));  
if(R(++u)<9.9) o=float4(1,1,1,1); 
if (i > i/2*2) o+=float4(1,0,0,1); else o=float4(0,0,0,0);  

u=a.texcoord;
p = 6.*frac(u *= 24./(_Value/2)) - .5;
u.y += ceil(time*2.*R(u.xx)/2);
i=int(p.y);
i = ( abs(p.x-1.5)>1.1 ? 0: i==5? 972980223: i==4? 690407533: i==3? 704642687: i==2? 696556137:i==1? 972881535: 0 
) / int(exp2(30.-ceil(p.x)-3.*floor(R(u))));  
if(R(++u)<9.9) o/=float4(1,1,1,1); 
if (i > i/2*2) o+=float4(1,0,0,1); else o+=float4(0,0,0,0);  

o.r*=_Value2;
o.g*=_Value3;
o.b*=_Value4;


float4 v=tex2D(_MainTex,a.texcoord+float2(0,o.r/128));

return v;   

}
ENDCG
}
}
}
