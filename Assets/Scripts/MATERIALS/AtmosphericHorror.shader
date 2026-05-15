Shader "Custom/AtmosphericHorror"
{
    Properties
    {
        [Header(Base Textures)]
        [MainTexture] _BaseMap("Albedo (Floor/Wall)", 2D) = "white" {}
        [MainColor] _BaseColor("Color Tint", Color) = (1,1,1,1)
        
        [Header(Surface Settings)]
        _Brightness("Overall Brightness", Range(0.1, 2.0)) = 0.7 // Lowered default for unlit feel
        _Smoothness("Smoothness (Shiny Floor)", Range(0.0, 1.0)) = 0.4
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        [Normal] _BumpMap("Normal Map (Wood/Tile Depth)", 2D) = "bump" {}
        _BumpScale("Normal Scale", Float) = 1.0

        [Header(Horror Atmosphere)]
        _DarknessStart("Darkness Start Distance", Float) = 15.0 // Darkness creeps in sooner
        _DarknessEnd("Darkness Full Distance", Float) = 45.0
        _DarknessColor("Darkness Color (Fog)", Color) = (0.02, 0.02, 0.03, 1) // Deeper near-black grey
        _AmbientTint("Building Shadow Tint", Color) = (0.1, 0.1, 0.12, 1) // Much darker ambient base
        _ShadowBrightness("Shadow Visibility", Range(0.0, 1.0)) = 0.15 // Less light bleed in shadows
        _ShadowContrast("Shadow Contrast", Range(0.5, 3.0)) = 1.4 // Higher contrast for harsher shadows
        _GrainIntensity("Victorian Grain Intensity", Range(0, 0.1)) = 0.015
        
        [Toggle(_EMISSION)] _UseEmission("Use Light Glow", Float) = 0.0
        [HDR] _EmissionColor("Light Color", Color) = (0,0,0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog
            #pragma shader_feature_local_fragment _EMISSION

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Brightness;
                float _Smoothness;
                float _Metallic;
                float _BumpScale;
                float _DarknessStart;
                float _DarknessEnd;
                float4 _DarknessColor;
                float4 _AmbientTint;
                float _ShadowBrightness;
                float _ShadowContrast;
                float4 _EmissionColor;
                float _GrainIntensity;
            CBUFFER_END

            float SimpleNoise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(output.positionWS);
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                Light mainLight = GetMainLight();
                
                // Diffuse Calculation with higher contrast for "unlit" feel
                float NdotL = dot(input.normalWS, mainLight.direction);
                float halfLambert = NdotL * 0.5 + 0.5;
                float diffuseMask = pow(halfLambert, _ShadowContrast);
                float3 diffuse = diffuseMask * mainLight.color;
                
                // Ambient is the primary light source for an unlit building
                float3 ambient = _AmbientTint.rgb + (albedo.rgb * _ShadowBrightness);
                float3 lighting = (diffuse + ambient) * _Brightness;
                float3 finalColor = albedo.rgb * lighting;

                // Dimmed Specular
                float3 halfDir = normalize(mainLight.direction + input.viewDirWS);
                float spec = pow(saturate(dot(input.normalWS, halfDir)), _Smoothness * 128.0);
                finalColor += spec * _Smoothness * mainLight.color * _Brightness * 0.5;

                #if _EMISSION
                    finalColor += _EmissionColor.rgb * _Brightness;
                #endif

                // Grain helps define shapes in low light
                float noise = SimpleNoise(input.uv * _Time.y);
                finalColor += (noise - 0.5) * _GrainIntensity;

                // Distance-based darkness (Fog)
                float dist = distance(input.positionWS, _WorldSpaceCameraPos);
                float darknessFactor = saturate((dist - _DarknessStart) / (_DarknessEnd - _DarknessStart));
                finalColor = lerp(finalColor, _DarknessColor.rgb, darknessFactor);

                return float4(finalColor, albedo.a);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}