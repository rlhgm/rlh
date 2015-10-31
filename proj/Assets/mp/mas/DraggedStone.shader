Shader "My/DraggedStone"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
			//_BumpMap ("Bumpmap", 2D) = "bump" {}
			[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

			//_Frequency("Clip Frequency", Range(0,10)) = 6
			//_ClipDist("ClipDist", Range(0,1)) = 0.5
			_sx("sx", Range(0,20)) = 1
			_sy("sy", Range(0,20)) = 1
			_speedX("speedX", Range(0,0.3)) = 0
			_speedY("speedY", Range(0,0.3)) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			Blend One OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile DUMMY PIXELSNAP_ON
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;

				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					half2 texcoord  : TEXCOORD0;
					float4 lvertex  : TEXCOORD1;
					//float4 lvertex2  : TEXCOORD2;
				};

				fixed4 _Color;
				float _sx;
				float _sy;
				float _speedX;
				float _speedY;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
					//OUT.lvertex = mul(_Object2World, IN.vertex);
					//OUT.lvertex = mul(UNITY_MATRIX_MV, IN.vertex);
					OUT.lvertex = IN.vertex;
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color *_Color;
				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

					return OUT;
				}

				sampler2D _MainTex;
				float _Frequency;
				float _ClipDist;

				float rand(float3 myVector) {
					return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
				}
				float rand2(float2 co) {
					return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					//fixed2 f2 = fixed2(IN.vertex.xy / _ScreenParams.xy);
					//fixed2 f2 = fixed2(IN.lvertex.xy / _ScreenParams.xy);
					fixed2 f2 = IN.lvertex.xy ;
				//clip(frac((IN.lvertex.x + _Offset) * _Frequency) - 0.5);
				//clip(frac(f2 + _Frequency) - 0.5);
				//clip(frac((_ScreenParams.xy + _Offset) * _Frequency) - 0.5);
				//clip(1);
				//clip(frac((IN.vertex.y + _Offset) * _Frequency) - 0.5);
				//clip(frac(IN.vertex.y)-0.5);

				//frac((IN.vertex.y + _Offset) * _Frequency);

				//fixed4 c = tex2D(_MainTex, IN.texcoord);

				//clip(frac(IN.lvertex.x * 8) - _ClipDist);
				//clip(frac(IN.lvertex.y * 8) - _ClipDist);

				//fixed4 c = fixed4(1,1,1,1);// tex2D(_MainTex, IN.texcoord) * IN.color;
				//float3 r;
				//r.x = _SinTime.w;
				//r.y = _CosTime.w;
				//r.z = 0.30;
				//_t2 = abs(_t2);
				//IN.texcoord.x = clamp(_t2, 0.1, 0.95); //rand2(r2)/10.0f;
				//IN.texcoord.y += rand2(r2)/10.0f;
				//IN.color.b = 1;// rand2(r);
				//IN.texcoord.x = (IN.texcoord.x + _t2 * 0.05);

				float _s = 2.0f;// abs(sin(_Time.y)) * 2.0;
				float _px = abs(sin(_Time.y)) * _speedX;
				float _py = abs(sin(_Time.y)) * _speedY;
				float _t = f2.x / _sx * 3.14159 * _s;// +_Time.y; // *abs(_SinTime.w) * 4;
				float _t2 = sin(_t);
				IN.texcoord.x = (IN.texcoord.x + _t2 * _px);
				IN.texcoord.x = clamp(IN.texcoord.x, 0.0, 1.0); //rand2(r2)/10.0f;
				//IN.color.r = abs(_t2); //clamp(_t2, 0.0, 1.0);

				_t = f2.y / _sy * 3.14159 * _s;// +_Time.y; // *abs(_SinTime.w) * 4;
				_t2 = sin(_t);
				IN.texcoord.y = (IN.texcoord.y + _t2 * _py);
				IN.texcoord.y = clamp(IN.texcoord.y, 0.0, 1.0); //rand2(r2)/10.0f;
				//IN.color.g = abs(_t2); // IN.lvertex.y / 255.0; // IN.lvertex.y; //rand(r);

				//IN.color.r = _t2; // f2.x; //IN.lvertex.x / 255.0; //rand(r);
				//if (IN.color.r > 1) IN.color.r = 1;
				
				
				//IN.color.b = 0.5;
				//IN.color.a = 1;

				
				//IN.texcoord.x += sin(IN.texcoord.x) / 10;// _SinTime.w / 10; // (rand2(r) - 0.5) / 100; //0.0; // 0.1;// rand(r);
				//IN.texcoord.y = rand(r);
				
				//IN.texcoord.x += rand2(r);// _SinTime.w / 10; // (rand2(r) - 0.5) / 100; //0.0; // 0.1;// rand(r);
				//IN.texcoord.y += rand2(r) / 10;

				//IN.color.r = rand2(r);// IN.lvertex.x; // f2.x;
				//IN.color.g = 1;// IN.lvertex.y; // f2.y;
				//IN.color.b = 1;

				//IN.texcoord.x = f2.x * f2.y;
				//IN.texcoord.y = f2.y * f2.y;

				//IN.texcoord.x += sin(IN.texcoord.x / 3.14159) ;

				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				//fixed4 c = IN.color;

				//c.rgb *= _Offset;
				//c.r = abs(IN.lvertex.x); // IN.vertex.x;
				//c.g = frac(IN.lvertex.y); // IN.vertex.x;
				//c.g = 0.0; // frac(IN.vertex.y);
				//c.r = 0.0; // IN.vertex.x / _ScreenParams.x;
				//c.r = IN.lvertex.y / _ScreenParams.y;
				//c.b = 0.0;
				c.rgb *= c.a;
				return c;

				//return fixed4(IN.vertex.xy / _ScreenParams.xy,0.0,1.0);

				//float2 wcoord = IN.vertex.xy / _ScreenParams.xy;
				//float vig = clamp(3.0*length(wcoord - 0.5),0.0,1.0);
				//return lerp(fixed4(wcoord,0.0,1.0),fixed4(0.3,0.3,0.3,1.0),vig);
			}
			ENDCG
		}
		}
}
