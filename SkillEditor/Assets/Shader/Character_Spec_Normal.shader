// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Character_Spec_Normal" 
{
	Properties 
	{
		//--- base params
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("MainTex(RGB)", 2D) = "white" {}
		//--- rim params
		_RimColor("RimColor",color) = (1,1,1,1)
		_RimPower("RimPower",Range(0.01,0.5)) = .2
		//spec
		_SpecPower("Spec Power",range(0.01,1)) = 1
		_SpecIntensity("Spec Intensity",float) = 1
		_xSpecColor("Spec Color", Color) = (1,1,1,1)
		_SpecMask("Spec Mask,(R channel used)", 2D) = ""{} // use r.
		_NormalTex("Normal Tex", 2D) = "normal"{}
	}

	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
			LOD 200
			Cull Off

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

				sampler2D _MainTex;

			half4 _Color;

			half4 _RimColor;
			half _RimPower;

			half _SpecPower;
			half _SpecIntensity;
			half4 _xSpecColor;

			sampler2D _SpecMask;
			sampler2D _NormalTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
				float3 lightDir : TEXCOORD1;
				float atten : TEXCOORD2;
				float3 viewDir : TEXCOORD3;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float3 viewPos = mul(UNITY_MATRIX_MV, v.vertex).xyz;
				float3 toLight = unity_LightPosition[0].xyz - viewPos.xyz * unity_LightPosition[0].w;
				float lensq = dot(toLight, toLight);
				o.atten = 1.0 / (1.0 + lensq * unity_LightAtten[0.5].z);

				TANGENT_SPACE_ROTATION;
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
				TRANSFER_VERTEX_TO_FRAGMENT(o);

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
				half v = n - 0.97;
				v = max(0, v);
				v *= 200;
				return v + n;
			}

			half4 CalculateLight(half4 texCol, half specular, half3 normal, half3 lightDir, half3 viewDir, half atten)
			{
				half4 ndotl = dot(normal, normalize(lightDir.xyz));
				half diff = ndotl * 0.5 + 0.6;
				half ne = dot(normal, viewDir);

				half rim = 1 - saturate(dot(viewDir, normal));
				half3 emission = _RimColor.rgb * (rim * _RimPower);

				half4 col;
				col.rgb = texCol * _LightColor0 * diff * atten + emission;
				col.rgb += col.rgb * UNITY_LIGHTMODEL_AMBIENT;

				half3 h = normalize(lightDir + viewDir) * 0.5;
				half nh = max(0, dot(normal, h));
				half3 specCol = _xSpecColor.rgb * _SpecIntensity * pow(nh, _SpecPower * 128) * specular;
				col.rgb += col.rgb * specCol * CalculateRamp(diff, ne);
				col.rgb += specCol * (CalculateViewShine(ne) * _SpecIntensity);

				return col;
			}

			half4 frag(v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				half4 specCol = tex2D(_SpecMask, i.uv);
				half3 normal = UnpackNormal(tex2D(_NormalTex, i.uv));

				col *= _Color;

				half4 reCol = CalculateLight(col, specCol.r, normalize(normal), normalize(i.lightDir), normalize(i.viewDir), i.atten);
				reCol.a = col.a;
				
				return reCol;
			}
			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}
