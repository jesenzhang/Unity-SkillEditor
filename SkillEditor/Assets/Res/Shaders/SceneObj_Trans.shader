// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Instanced2/SceneObj_Trans"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_NormalTex("_NormalTex", 2D) = "bump" {}
		_SpecularTex("_SpecularTex", 2D) = "white" {}
		_Specular("_Specular", Range(1.0, 2000.0)) = 250.0
		_Gloss("_Gloss", Range(0.0, 2.0)) = 1.0
		_cutoff("Base Alpha Cutoff", Range(0,1)) = .5
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200
		Cull Off

		// first pass
		Pass
		{
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag  
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON  
			#pragma multi_compile_fwdbase  

			#include "UnityCG.cginc"  
			#include "Lighting.cginc"  
			#include "AutoLight.cginc"  

			sampler2D _MainTex; float4 _MainTex_ST;
			sampler2D _NormalTex; float4 _NormalTex_ST;
			sampler2D _SpecularTex; float4 _SpecularTex_ST;
			float _Specular;
			float _Gloss;
			float _cutoff;

			struct a2v
			{
				float4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 tangent : TANGENT;
				fixed4 texcoord : TEXCOORD0;
				fixed4 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos : POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 lightDir : TEXCOORD1;
				fixed3 viewDir : TEXCOORD2;
				fixed4 TtoW0 : TEXCOORD3;
				fixed4 TtoW1 : TEXCOORD4;
				fixed4 TtoW2 : TEXCOORD5;
				//#ifndef LIGHTMAP_OFF  
				half2 uvLM : TEXCOORD6;
				//#endif  
				LIGHTING_COORDS(7,8)
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				//Create a rotation matrix for tangent space  
				TANGENT_SPACE_ROTATION;
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

				float3 worldView = mul((float3x3)unity_ObjectToWorld, -ObjSpaceViewDir(v.vertex));
				o.TtoW0 = float4(mul(rotation, unity_ObjectToWorld[0].xyz), worldView.x) * 1.0;
				o.TtoW1 = float4(mul(rotation, unity_ObjectToWorld[1].xyz), worldView.y) * 1.0;
				o.TtoW2 = float4(mul(rotation, unity_ObjectToWorld[2].xyz), worldView.z) * 1.0;

				//#ifndef LIGHTMAP_OFF  
				o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				//#endif  //

				// pass lighting information to pixel shader  
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 texColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				fixed3 norm = UnpackNormal(tex2D(_NormalTex, TRANSFORM_TEX(i.uv, _NormalTex)));
				fixed4 specColor = tex2D(_SpecularTex, TRANSFORM_TEX(i.uv, _SpecularTex));

				fixed atten = LIGHT_ATTENUATION(i);

				fixed3 ambi = UNITY_LIGHTMODEL_AMBIENT.xyz;

	
				fixed3 diff = _LightColor0.rgb * saturate(dot(normalize(norm),  normalize(i.lightDir)));

				fixed3 lightRefl = reflect(-i.lightDir, norm);
				fixed3 spec = _LightColor0.rgb * pow(saturate(dot(normalize(lightRefl), normalize(i.viewDir))), _Specular) * specColor.rgb * _Gloss;

				fixed4 fragColor;
				fragColor.rgb = float3((ambi + (diff + spec) * atten) * texColor);
				fragColor.a = texColor.a;

				//#ifndef LIGHTMAP_OFF  
				fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));
				//fragColor.rgb *= lm.rgb;
				//#endif  

				UNITY_APPLY_FOG(i.fogCoord, fragColor);
				clip(fragColor.a - _cutoff);

				//return fixed4(lm, 1);
				return fragColor;
			}

			ENDCG
		}

		// second pass
		Pass
		{
			Tags{ "RequireOption" = "SoftVegetation" }

			// Dont write to the depth buffer
			ZWrite off

			// Set up alpha blending
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag  
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON  
			#pragma multi_compile_fwdbase  

			#include "UnityCG.cginc"  
			#include "Lighting.cginc"  
			#include "AutoLight.cginc"  

			sampler2D _MainTex; float4 _MainTex_ST;
			sampler2D _NormalTex; float4 _NormalTex_ST;
			sampler2D _SpecularTex; float4 _SpecularTex_ST;
			float _Specular;
			float _Gloss;
			float _cutoff;

			struct a2v
			{
				float4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 tangent : TANGENT;
				fixed4 texcoord : TEXCOORD0;
				fixed4 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos : POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 lightDir : TEXCOORD1;
				fixed3 viewDir : TEXCOORD2;
				fixed4 TtoW0 : TEXCOORD3;
				fixed4 TtoW1 : TEXCOORD4;
				fixed4 TtoW2 : TEXCOORD5;
				//#ifndef LIGHTMAP_OFF  
				half2 uvLM : TEXCOORD6;
				//#endif 
				LIGHTING_COORDS(7,8)
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				//Create a rotation matrix for tangent space  
				TANGENT_SPACE_ROTATION;
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

				float3 worldView = mul((float3x3)unity_ObjectToWorld, -ObjSpaceViewDir(v.vertex));
				o.TtoW0 = float4(mul(rotation, unity_ObjectToWorld[0].xyz), worldView.x) * 1.0;
				o.TtoW1 = float4(mul(rotation, unity_ObjectToWorld[1].xyz), worldView.y) * 1.0;
				o.TtoW2 = float4(mul(rotation, unity_ObjectToWorld[2].xyz), worldView.z) * 1.0;

				//#ifndef LIGHTMAP_OFF  
				o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				//#endif  

				// pass lighting information to pixel shader  
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 texColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				fixed3 norm = UnpackNormal(tex2D(_NormalTex, TRANSFORM_TEX(i.uv, _NormalTex)));
				fixed4 specColor = tex2D(_SpecularTex, TRANSFORM_TEX(i.uv, _SpecularTex));

				fixed atten = LIGHT_ATTENUATION(i);

				fixed3 ambi = UNITY_LIGHTMODEL_AMBIENT.xyz;
	
				fixed3 diff = _LightColor0.rgb * saturate(dot(normalize(norm),  normalize(i.lightDir)));

				fixed3 lightRefl = reflect(-i.lightDir, norm);
				fixed3 spec = _LightColor0.rgb * pow(saturate(dot(normalize(lightRefl), normalize(i.viewDir))), _Specular) * specColor.rgb * _Gloss;

				fixed4 fragColor;
				fragColor.rgb = float3((ambi + (diff + spec) * atten) * texColor);
				fragColor.a = texColor.a;

				//#ifndef LIGHTMAP_OFF  
				fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));
				//fragColor.rgb *= lm.rgb;
				//#endif

				UNITY_APPLY_FOG(i.fogCoord, fragColor);
				clip(-(fragColor.a - _cutoff));

				return fragColor;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}