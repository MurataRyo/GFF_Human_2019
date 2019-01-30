Shader "Transparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("mainTex", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

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
			fixed4 mainColor = tex2D (_MainTex, tex.uv_MainTex);
			fixed4 subColor = fixed4(0,1,1,0.5);

			float mainA = 1 - subColor.a;
			if (mainA < 0)
			{
				mainA = 0;
			}

			mainColor.r = mainColor.r * mainA + subColor.r * (1 - mainA);
			mainColor.g = mainColor.g * mainA + subColor.g * (1 - mainA);
			mainColor.b = mainColor.b * mainA + subColor.b * (1 - mainA);

			o.Albedo = mainColor;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 0.4;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
