// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Colors_HSV" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_HueShift("HueShift",  Range (0,360) ) = 0
_Sat("Saturation", Float) = 1
_Val("Value", Float) = 1
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
//		#pragma target 3.0
#include "UnityCG.cginc"


uniform sampler2D _MainTex;
uniform float _HueShift;
uniform float _Sat;
uniform float _Val;


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

float3 shift_col(float3 RGB, float3 shift)
{

float3 RESULT = float3(RGB);
float VSU = shift.z*shift.y*cos(shift.x*3.14159265/180);
float VSW = shift.z*shift.y*sin(shift.x*3.14159265/180);

RESULT.x = (.299*shift.z+.701*VSU+.168*VSW)*RGB.x
+ (.587*shift.z-.587*VSU+.330*VSW)*RGB.y
+ (.114*shift.z-.114*VSU-.497*VSW)*RGB.z;

RESULT.y = (.299*shift.z-.299*VSU-.328*VSW)*RGB.x
+ (.587*shift.z+.413*VSU+.035*VSW)*RGB.y
+ (.114*shift.z-.114*VSU+.292*VSW)*RGB.z;

RESULT.z = (.299*shift.z-.3*VSU+1.25*VSW)*RGB.x
+ (.587*shift.z-.588*VSU-1.05*VSW)*RGB.y
+ (.114*shift.z+.886*VSU-.203*VSW)*RGB.z;

return (RESULT);
}



float4 frag (v2f i) : COLOR
{
return float4(shift_col(tex2D(_MainTex, i.texcoord), float3(_HueShift, _Sat, _Val)), 1.0);
}

ENDCG
}

}
}