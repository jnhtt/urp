//https://github.com/UnityTechnologies/LWRPScriptableRenderPass_ExampleLibrary/blob/master/LWRPScriptableRenderPass_ExampleLibrary_Project/Assets/ScriptableRenderPasses/BlurryRefractions/SeperableBlur.shader
Shader "Hidden/SeparableBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always
            Fog { Mode off }

            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 uv01 : TEXCOORD1;
                float4 uv23 : TEXCOORD2;
                float4 uv45 : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 offsets;

            v2f vert (appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                v.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                o.uv.xy = v.texcoord.xy;
                o.uv01 = v.texcoord.xyxy + offsets.xyxy * float4(1, 1, -1, -1);
                o.uv23 = v.texcoord.xyxy + offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
                o.uv45 = v.texcoord.xyxy + offsets.xyxy * float4(1, 1, -1, -1) * 3.0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(0, 0, 0, 0);
                col += 0.40 * tex2D(_MainTex, i.uv);
                col += 0.15 * tex2D(_MainTex, i.uv01.xy);
                col += 0.15 * tex2D(_MainTex, i.uv01.zw);
                col += 0.10 * tex2D(_MainTex, i.uv23.xy);
                col += 0.10 * tex2D(_MainTex, i.uv23.zw);
                col += 0.05 * tex2D(_MainTex, i.uv45.xy);
                col += 0.05 * tex2D(_MainTex, i.uv45.zw);
                return col;
            }
            ENDCG
        }
    }
}
