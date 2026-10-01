Shader "FPS/Weapons/WeaponSurface"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.1, 0.1, 0.1, 1)
        _BaseMap("Base Map", 2D) = "white" {}
        _MetalColor("Metal Color", Color) = (0.7, 0.7, 0.7, 1)
        _MetallicMap("Metallic", 2D) = "white" {}
        _Metallic("Metallic Amount", Range(0, 1)) = 0.8
        _SmoothnessMap("Smoothness", 2D) = "white" {}
        _Smoothness("Smoothness", Range(0, 1)) = 0.7
        _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalScale("Normal Scale", Range(0, 2)) = 1.0
        _RimColor("Rim Light Color", Color) = (0.2, 0.2, 0.2, 1)
        _RimPower("Rim Power", Range(0.1, 5)) = 2.0
        _OcclusionMap("Occlusion", 2D) = "white" {}
        _OcclusionStrength("Occlusion Strength", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry+100"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
                float3 viewDir : TEXCOORD5;
                float fogCoord : TEXCOORD6;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_SmoothnessMap);
            SAMPLER(sampler_SmoothnessMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_OcclusionMap);
            SAMPLER(sampler_OcclusionMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _MetalColor;
            float _Metallic;
            float _Smoothness;
            float _NormalScale;
            float4 _RimColor;
            float _RimPower;
            float _OcclusionStrength;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                float3 posWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(posWS);
                output.positionWS = posWS;
                
                output.normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                output.tangentWS = normalize(TransformObjectToWorldDir(input.tangentOS.xyz));
                output.bitangentWS = cross(output.normalWS, output.tangentWS) * input.tangentOS.w;
                
                output.viewDir = normalize(_WorldSpaceCameraPos - posWS);
                output.uv = input.uv;
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                
                // Sample maps
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half3 baseColor = baseMap.rgb * _BaseColor.rgb;
                
                // Normal mapping
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv));
                normalTS.xy *= _NormalScale;
                normalTS = normalize(normalTS);
                
                float3x3 TBN = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                float3 normalWS = mul(normalTS, TBN);
                normalWS = normalize(normalWS);
                
                // Metallic
                float metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, uv).r * _Metallic;
                float smoothness = SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, uv).r * _Smoothness;
                
                // Occlusion
                float occlusion = lerp(1.0, SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).r, _OcclusionStrength);
                
                // Blend metal color with base
                baseColor = lerp(baseColor, _MetalColor.rgb, metallic * 0.5);
                
                // Rim light for tactical appeal
                float rimDot = 1.0 - max(0.0, dot(input.viewDir, normalWS));
                float rimLight = pow(rimDot, _RimPower) * 0.3;
                half3 rim = _RimColor.rgb * rimLight;
                
                // Surface data
                SurfaceData surfaceData;
                surfaceData.albedo = baseColor;
                surfaceData.normalTS = normalTS;
                surfaceData.emission = rim;
                surfaceData.metallic = metallic;
                surfaceData.smoothness = smoothness;
                surfaceData.occlusion = occlusion;
                surfaceData.specularColor = half3(0, 0, 0);
                
                InputData inputData;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = input.viewDir;
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                inputData.fogCoord = input.fogCoord;
                inputData.vertexLighting = half3(0, 0, 0);
                inputData.bakedGI = half3(0, 0, 0);
                inputData.normalizedScreenSpaceUV = float2(0, 0);
                inputData.shadowMask = half4(1, 1, 1, 1);
                
                half4 finalColor = UniversalFragmentPBR(inputData, surfaceData);
                finalColor.rgb = MixFog(finalColor.rgb, input.fogCoord);
                
                return finalColor;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 posWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(posWS);
                #if UNITY_REVERSED_Z
                output.positionCS.z = min(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                output.positionCS.z = max(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif
                return output;
            }

            half4 frag(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/Lit"
}
