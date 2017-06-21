//2016_01_13//
//CYED_TAShader_VegetationBend_Leaf_Surface By KK//

Shader "Transparent/TAShaders/VegetationBend_Leaf" {
	Properties {
		_Color("主色调", Color) = (0,0,0,0)
		_MainTex ("固有色(RGB) 透明(A)", 2D) = "white" {}
		_Cutoff ("透明范围", float) = 0.5
		
		_WindDir("风向(XYZ) 风力(W)", Vector) = (0.1,0.0,0.05,0.3)
		_Frequency("摇摆频率", Range (0, 20)) = 1.2
	}
	SubShader {
		Tags {"RenderType"="TransparentCutout" "IgnoreProjector"="True" "Queue"="AlphaTest"}
		Cull Off
		LOD 400
		
		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff vertex:vert halfasview noforwardadd
		sampler2D _MainTex;
		half4 _WindDir;
		half  _Frequency;
		half _Offset;
		half4 _Color;
		
		struct Input
        {
            float2 uv_MainTex;
        };
		
		void vert (inout appdata_full v)
		{
			half power = _WindDir.w;
			//worldPos.xyz += _WindDir.xyz * (sin(worldPos.x + _Frequency * _Time.z) + sin(worldPos.z + _Frequency * 0.43 * _Time.z) + 1) * power * v.color.x;
			v.vertex.xyz += (half3(1.0,0.5,0.5) + v.normal) * _WindDir.xyz * (cos(v.vertex.x + _Frequency * _Time.z) + sin(v.vertex.z + _Frequency * 0.4 * _Time.z)) * power * v.color.x;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
		 	o.Alpha = c.a;
		}
		ENDCG
}
	FallBack "Transparent/Cutout/VertexLit"
}
