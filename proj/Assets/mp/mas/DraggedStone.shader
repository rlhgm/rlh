Shader "My/DraggedStone"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
			_Color("Tint", Color) = (1,1,1,1)
			_BeamColor("_BeamColor", Color) = (1,1,1,1)
			[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

			_sx("sx", Range(0,20)) = 1
			_sy("sy", Range(0,20)) = 1
			_speedX("speedX", Range(0,0.3)) = 0
			_speedY("speedY", Range(0,0.3)) = 0
			_rpx("rpx", Range(-20,20)) = 1
			_rpy("rpy", Range(-20,20)) = 1
			_draggedDuration("_draggedDuration", Range(0,60.0)) = 0
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
				fixed _BeamColor;
				float _sx;
				float _sy;
				float _speedX;
				float _speedY;
				float _rpx;
				float _rpy;
				float _draggedDuration;

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
				float _t131 = abs(tan(_draggedDuration*1.5));
				float _c = (1.0 - abs(_d - _t131));
				//_c = pow(_c, 3);
				_c = clamp(_c, 0, 1) * 0.4;


				float _s = 2.0f;// abs(sin(_Time.y)) * 2.0;
				float _addSin = abs(sin(_draggedDuration));
				float _px = _addSin * _speedX;
				float _py = _addSin * _speedY;
				float _t = f2.x / _sx * 3.14159 * _s;// +_Time.y; // *abs(_SinTime.w) * 4;
				float _t2 = sin(_t);
				IN.texcoord.x = (IN.texcoord.x + _t2 * _px);
				IN.texcoord.x = clamp(IN.texcoord.x, 0.0, 1.0); //rand2(r2)/10.0f;
				//IN.color.r = abs(_t2); //clamp(_t2, 0.0, 1.0);

				_t = f2.y / _sy * 3.14159 * _s;// +_Time.y; // *abs(_SinTime.w) * 4;
				_t2 = sin(_t);
				IN.texcoord.y = (IN.texcoord.y + _t2 * _py);
				IN.texcoord.y = clamp(IN.texcoord.y, 0.0, 1.0); //rand2(r2)/10.0f;
				
				fixed4 texCol = tex2D(_MainTex, IN.texcoord) * IN.color;
				float a = texCol.a;

				fixed4 _bcp = fixed4(0, 0.1, 1.0, 0.79);
				
				if (_draggedDuration < 1.17)
				{
					fixed4 _bcp2 = fixed4(0, 0.0, 0.5 * sin( (_draggedDuration / 1.17) * 3.14159 ), 0.0);
					texCol = texCol * (1.0 - _c) + ((1.7 - _draggedDuration) * _bcp2);
				}
				else
				{
					texCol = texCol * (1.0 - _c);
				}
				texCol.a = a;

				//_BeamColor = fixed4(0,0.1,1.0,0.79);
				//_BeamColor.g = 0.1;
				//_BeamColor.b = 1.0;
				//_BeamColor.a = 0.79;
				
				fixed4 colCol = _bcp * _c;
				fixed4 c = texCol + colCol;

				//fixed4 c = texCol;

				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
		}
}
