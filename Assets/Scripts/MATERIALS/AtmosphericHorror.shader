Shader "Custom/AtmosphericHorror"
{
    Properties
    {
        [Header(Base Textures)]
        [MainTexture] _BaseMap("Albedo (Floor/Wall)", 2D) = "white" {}
        [MainColor] _BaseColor("Color Tint", Color) = (1,1,1,1)
        
        [Header(Surface Settings)]
        _Brightness("Overall Brightness", Range(0.5, 5.0)) = 1.0
        _Smoothness("Smoothness (Shiny Floor)", Range(0.0, 1.0)) = 0.5
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        [Normal] _BumpMap("Normal Map (Wood/Tile Depth)", 2D) = "bump" {}
        _BumpScale("Normal Scale", Float) = 1.0

        [Header(Horror Atmosphere)]
        _DarknessStart("Darkness Start Distance", Float) = 15.0
        _DarknessEnd("Darkness Full Distance", Float) = 40.0
        _DarknessColor("Darkness Color (Fog)", Color) = (0.05, 0.07, 0.1, 1)
        _AmbientTint("Cold Ambient Tint (Shadow Color)", Color) = (0.15, 0.18, 0.22, 1)
        _ShadowBrightness("Shadow Visibility", Range(0.0, 1.0)) = 0.2
        _ShadowContrast("Shadow Contrast", Range(0.5, 2.0)) = 1.0
        _GrainIntensity("Victorian Grain Intensity", Range(0, 0.1)) = 0.02
        
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
                // 1. Sample Base Color
                float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // 2. Lighting Calculation
                Light mainLight = GetMainLight();
                
                // Diffuse with adjustable contrast
                float NdotL = dot(input.normalWS, mainLight.direction);
                float halfLambert = NdotL * 0.5 + 0.5;
                float diffuseMask = pow(halfLambert, _ShadowContrast);
                float3 diffuse = diffuseMask * mainLight.color;
                
                // Lift shadows using Ambient Tint and Shadow Brightness
                float3 ambient = _AmbientTint.rgb + (albedo.rgb * _ShadowBrightness);
                float3 lighting = (diffuse + ambient) * _Brightness;
                float3 finalColor = albedo.rgb * lighting;

                // 3. Specular (Clean highlights)
                float3 halfDir = normalize(mainLight.direction + input.viewDirWS);
                float spec = pow(saturate(dot(input.normalWS, halfDir)), _Smoothness * 256.0);
                finalColor += spec * _Smoothness * mainLight.color * _Brightness;

                // 4. Emission
                #if _EMISSION
                    finalColor += _EmissionColor.rgb * _Brightness;
                #endif

                // 5. Victorian Grain Effect
                float noise = SimpleNoise(input.uv * _Time.y);
                finalColor += (noise - 0.5) * _GrainIntensity;

                // 6. DISTANCE DARKNESS (Lighter falloff)
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