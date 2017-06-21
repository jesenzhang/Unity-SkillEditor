// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/FX_Drunk" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
_Value ("_Value", Range(0.0, 20.0)) = 6.0
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
uniform float _Distortion;
uniform float4 _ScreenResolution;
uniform float _Value;
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

fixed4 frag (v2f i) : COLOR
{
float drunk 	 = (sin(_TimeX*2.0)*_Value) ;
float unitDrunk1 = (sin(_TimeX*1.2)+1.0)/2.0;
float unitDrunk2 = (sin(_TimeX*1.8)+1.0)/2.0;

float2 normalizedCoord = fmod((i.texcoord.xy + (float2(0, drunk) / _ScreenResolution.x)), 1.0);
normalizedCoord.x = pow(normalizedCoord.x, lerp(1.25, 0.85, unitDrunk1));
normalizedCoord.y = pow(normalizedCoord.y, lerp(0.85, 1.25, unitDrunk2));

float2 normalizedCoord2 = fmod((i.texcoord.xy + (float2(drunk, 0.) / _ScreenResolution.x)), 1.0);	
normalizedCoord2.x = pow(normalizedCoord2.x, lerp(0.95, 1.1, unitDrunk2));
normalizedCoord2.y = pow(normalizedCoord2.y, lerp(1.1, 0.95, unitDrunk1));

float2 normalizedCoord3 = i.texcoord.xy;

fixed4 color  = tex2D(_MainTex, normalizedCoord);	
fixed4 color2 = tex2D(_MainTex, normalizedCoord2);
fixed4 color3 = tex2D(_MainTex, normalizedCoord3);

color.x  = sqrt(color2.x);
color2.x = color.x;

fixed4 finalColor = lerp( lerp(color, color2, lerp(0.4, 0.6, unitDrunk1)), color3, 0.4);

if (length(finalColor) > 1.4)
finalColor.xy = lerp(finalColor.xy, normalizedCoord3, 0.5);
else if (length(finalColor) < 0.4)
finalColor.yz = lerp(finalColor.yz, normalizedCoord3, 0.5);

return finalColor;	
}

ENDCG
}

}
}