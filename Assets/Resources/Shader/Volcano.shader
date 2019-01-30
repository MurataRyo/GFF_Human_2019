Shader "Volcano" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Volcano", 2D) = "white" {}
		_SubTex ("Ground", 2D) = "" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Speed("Speed", Range(0,2)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SubTex;

		struct Input {
			//uvの後にsample2Dと同じ名前にすると勝手に入る
			float2 uv_MainTex;
			float2 uv_SubTex;
		};

		half _Glossiness;
		half _Metallic;
		float _Speed;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input tex, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			//溶岩
			fixed4 mainColor = tex2D (_MainTex, tex.uv_MainTex + -(_Time * _Speed));
			//地面
			fixed4 subColor = tex2D (_SubTex, tex.uv_SubTex);

			if(subColor.a < 1)
			{
				o.Emission = mainColor;
			}
			else
			{
				o.Albedo = subColor;
			}
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
