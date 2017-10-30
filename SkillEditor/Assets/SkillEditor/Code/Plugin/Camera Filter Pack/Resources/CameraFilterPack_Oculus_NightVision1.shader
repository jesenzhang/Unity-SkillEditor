// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Oculus_NightVision1" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(1.0, 10.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)

_Vignette ("_Vignette", Range(0.0, 100.0)) = 1.5
_Linecount ("_Linecount", Range(1.0, 150.0)) = 90.0
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
#pragma glsl
#pragma target 3.0

#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float _Vignette;	
uniform float _Linecount;

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



float2 pos, uv;

float linecount ;


inline float noise(float factor)
{
float4 v = tex2D(_MainTex, uv + _TimeX * float2(9.0, 7.0));
return factor * v.x + (1.0 - factor);
}

inline float4 base(void)
{
return tex2D(_MainTex, uv + .1 * noise(1.0) * float2(0.1, 0.0));
}

inline float trianglee(float phase)
{
phase *= 2.0;
return 1.0 - abs(fmod(phase, 2.0) - 1.0);
}

inline float scanline(float factor, float contrast)
{
float4 v = base();
float lum = .2 * v.x + .5 * v.y + .3  * v.z;
lum *= noise(0.3);
float tri = trianglee(pos.y * linecount);
tri = pow(tri, contrast * (1.0 - lum) + .5);
return tri * lum;
}

inline float4 gradient(float i)
{
i = clamp(i, 0.0, 1.0) * 2.0;
if (i < 1.0) {
return (1.0 - i) * float4(0.0, 0.1, 0.0, 1.0) + i * float4(0.2, 0.5, 0.1, 1.0);
} else {
i -= 1.0;
return (1.0 - i) * float4(0.2, 0.5, 0.1, 1.0) + i * float4(0.9, 1.0, 0.6, 1.0);
}
}

inline float4 vignette(float4 at)
{
float dx = _Vignette * abs(pos.x - .5);
float dy = _Vignette * abs(pos.y - .5);
return at * (1.0 - dx * dx - dy * dy);
}	

float4 frag (v2f i) : COLOR
{
linecount =  _Linecount;
pos = uv = i.texcoord.xy ;
uv.y = floor(uv.y * linecount) / linecount;
return vignette(gradient(scanline(0.8, 2.0)));

}

ENDCG
}

}
}