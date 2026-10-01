Shader "FPS/UI/HolographicUI"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Color", Color) = (0, 1, 0.7, 1)
        _Glow("Glow Intensity", Range(0, 2)) = 1.0
        _Scan("Scan Position", Range(0, 1)) = 0.5
        _ScanWidth("Scan Width", Range(0, 0.2)) = 0.05
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _Color;
            float _Glow;
            float _Scan;
            float _ScanWidth;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half3 color = _Color.rgb * tex.rgb;
                
                // Scan line effect
                float scanEffect = smoothstep(_Scan - _ScanWidth, _Scan, input.uv.y) * 
                                   smoothstep(_Scan + _ScanWidth, _Scan, input.uv.y);
                color += scanEffect * 0.3;
                
                // Glow
                color *= (1.0 + _Glow * 0.5);
                
                half alpha = tex.a * _Color.a * (0.7 + sin(_Time.y * 3.0) * 0.3);
                
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
}
