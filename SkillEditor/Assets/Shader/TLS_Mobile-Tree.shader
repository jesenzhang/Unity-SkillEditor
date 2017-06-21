// Simplified Specular shader. Differences from regular Specular one:
// - no Main Color nor Specular Color
// - specular lighting directions are approximated per vertex
// - writes zero to alpha channel
// - no Deferred Lighting support
// - no Lightmap support
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "TLStudio/Transparent/Tree" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
    _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader { 
	Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
	LOD 150
	Cull Off
CGPROGRAM
#pragma surface surf MobileBlinnPhong noforwardadd alphatest:_Cutoff

inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
{
	fixed diff = max (0.3, dot (s.Normal, lightDir));
	
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * diff*2;
	c.a = s.Alpha ;
	return c;
}
sampler2D _MainTex;
struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb;
	o.Alpha = tex.a;
}
ENDCG
}

FallBack "Transparent/Cutout/VertexLit"
}
