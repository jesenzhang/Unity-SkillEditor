// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Tex_Spec_Reflect"
{
    Properties
	{
        _main_tex ("main_tex", 2D) = "white" {}
        _selfIllum_tex ("selfIllum_tex", 2D) = "white" {}
        _node_Normal ("normal_tex", 2D) = "bump" {}
		_gloss ("gloss", Range(0, 1)) = 0.3766829
		_Cutoff("Base Alpha cutoff", Range(0,.9)) = .5
		_armor_Intensity("FuZhuangLiangDu", Range(0, 4)) = 0.9496132
        _armor_Specular ("FuZhuangGaoGuang", Range(0, 3)) = 0.7968687
		_refle_Specular("FanSheGaoGuang", Range(0, 3)) = 0.7968687
		_backlight_Color("NiGuangBuSe", Color) = (0.5,0.5,0.5,1)
		_backlight_Intensity("NiGuangLiangDu", Range(0, 4)) = 0.9496132
        _edge_Intensity ("BianYuanLiangDu", Range(0, 2)) = 1
        _armor_Colorization ("FuZhuangBianSe", Color) = (1,0,0,1)
    }

    SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }

			//Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            //#pragma target 3.0
            uniform sampler2D _main_tex; uniform float4 _main_tex_ST;
            uniform float _gloss;
			uniform float _Cutoff;
            uniform sampler2D _selfIllum_tex; uniform float4 _selfIllum_tex_ST;
            uniform sampler2D _node_Normal; uniform float4 _node_Normal_ST;
            uniform float _armor_Intensity;
            uniform float _armor_Specular;
			uniform float _refle_Specular;
            uniform float4 _backlight_Color;
			uniform float _backlight_Intensity;
            uniform float _edge_Intensity;
            uniform float4 _armor_Colorization;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }

			UnityGI ComputeUnityGIData(float3 lightColor, float3 lightDirection, float3 normalDirection, 
				float4 posWorld, float3 viewDirection,float attenuation,float3 viewReflectDirection, float gloss)
			{
				UnityLight light;
#ifdef LIGHTMAP_OFF
				light.color = lightColor;
				light.dir = lightDirection;
				light.ndotl = LambertTerm(normalDirection, light.dir);
#else
				light.color = half3(0.f, 0.f, 0.f);
				light.ndotl = 0.0f;
				light.dir = half3(0.f, 0.f, 0.f);
#endif
				UnityGIInput d;
				d.light = light;
				d.worldPos = posWorld.xyz;
				d.worldViewDir = viewDirection;
				d.atten = attenuation;
				d.boxMax[0] = unity_SpecCube0_BoxMax;
				d.boxMin[0] = unity_SpecCube0_BoxMin;
				d.probePosition[0] = unity_SpecCube0_ProbePosition;
				d.probeHDR[0] = unity_SpecCube0_HDR;
				d.boxMax[1] = unity_SpecCube1_BoxMax;
				d.boxMin[1] = unity_SpecCube1_BoxMin;
				d.probePosition[1] = unity_SpecCube1_ProbePosition;
				d.probeHDR[1] = unity_SpecCube1_HDR;
				Unity_GlossyEnvironmentData ugls_en_data;
				ugls_en_data.roughness = 1.0 - gloss;
				ugls_en_data.reflUVW = viewReflectDirection;
				UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data);
				return gi;
			}
			
			/*
			//RGB to HSV  
			fixed3 RGBConvertToHSV(fixed3 rgb)
			{
				fixed R = rgb.x, G = rgb.y, B = rgb.z;
				fixed3 hsv;
				fixed max1 = max(R, max(G, B));
				fixed min1 = min(R, min(G, B));
				if (R == max1)
				{
					hsv.x = (G - B) / (max1 - min1);
				}
				if (G == max1)
				{
					hsv.x = 2 + (B - R) / (max1 - min1);
				}
				if (B == max1)
				{
					hsv.x = 4 + (R - G) / (max1 - min1);
				}
				hsv.x = hsv.x * 60.0;
				if (hsv.x < 0)
					hsv.x = hsv.x + 360;
				hsv.z = max1;
				hsv.y = (max1 - min1) / max1;
				return hsv;
			}

			//HSV to RGB  
			fixed3 HSVConvertToRGB(fixed3 hsv)
			{
				fixed R, G, B;
				//float3 rgb;  
				if (hsv.y == 0)
				{
					R = G = B = hsv.z;
				}
				else
				{
					hsv.x = hsv.x / 60.0;
					int i = (int)hsv.x;
					fixed f = hsv.x - (fixed)i;
					fixed a = hsv.z * (1 - hsv.y);
					fixed b = hsv.z * (1 - hsv.y * f);
					fixed c = hsv.z * (1 - hsv.y * (1 - f));
					if (i == 0)
					{
						R = hsv.z; G = c; B = a;
					}
					else if (i == 1)
					{
						R = b; G = hsv.z; B = a;
					}
					else if (i == 2)
					{
						R = a; G = hsv.z; B = c;
					}
					else if (i == 3)
					{
						R = a; G = b; B = hsv.z;
					}
					else if (i == 4)
					{
						R = c; G = a; B = hsv.z;
					}
					else
					{
						R = hsv.z; G = a; B = b;
					}
				}
				return fixed3(R, G, B);
			}
			*/

			fixed4 ComputeLight(fixed3 normalDirection, fixed3 lightColor,fixed3 lightDirection, fixed3 normalDir,
				    fixed3 viewDirection,fixed specPow, fixed attenuation, fixed3 halfDirection,
				    fixed3 giIndirect,fixed4 _node_SelfIllum_var, fixed3 attenColor,fixed2 uv0,bool isArmor, bool hasInDirectLight, bool hasEmissive)
			{
				fixed Pi = 3.141592654;
				fixed InvPi = 0.31830988618;
				fixed NdotL = max(0, dot(normalDirection, lightDirection));

				fixed para = 3-_armor_Specular;
				if (!isArmor)
				{
					para = 1;
				}
				fixed specular = (_node_SelfIllum_var.r*pow(1.0 - max(0, dot(normalDir, viewDirection)), para));
				fixed3 specularColor = fixed3(specular, specular, specular);

				para = 3 - _refle_Specular;
				fixed refle = (_node_SelfIllum_var.g*pow(1.0 - max(0, dot(normalDir, viewDirection)), para));
				fixed specularMonochrome = refle;
				fixed normTerm = (specPow + 8.0) / (8.0 * Pi);
				fixed3 directSpecular = (floor(attenuation) * lightColor.xyz) * pow(max(0, dot(halfDirection, normalDirection)), specPow)*normTerm*specularColor;
				//fixed3 indirectSpecular = giIndirect * _node_SelfIllum_var.g * refle + fixed3(_edge_Intensity, _edge_Intensity, _edge_Intensity)*specularColor;
				fixed3 indirectSpecular = giIndirect * _node_SelfIllum_var.g * refle + specularColor;
				indirectSpecular += fixed3(_edge_Intensity, _edge_Intensity, _edge_Intensity)*specularColor;
				fixed3 specularAll = directSpecular;
				if (hasInDirectLight)
				{
					specularAll += indirectSpecular;
				}

				NdotL = dot(normalDirection, lightDirection);
				fixed3 forwardLight = max(0.0, NdotL);
				fixed3 backLight = max(0.0, -NdotL) * (_backlight_Color.rgb*_backlight_Intensity);
				NdotL = max(0.0, dot(normalDirection, lightDirection));
				fixed3 directDiffuse = (forwardLight + backLight) * attenColor;
				//return directDiffuse;

				//para = 4 - _armor_Intensity;
				para = 4 - _armor_Intensity;
				if (!isArmor)
				{
					para = 1;
				}
				fixed diffuseIntensity = pow(1.0 - max(0, dot(normalDir, viewDirection)), para);
				fixed3 indirectDiffuse = fixed3(0, 0, 0);
				indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
				fixed4 _main_tex_var = tex2D(_main_tex, TRANSFORM_TEX(uv0, _main_tex));
				fixed3 diffuseColor = _main_tex_var.rgb * diffuseIntensity + _node_SelfIllum_var.r;
				diffuseColor *= 1 - specularMonochrome;

				fixed3 diffuse = directDiffuse *diffuseColor;
				if (hasInDirectLight)
				{
					diffuse += indirectDiffuse *diffuseColor;
				}
				
				fixed3 emissive = _main_tex_var.rgb;

				fixed3 returnColor = diffuse + specularAll;
				if (hasEmissive)
				{
					returnColor += emissive;
				}
				
				/*
				if (isArmor && _armor_Colorization.x>0.01 && _armor_Colorization.y>0.01 && _armor_Colorization.z>0.01)
				{
					fixed3 colorFilter_HSV = RGBConvertToHSV(_armor_Colorization);
					fixed3 returnColor_HSV = RGBConvertToHSV(returnColor);
					fixed3 colorMode_HSV = fixed3(colorFilter_HSV.x, colorFilter_HSV.y, returnColor_HSV.z);
					fixed3 colorMode = HSVConvertToRGB(colorMode_HSV);
					returnColor = colorMode;
				}
				*/
				
				return fixed4(returnColor, _main_tex_var.a);
			}

			fixed4 frag(VertexOutput i) : COLOR
			{
                i.normalDir = normalize(i.normalDir);
				fixed3x3 tangentTransform = fixed3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed3 _node_Normal_var = UnpackNormal(tex2D(_node_Normal,TRANSFORM_TEX(i.uv0, _node_Normal)));
                fixed3 normalLocal = _node_Normal_var.rgb;
                fixed3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                fixed3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 lightColor = _LightColor0.rgb;
                fixed3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                fixed attenuation = LIGHT_ATTENUATION(i);
				//fixed attenuation = 0.4;
                fixed3 attenColor = attenuation * _LightColor0.xyz;
				//return fixed4(attenuation, attenuation, attenuation, 1);
				//return fixed4(attenColor, 1);

///////// Gloss:
                fixed4 _node_SelfIllum_var = tex2D(_selfIllum_tex,TRANSFORM_TEX(i.uv0, _selfIllum_tex));
				fixed gloss = _gloss;
                fixed specPow = exp2( gloss );
/////// GI Data:
                UnityGI gi = ComputeUnityGIData(lightColor, lightDirection, normalDirection, 
					i.posWorld, viewDirection, attenuation, viewReflectDirection,gloss);
                //lightDirection = gi.light.dir;
               // lightColor = gi.light.color;
/// Final Color:
				bool isArmor = false;
				if (_node_SelfIllum_var.b > 0.01 && _node_SelfIllum_var.b < 0.99)
				{
					isArmor = true;
				}

				fixed4 finalGlobalColor = ComputeLight(normalDirection, lightColor,lightDirection, i.normalDir,
					viewDirection, specPow, attenuation, halfDirection, gi.indirect.specular, _node_SelfIllum_var, attenColor, i.uv0, isArmor,true, true);
				
                UNITY_APPLY_FOG(i.fogCoord, finalGlobalColor);
				//clip(finalGlobalColor.a - _Cutoff);
                return finalGlobalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

