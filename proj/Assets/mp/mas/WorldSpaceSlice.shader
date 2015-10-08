Shader "Example/WorldSpace Slices" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", 2D) = "white" {}
      //_BumpMap ("Bumpmap", 2D) = "bump" {}
      _Frequency ("Clip Frequency", Range(0,10)) = 5.0
      _Offset ("Clip Offset", Range(0,1)) = 0
    }
    SubShader {
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

      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;
          //float2 uv_BumpMap;
          float3 worldPos;
		  float4 color;
      };
      sampler2D _MainTex;
      //sampler2D _BumpMap;
      float _Frequency;
      float _Offset;
      void surf (Input IN, inout SurfaceOutput o) {
		  //clip(frac((IN.worldPos.x + _Offset) * _Frequency) - 0.5);
          //clip (frac((IN.worldPos.y+_Offset) * _Frequency) - 0.5);

		  fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
          //o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		  //c.r = frac(IN.worldPos.x);
		  //c.g = frac(IN.worldPos.y);
		  //c.b = 1.0;
		  //o.Albedo *= c.a;
          //o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));

		  o.Albedo = c.rgb;
		  o.Albedo *= c.a;
		  o.Alpha *= c.a;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
  