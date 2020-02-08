Shader "tk2d/Map"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_AlphaMulti("Alpha Multi", Range(0,1)) = 0.5
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }

		Pass
		{
			ZWrite Off Lighting Off Cull Off Fog{ Mode Off } Blend OneMinusDstAlpha DstAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				color.a = 1;

				return color;
			}
			ENDCG
		}

		Pass
		{
			ZWrite Off Lighting Off Cull Off Fog{ Mode Off } Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			fixed _AlphaMulti;

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				clip(0.5 - color.a);

				color.a = _AlphaMulti;

				return color;
			}

			ENDCG
		}
	}
}
