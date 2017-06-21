#ifndef CARTOON_CGINC
#define CARTOON_CGINC
    sampler2D _Ramp;
	fixed4 _LightDir;
	fixed4 _LightColor;
	fixed _LightIntensity;
	

	//normal map
	#if defined(NORMAL_MAP)
		sampler2D _NormalMap;
	#endif
	
	//specular
	#if defined(SPEC)
		fixed _SpecPower;
		fixed _SpecIntensity;
		// fixed4 _SpecColor;
		sampler2D _SpecMask;
	#endif

    half4 LightingRamp (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
		half4 lightColor = _LightColor;
		half lightIntensity = _LightIntensity;
		#if defined(UNITY_LIGHT)
			lightColor = _LightColor0;
		#else
			lightDir = normalize(_LightDir);
		#endif
		
        half NdotL = dot (s.Normal, normalize(lightDir.xyz));
        half diff = NdotL * 0.5 + 0.5;
		half ne = dot(s.Normal,viewDir) * 0.5 + 0.5;
        half3 ramp = tex2D (_Ramp, float2(diff,ne)).rgb;
        half4 c;
        c.rgb = s.Albedo * lightIntensity * lightColor * ramp * (atten );
        c.a = s.Alpha;
		
		//ambient.
		c.rgb += c.rgb * UNITY_LIGHTMODEL_AMBIENT;
		
		//spec
		#if defined(SPEC)
			half3 h = normalize(lightDir + viewDir);
			half nh = max(0,dot(s.Normal,h));
			c.rgb += _SpecColor.rgb *_SpecIntensity * pow(nh,_SpecPower*128) * s.Specular;
		#endif
		
        return c;
    }

    struct Input {
        float2 uv_MainTex;
		float3 viewDir;
    };
	
    sampler2D _MainTex;
    float4 _Color;
	float4 _RimColor;
	float _RimPower;
	
    void surf (Input IN, inout SurfaceOutput o) {
		fixed4 texColor = tex2D (_MainTex, IN.uv_MainTex);
        o.Albedo = texColor.rgb * _Color;
		o.Alpha = texColor.a;
		
		#if defined(NORMAL_MAP)
			o.Normal = UnpackNormal(tex2D(_NormalMap,IN.uv_MainTex));
		#endif
		
		half rim = 1 - saturate(dot(IN.viewDir,o.Normal));
		o.Emission = _RimColor.rgb * (rim * _RimPower);
		
		// spec mask.
		#if defined(SPEC)
		fixed4 specMask = tex2D(_SpecMask,IN.uv_MainTex);
		o.Specular = specMask.r;
		#endif
    }
#endif //end cartoon_cginc