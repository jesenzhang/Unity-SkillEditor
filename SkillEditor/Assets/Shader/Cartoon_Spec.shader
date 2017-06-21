Shader "Custom/Character/Cartoon_Spec" {
	Properties {
		//--- base params
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Ramp("Ramp",2d)=""{}
		//--- light params
		_LightColor("Light Color" , Color) = (1,1,1,1)
		_LightIntensity("Light Intensity",float) = 1
		_LightDir("Light Direction" , Vector) = (-0.17,10.07,5.06,0)
		//--- rim params
		_RimColor("RimColor",color) = (1,1,1,1)
		_RimPower("RimPower",Range(0.01,0.5)) = .2
		// normal
		_NormalMap("NormalMap",2d) = ""{}
		//spec
		_SpecPower("Spec Power",range(0.01,1)) = 1
		_SpecIntensity("Spec Intensity",float) = 1
		_SpecColor("Spec Color",Color) = (1,1,1,1)
		_SpecMask("Spec Mask,(R channel used)",2d) = ""{} // use r.
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Ramp noforwardadd nolightmap nodirlightmap exclude_path:prepass novertexlights  approxview halfasview
		#pragma debug
		#define NORMAL_MAP
		#define SPEC
		// #define UNITY_LIGHT
		#include "Cartoon.cginc"
		
		ENDCG
	}
	FallBack "Mobile/Diffuse"
}
