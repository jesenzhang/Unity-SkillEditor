// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Pixelisation_Dot" { 
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
float gridSize = _Value;
float3 backgroundColor=float3(_Value2,_Value2,_Value2);
float2 q = (i.texcoord.xy);
float2 uv = (floor(i.texcoord.xy/gridSize)*gridSize);
float3 texColor = tex2D(_MainTex,uv).xyz;
float diff = pow(distance(texColor,float3(0.0,1.0,0.0)),8.0); 
diff = smoothstep(0.0,1.5,diff);
texColor = lerp(backgroundColor,texColor,diff);
float texLum = dot(float3(0.2126,0.7152,0.0722),texColor);
float3 color = backgroundColor;
float2 ppos = (q - uv)/(float2(gridSize,gridSize));
float power = texLum*texLum*16.0;
float radius = 0.5;
float dist = pow(abs(ppos.x-0.5),power) + pow(abs(ppos.y - 0.5),power);
if( dist < pow(radius,power))
{
color = texColor;
}
return  float4(color,1.0); 
}
ENDCG
}
}
}
