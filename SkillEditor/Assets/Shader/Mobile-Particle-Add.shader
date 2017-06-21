// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Mobile/Particles/Additive" {
	Properties {
		_MainTex ("Particle Texture", 2D) = "white" {}
		_ColorScale("color scale", Range(0,2)) = 1
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// uniforms
			half4 _MainTex_ST;
			fixed _ColorScale;

			// vertex shader input data
			struct a2v {
				float4 pos : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			// vertex-to-fragment interpolators
			struct v2f {
				fixed4 color : COLOR0;
				half2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			// vertex shader
			v2f vert (a2v v) {
				v2f o;
				o.color = v.color;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pos = UnityObjectToClipPos(v.pos);
				return o;
			}

			// textures
			sampler2D _MainTex;

			// fragment shader
			fixed4 frag (v2f i) : SV_Target {
				return tex2D(_MainTex, i.uv) * i.color * _ColorScale;
			}
			ENDCG
		}
	}

}