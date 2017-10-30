// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/Lut_TestMode" {
Properties
{
_MainTex("Base (RGB)", 2D) = "" {}
}

CGINCLUDE
#include "UnityCG.cginc"

struct v2f {
float4 pos : SV_POSITION;
float2 uv  : TEXCOORD0;
};

sampler2D _MainTex;
sampler3D _LutTex;
float _Blend;
float _Intensity;
float _Extra;
float _Extra2;
float _Extra3;

v2f vert(appdata_img v)
{
v2f o;
o.pos = UnityObjectToClipPos(v.vertex);
o.uv = v.texcoord.xy;
return o;
}

float4 frag(v2f i) : SV_Target
{
	float4 c = tex2D(_MainTex, i.uv);
	if (i.uv.x > _Extra3) c=c*_Intensity;
	if (i.uv.x>_Extra3) c.rgb = lerp(c.rgb,tex3D(_LutTex, c.rgb+_Extra).rgb+_Extra2, _Blend);
	return c;
}

float4 fragLinear(v2f i) : SV_Target
{
	float4 c = tex2D(_MainTex, i.uv);
	if (i.uv.x > _Extra3) c = c*_Intensity;
	c.rgb = sqrt(c.rgb);
	if (i.uv.x>_Extra3) c.rgb = lerp(c.rgb,tex3D(_LutTex, c.rgb+_Extra).rgb + _Extra2, _Blend);
	c.rgb = c.rgb*c.rgb;
	return c;
}

ENDCG


Subshader
{
Pass
{
ZTest Always Cull Off ZWrite Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
ENDCG
}

Pass
{
ZTest Always Cull Off ZWrite Off

CGPROGRAM
#pragma vertex vert
#pragma fragment fragLinear
#pragma target 3.0
ENDCG
}
}

Fallback off

}