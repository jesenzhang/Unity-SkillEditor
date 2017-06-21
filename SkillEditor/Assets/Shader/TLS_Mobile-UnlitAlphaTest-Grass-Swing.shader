// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "TLStudio/Transparent/UnLit Cutout_Ani" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Range("Range" , Range(0,0.1)) = 0.05

}
SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100

	Lighting Off

	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;
			half	_Range;
			v2f vert (appdata_full v)
			{
				half4	finalpos = mul(unity_ObjectToWorld,v.vertex);
				half3	dist =	_WorldSpaceCameraPos.xyz - finalpos.xyz;
				half4	mdlPos;
				if(length(dist) < 25)
				{
					float 	finalbias = fmod(finalpos.x*finalpos.x + finalpos.y*finalpos.y + finalpos.z*finalpos.z,4);
					if(v.color.r == 0)
					{
						mdlPos	= v.vertex;
					}
					else
					{
						half st = 0;
						if(finalbias < 1) st = max(_CosTime.w + 0.3, -0.5);
						else if(finalbias >= 1 && finalbias < 2) st = min(_SinTime.w, 0.8) * finalbias;
						else if(finalbias >= 2 && finalbias < 3) st = min(_CosTime.w, 0.9) * finalbias;
						else if(finalbias >= 3 && finalbias < 4) st = max(_SinTime.w + 0.8, -0.8) * finalbias;
						mdlPos.xyz = v.vertex.xyz + v.tangent * st * _Range ;
						mdlPos.w = v.vertex.w;
					}
				}
				else
				{
					mdlPos = v.vertex;
				}

				v2f o;
				o.vertex = UnityObjectToClipPos(mdlPos);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				clip(col.a - _Cutoff);
				return col;
			}
		ENDCG
	}
}

}