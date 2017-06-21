// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Vision_Hell_Blood" { 
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
float2 hash22(float2 p) 
{ 
return frac(sin(p.x+p.y*2)); 
}

float Voronoi(float2 p)
{	
float2 ip = floor(p);
p = frac(p);

float d = 1.;
for (float i = -1.; i < 1.1; i++){
for (float j = -1.; j < 1.1; j++){
float2 offset = hash22(ip + float2(i, j));
float2 r = float2(i, j) + offset - p; 
float d2 = dot(r, r);
d = min(d, d2);
}
}

return sqrt(d); 
}

float4 frag (v2f i) : COLOR
{
float2 uv2 = i.texcoord;
float2 uv = uv2-float2(0.5,0.5);

float ti = _Time*_Value3;
float t = ti, s, a, e;
float th = 0.37;
float cs = cos(th);
float si = sin(th);

float3 sp = float3(uv, 0);
float3 ro = float3(0, 0, -1);
float3 lp = float3(cos(t)*0.375, sin(t)*0.1, -1.);
float sum = 0.;

float2 M = float2(cs/-si, cs/si);

float3 col = float3(0,0,0);
float f=0., fx=0., fy=0.;
float2 eps = float2(0.0078125, 0.);
float2 offs = float2(0.1,0.1);


for (float i = 0.; i<4; i++){
s = frac((i - t*2.)*.2);
e = exp2(s*6)*0.5;
a = (1.-cos(s*6.283))/e;
f += Voronoi(M*sp.xy*e + offs) * a;
fx += Voronoi(M*(sp.xy+eps.xy)*e + offs) * a;
fy += Voronoi(M*(sp.xy+eps.yx)*e + offs) * a;
sum += a;
}

f /= sum;
fx /= sum;
fy /= sum;
fx = (fx-f)/eps.x;
fy = (fy-f)/eps.x;
float3 n = normalize( float3(0, 0, -1) - float3(fx, fy, 0)*0.2 );           
float3 ld = lp - sp;
float lDist = max(length(ld), 0.001);
ld /= lDist;
float atten = min(1.*(lDist*0.75 + lDist*lDist*0.15), 1.);
float diff = max(dot(n, ld), 0.);  
diff = pow(diff, 2.)*0.66 + pow(diff, 4.)*0.34; 
float spec = pow(max(dot( reflect(-ld, n), ro), 0.), 8.); 
float3 objCol = float3(f, f*f*sqrt(f)*0.4, f*0.6);
col = (objCol * (diff + 0.5) + float3(0.5, 0.85, 1.)*spec) * atten;
col.r*=1.5;
float3 col2 = tex2D(_MainTex,uv2-float2(col.r*0.15,col.r*0.15)).rgb*_Value4;
float3 col3 = tex2D(_MainTex,uv2).rgb;
col-= col2;
float dist2 =smoothstep(_Value,_Value-_Value2, length(uv));
col=lerp(col,col3,dist2);
return  float4(col, 1.);
}
ENDCG
}
}
}
