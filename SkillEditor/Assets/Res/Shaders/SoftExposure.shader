// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/SoftExposure" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Bloom ("Bloom (RGB)", 2D) = "black" {}
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _Bloom;
				
		uniform half4 _MainTex_TexelSize;
		half4 _MainTex_ST;
		
		uniform half4 _Parameter;
		uniform half4 _OffsetsA;
		uniform half4 _OffsetsB;

		struct v2f_simple 
		{
			float4 pos : SV_POSITION; 
			half2 uv : TEXCOORD0;

        #if UNITY_UV_STARTS_AT_TOP
				half2 uv2 : TEXCOORD1;
		#endif
		};	
		
		v2f_simple vertBloom ( appdata_img v )
		{
			v2f_simple o;
			
			o.pos = UnityObjectToClipPos (v.vertex);
        	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
        	
        #if UNITY_UV_STARTS_AT_TOP
        	o.uv2 = o.uv;
        	if (_MainTex_TexelSize.y < 0.0)
        		o.uv.y = 1.0 - o.uv.y;
        #endif
        	        	
			return o; 
		}

		struct v2f_tap
		{
			float4 pos : SV_POSITION;
			half2 uv20 : TEXCOORD0;
			half2 uv21 : TEXCOORD1;
			half2 uv22 : TEXCOORD2;
			half2 uv23 : TEXCOORD3;
		};			

		v2f_tap vert4Tap ( appdata_img v )
		{
			v2f_tap o;

			o.pos = UnityObjectToClipPos (v.vertex);
        	o.uv20 = UnityStereoScreenSpaceUVAdjust(v.texcoord + _MainTex_TexelSize.xy, _MainTex_ST);
			o.uv21 = UnityStereoScreenSpaceUVAdjust(v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h,-0.5h), _MainTex_ST);
			o.uv22 = UnityStereoScreenSpaceUVAdjust(v.texcoord + _MainTex_TexelSize.xy * half2(0.5h,-0.5h), _MainTex_ST);
			o.uv23 = UnityStereoScreenSpaceUVAdjust(v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h,0.5h), _MainTex_ST);

			return o; 
		}					
						
		fixed4 fragBloom ( v2f_simple i ) : SV_Target
		{	
			float4 blurred = tex2D(_Bloom, i.uv);
			float4 src = tex2D(_MainTex, i.uv2);
			float3 mixed = blurred * _Parameter.y + src * (1- _Parameter.y);
			return float4(mixed, 1);
		} 
		
		fixed4 fragDownsample ( v2f_tap i ) : SV_Target
		{				
			fixed4 color = tex2D (_MainTex, i.uv20);
			color += tex2D (_MainTex, i.uv21);
			color += tex2D (_MainTex, i.uv22);
			color += tex2D (_MainTex, i.uv23);
			return color / 4;
		}
	
		// weight curves

		static const half curve[7] = { 0.0205, 0.0855, 0.232, 0.324, 0.232, 0.0855, 0.0205 };  // gauss'ish blur weights

		static const half4 curve4[7] = { half4(0.0205,0.0205,0.0205,0), half4(0.0855,0.0855,0.0855,0), half4(0.232,0.232,0.232,0),
			half4(0.324,0.324,0.324,1), half4(0.232,0.232,0.232,0), half4(0.0855,0.0855,0.0855,0), half4(0.0205,0.0205,0.0205,0) };

		struct v2f_withBlurCoords8 
		{
			float4 pos : SV_POSITION;
			half4 uv : TEXCOORD0;
			half2 offs : TEXCOORD1;
		};	
		
		struct v2f_withBlurCoordsSGX 
		{
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half4 offs[3] : TEXCOORD1;
		};

		v2f_withBlurCoords8 vertBlurHorizontal (appdata_img v)
		{
			v2f_withBlurCoords8 o;
			o.pos = UnityObjectToClipPos (v.vertex);
			
			o.uv = half4(v.texcoord.xy,1,1);
			o.offs = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _Parameter.x;

			return o; 
		}
		
		v2f_withBlurCoords8 vertBlurVertical (appdata_img v)
		{
			v2f_withBlurCoords8 o;
			o.pos = UnityObjectToClipPos (v.vertex);
			
			o.uv = half4(v.texcoord.xy,1,1);
			o.offs = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _Parameter.x;
			 
			return o; 
		}	

		half4 fragBlur8 ( v2f_withBlurCoords8 i ) : SV_Target
		{
			half2 uv = i.uv.xy; 
			half2 netFilterWidth = i.offs;  
			half2 coords = uv - netFilterWidth * 3.0;  
			
			half4 color = 0;
  			for( int l = 0; l < 7; l++ )  
  			{   
				half4 tap = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(coords, _MainTex_ST));
				color += tap * curve4[l];
				coords += netFilterWidth;
  			}
			return color;
		}

	ENDCG
	
	SubShader {
	  ZTest Off Cull Off ZWrite Off Blend Off
	  
	// 0
	Pass {
	
		CGPROGRAM
		#pragma vertex vertBloom
		#pragma fragment fragBloom
		
		ENDCG
		 
		}

	// 1
	Pass { 
	
		CGPROGRAM
		
		#pragma vertex vert4Tap
		#pragma fragment fragDownsample
		
		ENDCG
		 
		}

	// 2
	Pass {
		ZTest Always
		Cull Off
		
		CGPROGRAM 
		
		#pragma vertex vertBlurVertical
		#pragma fragment fragBlur8
		
		ENDCG 
		}	
		
	// 3	
	Pass {		
		ZTest Always
		Cull Off
				
		CGPROGRAM
		
		#pragma vertex vertBlurHorizontal
		#pragma fragment fragBlur8
		
		ENDCG
		}	
	}
	FallBack Off
}
