Shader "Unlit/simple_gaussian_blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", Float) = 3.0
        _Steps ("Steps", Integer) = 16
        _Quality ("Quality", Integer) = 3
    }
    SubShader
    {
        //Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            sampler2D _CameraOpaqueTexture;
            float4 _CameraOpaqueTexture_TexelSize;
            
            float _Radius;
            float _Steps;
            float _Quality;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 frag (v2f frag) : SV_Target
            {
                float TAU = 6.28318530718; // Pi*2
                float2 radius = _Radius/_MainTex_TexelSize.zw;
                float4 color = tex2D(_MainTex, frag.uv);
                
                // Blur calculations
                for( float d=0.0; d<TAU; d+=TAU/_Steps)
                {
		            for(float i=1.0/_Quality; i<=1.0; i+=1.0/_Quality)
                    {
			            color += tex2D(_MainTex, frag.uv+float2(cos(d), sin(d)) * radius*i);		
                    }
                }
                
                color /= _Quality * _Steps - 15;
                return float4(color.rgb * frag.color.rgb, frag.color.a);
                //return float4(frag.color.rgb, 1.0);
            }

            ENDCG
        }
    }
}
