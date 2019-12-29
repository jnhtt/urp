Shader "Hidden/BloomBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BloomThreshold("Bloom Threshold", Range(0.0, 1.0)) = 0.01
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            fixed _BloomThreshold;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mark = tex2D(_MainTex, i.uv);
                mark.rgb = step(_BloomThreshold, mark.a) * mark.rgb;
                return mark;
            }
            ENDCG
        }
    }
}
