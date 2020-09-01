// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Battlehub/RTHandles/VertexColor" {
	Properties
	{
		_ZWrite("ZWrite", Float) = 0.0
		_ZTest("ZTest", Float) = 0.0
		_Cull("Cull", Float) = 0.0
	}
	SubShader
	{

		Tags{ "Queue" = "Transparent+5" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull[_Cull]
			ZTest[_ZTest]
			ZWrite[_ZWrite]

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert  
			#pragma fragment frag 

			struct vertexInput {
				float4 vertex : POSITION;
				float4 color: COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 color: COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			inline float4 GammaToLinearSpace(float4 sRGB)
			{
				if (IsGammaSpace())
				{
					return sRGB;
				}
				return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);
			}

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				output.pos = UnityObjectToClipPos(input.vertex);
				output.color = GammaToLinearSpace(input.color);
				output.color.a = input.color.a;
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				UNITY_SETUP_INSTANCE_ID(input);
				return  input.color;
			}

			ENDCG
		}
	}
}