// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "TLStudio/FX/Water" { 
Properties {
	_WaveScale ("Wave scale", Range (0.02,0.15)) = 0.063
	_ReflDistort ("Reflection distort", Range (0,1.5)) = 0.44
	_RefrDistort ("Refraction distort", Range (0,1.5)) = 0.40
	_RefrColor ("Refraction color", COLOR)  = ( .34, .85, .92, 1)
	_Fresnel ("Fresnel (A) ", 2D) = "gray" {}
	_BumpMap ("Normalmap ", 2D) = "bump" {}
	WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
	_ReflectiveColor ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
	//_ReflectiveColorCube ("Reflective color cube (RGB) fresnel (A)", Cube) = "" { TexGen CubeReflect }
	_MainTex ("Fallback texture", 2D) = "" {}
	_ReflectionTex ("Internal Reflection", 2D) = "" {}
	_RefractionTex ("Internal Refraction", 2D) = "" {}
	_CubeWaterTexture ("CubeWaterTexture", 2D) = "white" {}
	_CubeReflection ("CubeReflection", Cube) = "_Skybox" {}
	_CubeWaveSpeed ("CubeWaterSpeed", Vector) = (0,0,0,0)
	_CubeRelDistortion ("CubeRelDistortion", Float ) = 0
	_CubeCubeAdd ("CubeCubeAdd", Range (0, 5) ) = 0
	_CubeAmbientColor ("CubeAmb Color", Color) = (1.0, 1.0, 1.0, 1.0)
}


// -----------------------------------------------------------
// Fragment program cards


Subshader { 
LOD 200
	Tags { 
		"WaterMode"="Ref" 
		"Queue"="Geometry"
		"RenderType"="Opaque" 
	}
	
	Pass {
	
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest 
	#pragma multi_compile WATER_REFRACTIVE WATER_REFLECTIVE WATER_SIMPLE WATER_CUBE

	#if defined (WATER_REFLECTIVE) || defined (WATER_REFRACTIVE)
		#define HAS_REFLECTION 1
	#endif

	#if defined (WATER_REFRACTIVE)
		#define HAS_REFRACTION 1
	#endif


	#include "UnityCG.cginc"
	uniform float4 _WaveScale4;
	uniform float4 _WaveOffset;

	#if HAS_REFLECTION
		uniform float _ReflDistort;
	#endif
	
	#if HAS_REFRACTION
		uniform float _RefrDistort;
	#endif


	#if defined (WATER_CUBE)
		uniform float4	_CubeWaterTexture_ST;
		uniform float4 	_BumpMap_ST;
		uniform float4 	_CubeWaveSpeed;
		uniform fixed 	_CubeRelDistortion;
		uniform fixed 	_CubeCubeAdd;
		uniform float4 	_CubeAmbientColor;
	#endif

	struct appdata 
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 texcoord0 : TEXCOORD0;
	};

	struct v2f 
	{
		float4 pos : SV_POSITION;

		#if defined(HAS_REFLECTION) || defined(HAS_REFRACTION)
			float4 ref : TEXCOORD0;
			float2 bumpuv0 : TEXCOORD1;
			float2 bumpuv1 : TEXCOORD2;
			float3 viewDir : TEXCOORD3;
		#endif

		#if defined (WATER_SIMPLE)
			half2 WaterUV0:TEXCOORD0;
		#endif

		#if defined (WATER_CUBE)
			float2 uv0 : TEXCOORD0;
			float4 posWorld : TEXCOORD1;
			float3 normalDir : TEXCOORD2;
		#endif
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		
		#if defined(HAS_REFLECTION) || defined(HAS_REFRACTION)
			float4 temp;
			temp.xyzw = v.vertex.xzxz * _WaveScale4 / 1.0 + _WaveOffset;
			o.bumpuv0 = temp.xy;
			o.bumpuv1 = temp.wz;
			o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
			o.ref = ComputeScreenPos(o.pos);
		#endif
		
		#if defined (WATER_SIMPLE)
			half2 Tempuv = v.vertex.xz*_WaveScale4.x;
			half2 t = frac(_Time.x);
			o.WaterUV0=  Tempuv +t;
		#endif

		#if defined (WATER_CUBE)
			o.uv0 = v.texcoord0;
			o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
			o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		#endif

		return o;
	}

	#if defined (WATER_REFLECTIVE) || defined (WATER_REFRACTIVE)
		sampler2D _ReflectionTex;
		sampler2D _BumpMap;
		sampler2D _Fresnel;
		sampler2D _RefractionTex;
		uniform float4 _RefrColor;
	#endif

	#if defined (WATER_REFLECTIVE)
		sampler2D _ReflectiveColor;
	#endif

	#if defined (WATER_SIMPLE)
		sampler2D _MainTex;
	#endif

	#if defined (WATER_CUBE)
		uniform sampler2D _CubeWaterTexture; 
		sampler2D _BumpMap;
		uniform sampler2D _CubeNormal; 
		uniform samplerCUBE _CubeReflection;
	#endif


	half4 frag( v2f i ) : SV_Target
	{
		#if defined (WATER_REFLECTIVE) || defined (WATER_REFRACTIVE)
			i.viewDir = normalize(i.viewDir);
			half3 bump1 = UnpackNormal(tex2D( _BumpMap, i.bumpuv0 )).rgb;
			half3 bump2 = UnpackNormal(tex2D( _BumpMap, i.bumpuv1 )).rgb;
			half3 bump = (bump1 + bump2) * 0.5;
			// fresnel factor
			half fresnelFac = dot( i.viewDir, bump );
			// perturb reflection/refraction UVs by bumpmap, and lookup colors
			float4 uv1 = i.ref; uv1.xy += bump * _ReflDistort;
		#endif

		#if HAS_REFLECTION
			half4 refl = tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(uv1) );
		#endif
	
		#if HAS_REFRACTION
			float4 uv2 = i.ref; uv2.xy -= bump * _RefrDistort;
			half4 refr = tex2Dproj( _RefractionTex, UNITY_PROJ_COORD(uv2) ) * _RefrColor;
		#endif
		
		// final color is between refracted and reflected based on fresnel	
		half4 color;
		
		#if defined(WATER_REFRACTIVE)
			half fresnel = UNITY_SAMPLE_1CHANNEL( _Fresnel, float2(fresnelFac,fresnelFac) );
			color = lerp( refr, refl, fresnel );
		#endif
		
		#if defined(WATER_REFLECTIVE)
			half4 water = tex2D( _ReflectiveColor, float2(fresnelFac,fresnelFac) );
			color.rgb = lerp( water.rgb, refl.rgb, water.a );
			color.a = refl.a * water.a;
		#endif
		
		#if defined(WATER_SIMPLE)
			half4 water0 = tex2D( _MainTex, i.WaterUV0);
			color = water0;
		#endif
		

		#if defined(WATER_CUBE)
			i.normalDir = normalize(i.normalDir);
			float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
			float3 normalDirection = i.normalDir;
			float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
			float4 Timer = _Time;
			float2 uv1 = i.uv0+_CubeWaveSpeed.xy*Timer.r;
			float2 uv2 = i.uv0+_CubeWaveSpeed.zw*Timer.r;
			half3 _UVOffset1 = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(uv1, _BumpMap)));
			half3 _UVOffset2 = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(uv2, _BumpMap)));
			half3 _UVOffset = (_UVOffset1+_UVOffset2)*0.5;
			float2 uv3 = i.uv0+_UVOffset.rb*_CubeRelDistortion;
			fixed4 _WaterTexture_var = tex2D(_CubeWaterTexture,TRANSFORM_TEX(uv3, _CubeWaterTexture));
			float3 diffuse = (_CubeAmbientColor.rgb) * (_WaterTexture_var.rgb +texCUBE( _CubeReflection, normalize( (_UVOffset.rgb - 0.5) *_CubeRelDistortion + viewReflectDirection) ).rgb * _CubeCubeAdd );
			float3 finalColor = diffuse;
			color = fixed4(finalColor,1);
		#endif
		return color;
	}
ENDCG

	}
}

// -----------------------------------------------------------
//  Old cards

// three texture, cubemaps
//Subshader {
//LOD 200
//	Tags { "WaterMode"="Simple" "RenderType"="Opaque" }
//	Pass {
//		Color (0.5,0.5,0.5,0.5)
//		SetTexture [_MainTex] {
//			Matrix [_WaveMatrix]
//			combine texture * primary
//		}
//		SetTexture [_MainTex] {
//			Matrix [_WaveMatrix2]
//			combine texture * primary + previous
//		}
//		SetTexture [_ReflectiveColorCube] {
//			combine texture +- previous, primary
//			Matrix [_Reflection]
//		}
//	}
//}

// single texture
Subshader {
LOD 100
	Tags { "WaterMode"="Simple" "RenderType"="Opaque" }
	Pass {
		Color (0.5,0.5,0.5,0)
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix]
			combine texture, primary
		}
	}
}


}
