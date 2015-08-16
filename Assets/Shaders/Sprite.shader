Shader "Custom/Sprite" {
	Properties {
		_MainTex ("Base (RGBA)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue" = "Transparent"} 
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;

			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct fragmentInput {
				float4 vertex : SV_POSITION;
				float4 texcoord : TEXCOORD0;
			};

			fragmentInput vert( vertexInput i ) {
				fragmentInput o;
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.texcoord = i.texcoord;
				return o;
			}

			float4 frag( fragmentInput i ) : COLOR {
				return tex2D( _MainTex, i.texcoord.xy );
			}
			ENDCG
		} 
	}
}