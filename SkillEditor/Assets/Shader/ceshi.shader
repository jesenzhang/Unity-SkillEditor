// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:True,enco:True,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:7595,x:32750,y:32471,varname:node_7595,prsc:2|diff-6139-OUT,spec-6128-OUT,gloss-71-OUT,normal-7948-RGB,emission-452-OUT,transm-9260-OUT,amspl-1868-OUT;n:type:ShaderForge.SFN_Tex2d,id:7389,x:32142,y:32255,ptovrint:False,ptlb:main_tex,ptin:_main_tex,varname:_main_tex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b70fbfde1606ec041b602d0e1c4d0d4f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:2865,x:31322,y:32270,ptovrint:False,ptlb:gloss,ptin:_gloss,varname:_gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3766829,max:1;n:type:ShaderForge.SFN_Tex2d,id:7940,x:31755,y:32094,ptovrint:False,ptlb:node_7940,ptin:_node_7940,varname:_node_7940,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:bdc6e78db91c5254389a5a6ddaaf9801,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7948,x:31548,y:32535,ptovrint:False,ptlb:node_7948,ptin:_node_7948,varname:_node_7948,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8e77b3d94c58b0e4c9d5942b14eb7168,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Fresnel,id:6374,x:31967,y:33148,varname:node_6374,prsc:2|NRM-2596-OUT,EXP-8162-OUT;n:type:ShaderForge.SFN_Slider,id:8162,x:31508,y:33267,ptovrint:False,ptlb:bian,ptin:_bian,varname:_bian,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10,max:10;n:type:ShaderForge.SFN_NormalVector,id:2596,x:31726,y:33098,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:9515,x:31548,y:32400,ptovrint:False,ptlb:Spec,ptin:_Spec,varname:_Spec,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7968687,max:3;n:type:ShaderForge.SFN_Multiply,id:6128,x:32365,y:32599,varname:node_6128,prsc:2|A-7940-B,B-3652-OUT;n:type:ShaderForge.SFN_Multiply,id:71,x:32014,y:32443,varname:node_71,prsc:2|A-7940-B,B-2865-OUT;n:type:ShaderForge.SFN_Color,id:765,x:31548,y:32795,ptovrint:False,ptlb:node_765,ptin:_node_765,varname:_node_765,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9260,x:32294,y:32895,varname:node_9260,prsc:2|A-765-RGB,B-6374-OUT;n:type:ShaderForge.SFN_Fresnel,id:3652,x:31995,y:32674,varname:node_3652,prsc:2|NRM-2566-OUT,EXP-9515-OUT;n:type:ShaderForge.SFN_NormalVector,id:2566,x:31729,y:32604,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:1868,x:31838,y:32947,ptovrint:False,ptlb:node_1868,ptin:_node_1868,varname:_node_1868,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Add,id:6139,x:32428,y:32393,varname:node_6139,prsc:2|A-7389-RGB,B-6374-OUT;n:type:ShaderForge.SFN_Color,id:2388,x:32065,y:33386,ptovrint:False,ptlb:bianse,ptin:_bianse,varname:_bianse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Blend,id:8992,x:33595,y:32297,varname:node_8992,prsc:2,blmd:10,clmp:True;n:type:ShaderForge.SFN_Multiply,id:452,x:32328,y:33040,varname:node_452,prsc:2|A-7940-R,B-2388-RGB;proporder:7389-7940-2865-7948-8162-9515-765-1868-2388;pass:END;sub:END;*/

Shader "Instanced/ceshi" {
    Properties {
        _main_tex ("main_tex", 2D) = "white" {}
        _node_SelfIllum ("_node_SelfIllum", 2D) = "white" {}
        _gloss ("gloss", Range(0, 1)) = 0.3766829
        _node_Normal ("node_7948", 2D) = "bump" {}
		_bian("bian", Range(0, 4)) = 0.9496132
        _Spec ("Spec", Range(0, 3)) = 0.7968687
		_buse("buse", Color) = (0.5,0.5,0.5,1)
        _node_1868 ("node_1868", Range(0, 2)) = 2
        _bianse ("bianse", Color) = (1,0,0,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _main_tex; uniform float4 _main_tex_ST;
            uniform float _gloss;
            uniform sampler2D _node_SelfIllum; uniform float4 _node_SelfIllum_ST;
            uniform sampler2D _node_Normal; uniform float4 _node_Normal_ST;
            uniform float _bian;
            uniform float _Spec;
            uniform float4 _buse;
            uniform float _node_1868;
            uniform float4 _bianse;
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
			
			float3 ComputeLight(float3 normalDirection, float3 lightColor,float3 lightDirection, float3 normalDir,
				    float3 viewDirection,float specPow, float attenuation, float3 halfDirection,
				    float3 giIndirect,float4 _node_SelfIllum_var, float3 attenColor,float2 uv0,bool isSkin, bool hasInDirectLight, bool hasEmissive)
			{
				float Pi = 3.141592654;
				float InvPi = 0.31830988618;
				float NdotL = max(0, dot(normalDirection, lightDirection));
				float node_2805 = (_node_SelfIllum_var.g*_node_1868);
				float node_6128 = (_node_SelfIllum_var.b*pow(1.0 - max(0, dot(normalDir, viewDirection)), _Spec));
				float3 specularColor = float3(node_6128, node_6128, node_6128);
				float specularMonochrome = max(max(specularColor.r, specularColor.g), specularColor.b);
				float normTerm = (specPow + 8.0) / (8.0 * Pi);
				float3 directSpecular = (floor(attenuation) * lightColor.xyz) * pow(max(0, dot(halfDirection, normalDirection)), specPow)*normTerm*specularColor;
				float3 indirectSpecular = (giIndirect + float3(node_2805, node_2805, node_2805))*specularColor;
				float3 specular = directSpecular;
				if (hasInDirectLight)
				{
					specular += indirectSpecular;
				}

				NdotL = dot(normalDirection, lightDirection);
				float3 forwardLight = max(0.0, NdotL);
				float para = _bian;
				if (isSkin)
				{
					para = 1;
				}
				float node_6374 = pow(1.0 - max(0, dot(normalDir, viewDirection)), para);
				float3 backLight = max(0.0, -NdotL) * (_buse.rgb*node_6374);
				NdotL = max(0.0, dot(normalDirection, lightDirection));
				float3 directDiffuse = (forwardLight + backLight) * attenColor;
				float3 indirectDiffuse = float3(0, 0, 0);
				indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
				float4 _main_tex_var = tex2D(_main_tex, TRANSFORM_TEX(uv0, _main_tex));
				float3 diffuseColor = (_main_tex_var.rgb + node_6374  + (_node_SelfIllum_var.r*_bianse.rgb) + _node_SelfIllum_var.g);
				diffuseColor *= 1 - specularMonochrome;

				float3 diffuse = directDiffuse *diffuseColor;
				if (hasInDirectLight)
				{
					diffuse += indirectDiffuse *diffuseColor;
				}
				
				float3 emissive = _main_tex_var.rgb;

				//return _node_SelfIllum_var.g;
				if (hasEmissive)
				{
					return diffuse + specular + emissive;
				}
				else
				{
					return diffuse + specular;
				}
				
			}
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _node_Normal_var = UnpackNormal(tex2D(_node_Normal,TRANSFORM_TEX(i.uv0, _node_Normal)));
                float3 normalLocal = _node_Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;

///////// Gloss:
                float4 _node_SelfIllum_var = tex2D(_node_SelfIllum,TRANSFORM_TEX(i.uv0, _node_SelfIllum));
				float gloss = _gloss;
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityGI gi = ComputeUnityGIData(lightColor, lightDirection, normalDirection, 
					i.posWorld, viewDirection, attenuation, viewReflectDirection,gloss);
                //lightDirection = gi.light.dir;
               // lightColor = gi.light.color;
/// Final Color:
				bool isSkin = false;
				if (_node_SelfIllum_var.b == 0)
				{
					isSkin = true;
				}


				float3 finalGlobalColor = ComputeLight(normalDirection, lightColor,lightDirection, i.normalDir,
					viewDirection, specPow, attenuation, halfDirection, gi.indirect.specular, _node_SelfIllum_var, attenColor, i.uv0, isSkin,true, true);
				fixed4 finalGlobalRGBA = fixed4(finalGlobalColor, 1);

				float3 finalLocalColor = ComputeLight(normalDirection, lightColor,lightDirection, i.normalDir,
					viewDirection, specPow, attenuation, halfDirection, gi.indirect.specular, _node_SelfIllum_var, attenColor, i.uv0, isSkin,false,false);
				fixed4 finalLocalRGBA = fixed4(finalLocalColor * 1, 0);

				//float4 finalRGBA = saturate(finalGlobalRGBA +finalLocalRGBA);
				float4 finalRGBA = finalGlobalRGBA;
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
			
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
