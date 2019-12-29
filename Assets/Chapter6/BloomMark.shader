Shader "CustomRP/BloomMark"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Pass2Width ("Pass2Width", Range(0, 1)) = 0.01
        _Pass3Width ("Pass2Width", Range(0, 1)) = 0.01
        _BloomColor ("Bloom Color", Color) = (1, 1, 1, 1)
        _BloomWidth ("Bloom Width", Range(0, 0.5)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "BloomMark"
            Tags { "LightMode"="BloomMark" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _BloomColor;
            fixed _BloomWidth;

            float DistanceToCamera(float3 pos)
            {
                return mul(unity_MatrixV, mul(unity_ObjectToWorld, float4(pos, 1.0))).z;
            }

            v2f vert (appdata v)
            {
                v2f o;
                float d = DistanceToCamera(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * -d * _BloomWidth);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _BloomColor;
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode"="LightweightForward" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return half4(1, 0, 0, 1);
            }
            ENDCG
        }
    }
}
