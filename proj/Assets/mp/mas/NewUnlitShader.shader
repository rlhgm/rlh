Shader "My/Default"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		_Frequency("Clip Frequency", Range(0,10)) = 6
		_ClipDist("ClipDist", Range(0,1)) = 0.5
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

			fixed4 frag(v2f IN) : SV_Target
			{
				//fixed2 f2 = fixed2(IN.vertex.xy / _ScreenParams.xy);
				fixed2 f2 = fixed2(IN.lvertex.xy / _ScreenParams.xy);
				//clip(frac((IN.lvertex.x + _Offset) * _Frequency) - 0.5);
				//clip(frac(f2 + _Frequency) - 0.5);
				//clip(frac((_ScreenParams.xy + _Offset) * _Frequency) - 0.5);
				//clip(1);
				//clip(frac((IN.vertex.y + _Offset) * _Frequency) - 0.5);
				//clip(frac(IN.vertex.y)-0.5);

				//frac((IN.vertex.y + _Offset) * _Frequency);

				//fixed4 c = tex2D(_MainTex, IN.texcoord);

				clip(frac(IN.lvertex.x * 8) - _ClipDist);
				clip(frac(IN.lvertex.y * 8) - _ClipDist);

				//fixed4 c = fixed4(1,1,1,1);// tex2D(_MainTex, IN.texcoord) * IN.color;
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				//c.rgb *= _Offset;
				//c.r = abs(IN.lvertex.x); // IN.vertex.x;
				//c.g = frac(IN.lvertex.y); // IN.vertex.x;
				c.g = 0.0; // frac(IN.vertex.y);
				c.r = 0.0; // IN.vertex.x / _ScreenParams.x;
				//c.r = IN.lvertex.y / _ScreenParams.y;
				c.b = 0.0;
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
