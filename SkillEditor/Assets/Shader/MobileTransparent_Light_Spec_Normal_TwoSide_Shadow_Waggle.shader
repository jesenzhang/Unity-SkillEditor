// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/MobileTransparent_Light_Spec_Normal_TwoSide_Shadow_Waggle" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color("Color (RGB)", Color) = (1, 1, 1, 1)
		_SpecPower("Spec Power",range(0.01,1)) = 1
			_SpecIntensity("Spec Intensity",float) = 1
			_xSpecColor("Spec Color", Color) = (1,1,1,1)
			_SpecMask("Spec Mask,(R channel used)", 2D) = ""{} // use r.
		_NormalTex("Normal Tex", 2D) = "normal"{}
		_WaggleSpeed("Waggle Speed", float) = 50
		_WaggleDir("Waggle Dir", Vector) = (1, 1, 0, 0)
	}

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "LightMode"="ForwardBase"}
		LOD 100

			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha 

			Pass 
			{  
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile_fwdbase

					struct appdata_t 
					{
						float4 vertex : POSITION;
						float2 texcoord : TEXCOORD0;
						float3 normal : NORMAL;
						float4 tangent : TANGENT;
					};

				struct v2f 
				{
					float4 pos : SV_POSITION;
					half2 uv : TEXCOORD0;
					//UNITY_FOG_COORDS(1)
					float3 lightDir : TEXCOORD1;
					//float atten : TEXCOORD2;
					float3 viewDir : TEXCOORD2;	
					LIGHTING_COORDS(3, 4)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				half4 _Color;

				half4 _WaggleDir;
				half _WaggleSpeed;

				half _SpecPower;
				half _SpecIntensity;
				half4 _xSpecColor;

				sampler2D _SpecMask;
				sampler2D _NormalTex;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					//float delta = _Time.x % 2;
					o.pos += sin(_Time.x * _WaggleSpeed) * 0.01 * _WaggleDir;
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					//o.uv += (_Time.x % 0.5) * 0.1 * half2(1, 1);
					//UNITY_TRANSFER_FOG(o,o.vertex);

					/*
					float3 viewPos = mul(UNITY_MATRIX_MV, v.vertex).xyz;
					float3 toLight = unity_LightPosition[0].xyz - viewPos.xyz * unity_LightPosition[0].w;
					float lensq = dot(toLight, toLight);
					o.atten = 1.0 / (1.0 + lensq * unity_LightAtten[0.5].z);
					*/

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

					half4 col;
					col.rgb = texCol * _LightColor0 * diff * atten;
					col.rgb += col.rgb * UNITY_LIGHTMODEL_AMBIENT;

					half3 h = normalize(lightDir + viewDir) * 0.5;
					half nh = max(0, dot(normal, h));
					half3 specCol = _xSpecColor.rgb * _SpecIntensity * pow(nh, _SpecPower * 128) * specular;
					col.rgb += col.rgb * specCol * CalculateRamp(diff, ne);
					col.rgb += specCol * (CalculateViewShine(ne) * _SpecIntensity);

					return col;
				}

				half4 frag (v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					//fixed4 reCol = col * _Color;
					//reCol.a = col.a;
					//UNITY_APPLY_FOG(i.fogCoord, reCol);
					//return reCol;
					//UNITY_APPLY_FOG(i.fogCoord, col);

					half4 specCol = tex2D(_SpecMask, i.uv);
					half3 normal = normalize(UnpackNormal(tex2D(_NormalTex, i.uv)));
					half atten = LIGHT_ATTENUATION(i);
					atten += 0.5;

					col *= _Color;

					half4 reCol = CalculateLight(col, specCol.r, normal, normalize(i.lightDir), normalize(i.viewDir), atten);
					reCol.a = col.a;

					//return col;
					return reCol;
				}
				ENDCG
			}
	}
}
