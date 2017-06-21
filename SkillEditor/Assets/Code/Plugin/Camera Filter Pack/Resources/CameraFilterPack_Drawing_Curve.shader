// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Drawing_Curve" { 
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

float4 frag (v2f i) : COLOR
{
float2 xy = i.texcoord.xy ;

float amplitud = 0.03;
float frecuencia = 10.0;
float gris = 1.0;
float divisor = _Value / 512;
float grosorInicial = divisor * 0.2;

float3 datosPatron[6];
datosPatron[0] = float3(-0.7071, 0.7071, 3.0); 
datosPatron[1] = float3(0.0, 1.0, 0.6); 
datosPatron[2] = float3(0.0, 1.0, 0.5); 
datosPatron[3] = float3(1.0, 0.0, 0.4); 
datosPatron[4] = float3(1.0, 0.0, 0.3); 
datosPatron[5] = float3(0.0, 1.0, 0.2); 

float4 color = tex2D(_MainTex, float2(i.texcoord.x / 1.0, xy.y));

for(int i = 0; i < 6; i++)
{
float coseno = datosPatron[i].x;
float seno = datosPatron[i].y;

float2 punto = float2(
xy.x * coseno - xy.y * seno,
xy.x * seno + xy.y * coseno
);

float grosor = grosorInicial * float(i + 1);
float dist = fmod(punto.y + grosor * 0.5 - sin(punto.x * frecuencia) * amplitud, divisor);
float brillo = 0.3 * color.r + 0.4 * color.g + 0.3 * color.b;

if(dist < grosor && brillo < 0.75 - 0.12 * float(i))
{
float k = datosPatron[i].z;
float x = (grosor - dist) / grosor;
float fx = abs((x - 0.5) / k) - (0.5 - k) / k; 
gris = min(fx, gris);
}
}


return float4(gris,gris,gris,1.);
}
ENDCG
}
}
}
