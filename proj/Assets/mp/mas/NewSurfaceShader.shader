Shader "My/Diffuse Simple" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Offset("Clip Offset", Range(0,1)) = 0
	}
	
	SubShader
	{
		cull off
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert
		struct Input 
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		float _Offset;

		void surf(Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			//o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Albedo = c.rgb * _Offset;
			o.Alpha = _Offset;
		}
		ENDCG
	}
	Fallback "Diffuse"
}
