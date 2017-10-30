// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////


Shader "CameraFilterPack/FX_Glitch2" { 
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
			_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
	}
	SubShader
	{
		Pass
		{
			ZTest Always
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#pragma glsl
#include "UnityCG.cginc"
				uniform sampler2D _MainTex;
			uniform float _TimeX;
			uniform float4 _ScreenResolution;
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f
			{
				half2 texcoord  : TEXCOORD0;
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
			};
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				return OUT;
			}
			float hash( float n )
			{
				return frac( sin(n) * 43812.175489);
			}


			float noise( float2 p ) 
			{
				float2 pi = floor( p );
				float2 pf = frac( p );
				float n = pi.x + 59.0 * pi.y;
				pf = pf * pf * (3.0 - 2.0 * pf);
				return lerp( 
						lerp( hash( n ), hash( n + 1.0 ), pf.x ),
						lerp( hash( n + 59.0 ), hash( n + 1.0 + 59.0 ), pf.x ),
						pf.y );
			}

			float noise( float3 p ) 
			{

				float3 pi = floor( p );
				float3 pf = frac( p );
				float n = pi.x + 59.0 * pi.y + 256.0 * pi.z;
				pf.x = pf.x * pf.x * (3.0 - 2.0 * pf.x);
				pf.y = pf.y * pf.y * (3.0 - 2.0 * pf.y);
				pf.z = sin( pf.z );

				float v1 = 	lerp(
						lerp( hash( n ), hash( n + 1.0 ), pf.x ),
						lerp( hash( n + 59.0 ), hash( n + 1.0 + 59.0 ), pf.x ),
						pf.y );

				float v2 = 	lerp(
						lerp( hash( n + 256.0 ), hash( n + 1.0 + 256.0 ), pf.x ),
						lerp( hash( n + 59.0 + 256.0 ), hash( n + 1.0 + 59.0 + 256.0 ), pf.x ),
						pf.y );

				return lerp( v1, v2, pf.z );
			}

			float4 frag (v2f i) : COLOR 
			{
				float2 uv = i.texcoord.xy;
				uv -= 0.5;

				//float _TimeX = _TimeX / 30.0;
				_TimeX = _TimeX / 30.0;
				_TimeX = 0.5 + 0.5 * sin( _TimeX * 6.238 );
				_TimeX = tex2D( _MainTex, float2(0.5,0.5) ).x; 

				if( _TimeX < 0.2 ) uv *= 1.0;
				else if( _TimeX < 0.4 )
				{
					uv.x += 100.55;
					uv *= 0.00005;
				}
				else if( _TimeX < 0.6 )
				{
					uv *= 0.00045;
				}
				else if( _TimeX < 0.8 ) 
				{
					uv *= 500000.0;
				}
				else if( _TimeX < 1.0 ) 
				{
					uv *= 0.000045;
				}


				float fft = tex2D( _MainTex, float2(uv.x,0.25) ).x; 
				float ftf = tex2D( _MainTex, float2(uv.x,0.15) ).x; 
				float fty = tex2D( _MainTex, float2(uv.x,0.35) ).x; 
				uv *= 200.0 * sin( log( fft ) * 10.0 );

				if( sin( fty ) < 0.5 ) uv.x += sin( fty ) * sin( cos( _TimeX ) + uv.y * 40005.0 ) ;
				uv *= sin( _TimeX * 179.0 );

				float3 p;
				p.x = uv.x;
				p.y = uv.y;
				p.z = sin( 0.0 * _TimeX * ftf );

				float no = noise(p);

				float3 col = float3( 
						hash( no * 6.238  * cos( _TimeX ) ), 
						hash( no * 6.2384 + 0.4 * cos( _TimeX * 2.25 ) ), 
						hash( no * 6.2384 + 0.8 * cos( _TimeX * 0.8468 ) ) );

				float b = dot( uv, uv );
				b *= 10000.0;
				b = b * b;
				col.rgb *= tex2D( _MainTex, i.texcoord.xy ).rgb; 

				return  float4(col,1.0);
			}

			ENDCG
		}
	}
}
