Shader "Unlit/MultiPassShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Pass2Width ("Pass2Width", Range(0, 1)) = 0.01
        _Pass3Width ("Pass2Width", Range(0, 1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="Pass1" }
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

        Pass
        {
            Cull Front
            Tags { "LightMode"="Pass2" }
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Pass2Width;

            v2f vert (appdata v)
            {
                v2f o;
                float dist = UnityObjectToViewPos(v.vertex).z;
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * -dist * _Pass2Width);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return half4(0, 1, 0, 1);
            }
            ENDCG
        }

        Pass
        {
            Cull Front
            Tags { "LightMode"="Pass3" }
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Pass3Width;

            v2f vert (appdata v)
            {
                v2f o;
                float dist = UnityObjectToViewPos(v.vertex).z;
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * -dist * _Pass3Width);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return half4(0, 0, 1, 1);
            }
            ENDCG
        }
    }
}
