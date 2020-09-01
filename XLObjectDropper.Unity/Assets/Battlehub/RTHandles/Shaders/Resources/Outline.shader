Shader "Battlehub/RTHandles/Outline"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" { }
		_Color("Color", Color) = (0.2, 0.2, 0.2, 1)
		_Thickness("Thickness", Float) = 1.0
		_Smoothness("Smoothness", Float) = 1.0
		[HideInInspector]_ZTest("__zt", Float) = 0.0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		Pass
		{
			Cull Off
			ZTest[_ZTest]
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
				float4 baryc : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Thickness;
			float _Smoothness;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.baryc = v.color;
				o.color = _Color;
				o.color.a = v.color.a;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			float edgeFactor(float3 barycentric) {
				float3 d = fwidth(barycentric);
				float3 a3 = smoothstep(float3(0, 0, 0), d * _Thickness, barycentric);
				return  min(a3.z, a3.x); 
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 color = i.color;
				float ef = 1.0 - edgeFactor(i.baryc);
				color.a = lerp(1.0 - step(ef, 0), lerp(ef, 0, color.a), _Smoothness);
				return color * tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
	CustomEditor "BattlehubOutlineEditor"
}
