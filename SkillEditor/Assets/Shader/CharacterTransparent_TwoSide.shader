// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/CharacterTransparent_TwoSide" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_SpecPower("Spec Power",range(0.01,1)) = 1
			_SpecIntensity("Spec Intensity",float) = 1
			_xSpecColor("Spec Color", Color) = (1,1,1,1)
			_SpecRange("Spec Range", Range(0, 2)) = 0.97
			_SpecRangeMul("SpecRangeMul", float) = 200
	}

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" }
		//Tags {"LightMode"="ForwardBase"}
		LOD 100

			ZWrite On
			Cull Off

			Blend SrcAlpha OneMinusSrcAlpha 

			/*
			Pass 
			{ 
				AlphaTest Greater 0.3
					SetTexture [_MainTex] 
					{ 
						combine texture * primary, texture 
					} 
			} 
			*/

		Pass 
		{  
			//Blend SrcAlpha OneMinusSrcAlpha 
				//AlphaTest LEqual 0.3

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
				};

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half3 normal : TEXCOORD1;
				half3 lightDir : TEXCOORD2;
				half atten : TEXCOORD3;
				half3 viewDir : TEXCOORD4;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			half _SpecPower;
			half _SpecIntensity;
			half4 _xSpecColor;

			half _SpecRange;
			half _SpecRangeMul;


			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.normal = normalize(v.normal);

				float3 viewPos = mul(UNITY_MATRIX_MV, v.vertex).xyz;
				float3 toLight = unity_LightPosition[0].xyz - viewPos.xyz * unity_LightPosition[0].w;
				float lensq = dot(toLight, toLight);
				o.atten = 1.0 / (1.0 + lensq * unity_LightAtten[0.5].z);

				o.lightDir = normalize(-ObjSpaceLightDir(v.vertex));
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));

				return o;
			}

			half3 CalculateRamp(half diff, half ne)
			{
				ne += 1;
				half v = pow(ne, diff);
				return half3(v, v, v);
			}

			half CalculateViewShine(half n)
			{
				//half v = n - 0.97;
				half v = n - _SpecRange;
				v = max(0, v);
				//v *= 2000;
				v *= _SpecRangeMul;
				return v + n;
			}

			half4 CalculateLight(half4 texCol, half3 normal, half3 lightDir, half3 viewDir, half atten)
			{
				//half4 ndotl = dot(normal, normalize(lightDir.xyz));
				half4 ndotl = dot(normal, lightDir.xyz);
				half diff = ndotl * 0.5 + 0.6;
				half ne = dot(normal, viewDir);

				half4 col;
				col.rgb = texCol * _LightColor0 * diff * atten;
				//col.rgb += col.rgb * UNITY_LIGHTMODEL_AMBIENT;

				half3 h = normalize(lightDir + viewDir) * 0.5;
				half nh = max(0, dot(normal, h));
				half3 specCol = _xSpecColor.rgb * _SpecIntensity * pow(nh, _SpecPower * 128);
				col.rgb += col.rgb * specCol * CalculateRamp(diff, ne);
				col.rgb += specCol;
				col.rgb += specCol * (CalculateViewShine(ne) * _SpecIntensity);

				col.a = texCol.a;
				return col;
			}

			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.texcoord);
				col = CalculateLight(col, i.normal, i.lightDir, i.viewDir, i.atten);
				return col;
			}
			ENDCG
		}
	}
}
