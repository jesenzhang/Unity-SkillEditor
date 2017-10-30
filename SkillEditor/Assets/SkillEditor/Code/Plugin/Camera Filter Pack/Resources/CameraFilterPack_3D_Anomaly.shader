// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////



Shader "CameraFilterPack/3D_Anomaly" { 
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
uniform sampler2D Texture2;
uniform sampler2D _CameraDepthTexture;
uniform float _TimeX;
uniform float _Value2;


uniform float Anomaly_Distortion;
uniform float Anomaly_Distortion_Size;
uniform float Anomaly_Intensity;


uniform float Anomaly_Near;
uniform float Anomaly_Far;
uniform float Anomaly_With_Obj;

uniform float _FixDistance;


uniform float4 _ScreenResolution;
uniform float2 _MainTex_TexelSize;
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
float4 projPos : TEXCOORD1; 
};

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
OUT.color = IN.color;
OUT.projPos = ComputeScreenPos(OUT.vertex);
return OUT;
}



float4 Anomaly(float2 uv, float d)
{
float t = _Time * 20;
uv.x += sin(t + uv.y * Anomaly_Distortion_Size*0.65) / (128*d);
uv.y += cos(t + uv.x * Anomaly_Distortion_Size) / (128*d);
return tex2D(_MainTex, uv);
}

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif

float4 txt = tex2D(_MainTex, uv);
float4 s= Anomaly(uv, Anomaly_Distortion*0.25)*0.25;
s+= Anomaly(uv, Anomaly_Distortion*0.5)*0.25;
s+= Anomaly(uv, Anomaly_Distortion*0.75)*0.25;
s+= Anomaly(uv, Anomaly_Distortion)*0.25;
s *= Anomaly_Intensity;

float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
depth /= _FixDistance * 10;
depth = saturate(depth);
depth = smoothstep(Anomaly_Near, Anomaly_Far, depth);
txt= lerp(txt, txt+s * _Value2, depth * Anomaly_With_Obj);

return  txt;
}
ENDCG
}
}
}
