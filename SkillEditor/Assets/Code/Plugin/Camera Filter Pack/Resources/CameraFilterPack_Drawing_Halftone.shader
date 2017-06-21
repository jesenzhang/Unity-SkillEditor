// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

Shader "CameraFilterPack/Drawing_Halftone" {
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
_DotSize ("_DotSize", Range(1.0, 16.0)) = 10
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
uniform float _DotSize;

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

inline float added(float2 sh, float d)
{
float2 rsh = sh * 0.70710638280; 
return 0.5 + 0.25 * cos((rsh.x + rsh.y) * d) + 0.25 * cos((rsh.x - rsh.y) * d);
}

fixed4 frag (v2f i) : COLOR
{
fixed threshold 		= _Distortion;

// 0.70710638280 = 45 degree
fixed rasterPattern = added(i.texcoord.xy , 2136.2812 / _DotSize  );
fixed4 srcPixel 	= tex2D(_MainTex, i.texcoord.xy);

fixed avg 			= 0.2125 * srcPixel.r + 0.7154 * srcPixel.g + 0.0721 * srcPixel.b;
fixed gray 			= (rasterPattern * threshold + avg - threshold) / (1.0 - threshold);

return fixed4(gray,gray,gray,1.0);


}

ENDCG
}

}
}