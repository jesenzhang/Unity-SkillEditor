// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "EngineTA/PF/MatCapPlayer_UnLit_Double" {
	Properties{
		_MainTex("漫反射 (RGB)", 2D) = "white" {}
		_NormalMap("法线" , 2D) = "bump" {}
		_MaskTex("遮罩图 (RG) R-反射区域 G-高光", 2D) = "white" {}
		_MatCap("光照图 (RGB)", 2D) = "white" {}
		_OutLitColor("边缘光颜色", Color) = (1,1,1,1)
		_OutLitPow("边缘光强度", Range(0,10.0)) = 4
		_LightDir("主光源方向",Vector)=(0.0,0.0,0.0,1.0)
		_AmbeintColor("环境色", Color) = (1,1,1,1)
		_SpecColor("高光颜色", Color) = (1,1,1,1)
		_SpecPower("高光强度", Range(0.01,2.0)) =0.28
		_SpecMultiplier("高光系数", Range(0,5.0)) = 1.67
		_AmbeintPow("明度", Range(0,1.0)) = 0.2
		_ChangeColor("变色", Color) = (1,1,1,1)
		_RefPower("反射强度", Range(0.0,1.5)) = 1.0

		


	}
		Subshader{
			Tags{ "RenderType" = "Transparent"  } //"Queue" = "Transparent"
			//Fog { Color [_AddFog] }

			Pass{
			Name "BASE"
			Tags{ "LightMode" = "Always" }
			Cull Off
			Blend SrcAlpha  OneMinusSrcAlpha			
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members tan1,tan2)

#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_fog_exp2
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

			struct v2f
		{
			float4 pos    : SV_POSITION;
			float2 uv     : TEXCOORD0;
			float3 tV     : TEXCOORD1;
			float3 tL     : TEXCOORD2;
			float3 tan1   : TEXCOORD3;
			float3 tan2   : TEXCOORD4;
			
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _NormalMap;
		uniform sampler2D _MatCap;
		uniform sampler2D _MaskTex;
		uniform half4 _SpecColor;
		uniform half _SpecPower;
		uniform half _SpecMultiplier;		
		uniform half _AmbeintPow;
		uniform half4 _ChangeColor;
		uniform half _OutLitPow;
		uniform half4 _OutLitColor;
		uniform half _RefPower;
		uniform half4 _LightDir;
		uniform half4 _AmbeintColor;

		

		v2f vert(appdata_full v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			

			TANGENT_SPACE_ROTATION;
			o.tV = mul(rotation, ObjSpaceViewDir(v.vertex));
			float3 Litdir = normalize(mul((float3x3)unity_ObjectToWorld, _LightDir.xyz));
			o.tL = mul(rotation, Litdir);
			o.tan1 = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz);
			o.tan2 = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz);

			
			
			

			return o;
		}

		

		fixed4 frag(v2f i) : COLOR
		{
			fixed4 tex = tex2D(_MainTex, i.uv);
			

		
			fixed4 mask = tex2D(_MaskTex, i.uv);
			//NormalMap
			float4 normalMap = tex2D(_NormalMap, i.uv);
			float3 normalDir = normalize(UnpackNormal(normalMap));
			float  dif = max(0, dot(-i.tL, normalDir));
			dif = lerp(_AmbeintPow, 0.8, dif);
		    //matcap UV & matcap Color-------------
			float2 matCapUV;
			matCapUV.x = dot(i.tan1, normalDir);
			matCapUV.y = dot(i.tan2, normalDir);
			matCapUV = matCapUV * 0.5 + 0.5;
			matCapUV /= 2;
			float2 matCapUV2=matCapUV;
			matCapUV.y += 0.5;
			matCapUV2+=float2(0.5,0.5);
			
			//-------------------------------------
			fixed4 mc = tex2D(_MatCap, matCapUV);
			fixed4 mc2 = tex2D(_MatCap, matCapUV2);
			tex.rgb = lerp(tex.rgb, tex.rgb*_ChangeColor.rgb, mask.z);
			//反射效果
			mc2 = lerp(tex, mc2, mask.x*_RefPower);

			tex.xyz = tex.xyz *((mc2 * 2).xyz + 1)+mc*_OutLitPow*_OutLitColor;
			tex.xyz = lerp(tex.xyz, mc*_OutLitPow*_OutLitColor, mc);

			float nh = max(0, normalize(i.tL + i.tV).z);
			//明暗过渡色uv-------------------------
			float2 rampuv;
			rampuv.x = i.tL.z * 0.5 + 0.5;
			rampuv.y = 0.25;
			rampuv *= 0.5;
			fixed3 ramp = tex2D(_MatCap, rampuv).xyz;
			//------------------------------------

			fixed3 spec = _SpecColor * pow(nh, _SpecPower) * mask.y *_SpecMultiplier;
			fixed3 c = (ramp + _AmbeintColor.xyz) * tex.rgb;
			//fixed3 c = (ramp + _AmbeintColor.xyz) * tex;
			
			return fixed4((c + spec)*dif, tex.a);
		}
			ENDCG
		}
		}
			Fallback "Diffuse"
}
