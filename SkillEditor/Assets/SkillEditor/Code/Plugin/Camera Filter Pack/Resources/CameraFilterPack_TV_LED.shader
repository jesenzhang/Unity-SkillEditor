// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/TV_LED" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(1.0, 10.0)) = 1.0
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
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Distortion;
uniform float _Size;
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
float2 tex 		=  i.texcoord.xy *_ScreenResolution.xy;
float2 p 		=  floor(tex / _Size) * _Size; 
float2 offset 	=  fmod (tex,_Size);
float3 col=0;
float3 sum = tex2D(_MainTex, p / _ScreenResolution.xy);

float blurBar = clamp(sin(tex.y * 6.0 + _TimeX * 5.6) + 1.25, 0.0, 1.0);
if (offset.y < (_Size/3)*3) {		
if (offset.x < _Size/3) col.r = sum.r;
else if (offset.x < (_Size/3)*2) col.g = sum.g;
else col.b = sum.b;
}
col=sum+col;
col = lerp(col -  0.2, col, blurBar/2);
return float4(col,1.0);


}

ENDCG
}

}
}