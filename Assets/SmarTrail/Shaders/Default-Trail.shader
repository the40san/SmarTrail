Shader "SmarTrail/Default-Trail" {
	Properties {        
    	[Enum(OFF, 0, FRONT, 1, BACK, 2)] _CullMode("Cull Mode", int) = 0
    	
		[HDR] _Color ("Color", Color) = (1,1,1,1)
		
		_NormalTex ("Distortion", 2D) = "black" {}
		_Distortion ("Distortion Strength", Range(0, 10)) = 1
	}
	
	SubShader {
		Tags { 
	    	"RenderType"="Transparent" 
	    	"Queue"="Transparent" 
	    }
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass {}
		
		Pass {
            Cull[_CullMode]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            sampler2D _NormalTex;
            
            float4 _NormalTex_ST;
            float4 _GrabTexture_ST;
            float4 _Color;
            float  _Distortion;
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 grabCoord: TEXCOORD0;
                float2 texcoord: TEXCOORD1;
            };
            
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.grabCoord = ComputeGrabScreenPos(o.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _NormalTex);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                float4 normalTex = float4(UnpackNormal(tex2D(_NormalTex, i.texcoord)).rg, 0, 0);
                i.grabCoord += normalTex * _Distortion;
                
                float4 c = tex2Dproj(_GrabTexture, i.grabCoord) * _Color;
                return c;
            }
            ENDCG
        }
	}
	FallBack "Diffuse"
}
