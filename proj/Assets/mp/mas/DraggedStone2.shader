Shader "My/DraggedStone2"
{
	Properties
	{
		//Color beamOriginColor = new Color(0f, 23f / 255f, 1f, 200f / 255f);
		//Color beamTargetColor = new Color(0f, 23f / 255f, 1f, 200f / 255f);


		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (0,0.1,1,0.79)
			_BeamColor("_BeamColor", Color) = (1,1,1,1)
			//_BumpMap ("Bumpmap", 2D) = "bump" {}
			[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

			//_Frequency("Clip Frequency", Range(0,10)) = 6
			//_ClipDist("ClipDist", Range(0,1)) = 0.5
			_sx("sx", Range(0,20)) = 1
			_sy("sy", Range(0,20)) = 1
			_rpx("rpx", Range(-20,20)) = 1
			_rpy("rpy", Range(-20,20)) = 1
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
				fixed4 _BeamColor;
				float _sx;
				float _sy;
				float _rpx;
				float _rpy;

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
				
				float rand(float3 myVector) {
					return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
				}
				float rand2(float2 co) {
					return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed2 f2 = IN.lvertex.xy ;
				
				fixed2 center = fixed2(_rpx, _rpy);
				float _d = length(center - f2);
				//float _t = abs(_SinTime.w) * 1.5;
				float _t = abs(tan(_Time.y*1.5));
				float _c = (1.0 - abs(_d - _t));
				//_c = pow(_c, 3);
				_c = clamp(_c,0,1) * 0.4;

				
				//IN.color.r = 0.2;
				//IN.color.g = 0.2;
				//IN.color.b = 0.8;
				//IN.color.a = 0.25;

				//_Color("Tint", Color) = (0, 0.1, 1, 0.79)
				_BeamColor.r = 0;
				_BeamColor.g = 0.1;
				_BeamColor.b = 1.0;
				_BeamColor.a = 0.79;

				fixed4 texCol = tex2D(_MainTex, IN.texcoord); // *(IN.color * _c);
				float a = texCol.a;
				texCol = texCol * (1.0 - _c); // *(IN.color * _c);
				texCol.a = a;// (1.0 - _c);

				//_BeamColor.a = 0.1 * a;

				fixed4 colCol = _BeamColor * _c;
				//colCol.a = a;
				
				//c.rgb *= (_BeamColor.rgb * (1-_c));
				//c.a = 0.2;
				//fixed4 c = texCol;
				fixed4 c = texCol + colCol;
				//fixed4 c = IN.color;

				c.rgb *= a;
				return c;
			}
			ENDCG
		}
		}
}
