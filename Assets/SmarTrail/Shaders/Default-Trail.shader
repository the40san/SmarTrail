Shader "SmarTrail/Default-Trail" {
	Properties {        
    	[Enum(OFF, 0, FRONT, 1, BACK, 2)] _CullMode("Cull Mode", int) = 0
    	
		[HDR] _Color ("Color", Color) = (1,1,1,1)
		
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { 
	    	"RenderType"="Transparent" 
	    	"Queue"="Transparent" 
	    }
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
            Cull[_CullMode]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);  
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                float4 c = tex2D(_MainTex, i.texcoord) * _Color;
                return c;
            }
            ENDCG
        }
	}
	FallBack "Diffuse"
}
