Shader "SpriteShaders/SpriteShaders"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MainColor]   _Color   ("Tint", Color) = (1,1,1,1)

        [Toggle(OUTLINE_ON)] _OutlineToggle ("Outline", Float) = 0
        _OutlineColor ("  Color", Color) = (0,0,0,1)
        _OutlineSize  ("  Size",  Range(0,10)) = 1.0

        [Toggle(GLOW_ON)] _GlowToggle ("Glow", Float) = 0
        [HDR] _GlowColor     ("  Color",     Color) = (0,1,1,1)
        _GlowSize             ("  Size",      Range(0,10)) = 2.0
        _GlowIntensity        ("  Intensity", Range(0,10)) = 3.0

        [Toggle(FLASH_ON)] _FlashToggle ("Flash", Float) = 0
        _FlashColor  ("  Color",  Color) = (1,1,1,1)
        _FlashAmount ("  Amount", Range(0,1)) = 0.0

        [Toggle(COLOR_OVERLAY_ON)] _OverlayToggle ("Color Overlay", Float) = 0
        _OverlayColor  ("  Color",  Color) = (1,0,0,1)
        _OverlayAmount ("  Amount", Range(0,1)) = 0.5

        [Toggle(HUE_SHIFT_ON)] _HueShiftToggle ("Hue Shift", Float) = 0
        _HueShift      ("  Hue Shift", Range(0,1)) = 0.0
        _HueShiftSpeed ("  Speed",     Float)      = 1.0

        [Toggle(COLOR_ADJUST_ON)] _ColorAdjustToggle ("Color Adjust", Float) = 0
        _Brightness  ("  Brightness",  Range(-1,1)) = 0.0
        _Contrast    ("  Contrast",    Range(-1,1)) = 0.0
        _Saturation  ("  Saturation",  Range(-1,1)) = 0.0

        [Toggle(DISSOLVE_ON)] _DissolveToggle ("Dissolve", Float) = 0
        _DissolveTex      ("  Noise Texture",  2D)          = "white" {}
        _DissolveAmount   ("  Amount",          Range(0,1)) = 0.0
        _DissolveEdgeWidth("  Edge Width",      Range(0,0.3)) = 0.05
        _DissolveEdgeColor("  Edge Color",      Color)       = (1,0.5,0,1)

        [Toggle(PIXELATE_ON)] _PixelateToggle ("Pixelate", Float) = 0
        _PixelateSize ("  Pixel Size", Range(2,512)) = 32.0

        [Toggle(SHINE_ON)] _ShineToggle ("Shine", Float) = 0
        [HDR] _ShineColor   ("  Color",       Color)        = (1,1,1,1)
        _ShineWidth         ("  Width",        Range(0,1))  = 0.1
        _ShineAngle         ("  Angle",        Range(-180,180)) = 45.0
        _ShineGlossiness    ("  Glossiness",   Range(0,1))  = 0.5
        _ShineSpeed         ("  Speed",        Float)       = 1.0

        [Toggle(HOLOGRAM_ON)] _HologramToggle ("Hologram", Float) = 0
        [HDR] _HologramColor  ("  Color",       Color)       = (0,1,1,1)
        _HologramLineSpeed    ("  Line Speed",  Float)       = 2.0
        _HologramLineSize     ("  Line Size",   Range(0.01,1)) = 0.05
        _HologramDistortion   ("  Distortion",  Range(0,0.1)) = 0.01
        _HologramAlpha        ("  Alpha",       Range(0,1))  = 0.5

        [Toggle(GLITCH_ON)] _GlitchToggle ("Glitch", Float) = 0
        _GlitchIntensity ("  Intensity", Range(0,1)) = 0.3
        _GlitchSpeed     ("  Speed",     Float)      = 1.0
        _GlitchBlockSize ("  Block Size",Range(0.01,1)) = 0.1

        [Toggle(GRAYSCALE_ON)] _GrayscaleToggle ("Grayscale", Float) = 0
        _GrayscaleAmount ("  Amount", Range(0,1)) = 1.0

        [Toggle(WAVE_ON)] _WaveToggle ("Wave", Float) = 0
        _WaveAmplitude ("  Amplitude", Range(0,0.1)) = 0.02
        _WaveFrequency ("  Frequency", Range(0,20))  = 5.0
        _WaveSpeed     ("  Speed",     Float)        = 1.0

        [Toggle(WIND_ON)] _WindToggle ("Wind Sway", Float) = 0
        _WindStrength  ("  Strength",  Range(0,1)) = 0.03
        _WindSpeed     ("  Speed",     Float)         = 1.5
        _WindFrequency ("  Frequency", Range(0,10))   = 1.0

        [HideInInspector] _SrcBlend ("__src", Float) = 5.0   // SrcAlpha
        [HideInInspector] _DstBlend ("__dst", Float) = 10.0  // OneMinusSrcAlpha
        [HideInInspector] _ZWrite   ("__zw",  Float) = 0.0
        [HideInInspector] _Cull     ("__cull",Float) = 0.0   // Off
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "RenderType"      = "Transparent"
            "RenderPipeline"  = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]

        Pass
        {
            Name "SpriteShadersPass"

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag

            // Effect keywords
            #pragma shader_feature_local OUTLINE_ON
            #pragma shader_feature_local GLOW_ON
            #pragma shader_feature_local FLASH_ON
            #pragma shader_feature_local COLOR_OVERLAY_ON
            #pragma shader_feature_local HUE_SHIFT_ON
            #pragma shader_feature_local COLOR_ADJUST_ON
            #pragma shader_feature_local DISSOLVE_ON
            #pragma shader_feature_local PIXELATE_ON
            #pragma shader_feature_local SHINE_ON
            #pragma shader_feature_local HOLOGRAM_ON
            #pragma shader_feature_local GLITCH_ON
            #pragma shader_feature_local GRAYSCALE_ON
            #pragma shader_feature_local WAVE_ON
            #pragma shader_feature_local WIND_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "SpriteShadersIncludes.hlsl"

            TEXTURE2D(_MainTex);       SAMPLER(sampler_MainTex);
            TEXTURE2D(_DissolveTex);   SAMPLER(sampler_DissolveTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                float4 _DissolveTex_ST;
                float4 _Color;

                // Outline
                float4 _OutlineColor;
                float  _OutlineSize;

                // Glow
                float4 _GlowColor;
                float  _GlowSize;
                float  _GlowIntensity;

                // Flash
                float4 _FlashColor;
                float  _FlashAmount;

                // Overlay
                float4 _OverlayColor;
                float  _OverlayAmount;

                // Hue Shift
                float  _HueShift;
                float  _HueShiftSpeed;

                // Color Adjust
                float  _Brightness;
                float  _Contrast;
                float  _Saturation;

                // Dissolve
                float  _DissolveAmount;
                float  _DissolveEdgeWidth;
                float4 _DissolveEdgeColor;

                // Pixelate
                float  _PixelateSize;

                // Shine
                float4 _ShineColor;
                float  _ShineWidth;
                float  _ShineAngle;
                float  _ShineGlossiness;
                float  _ShineSpeed;

                // Hologram
                float4 _HologramColor;
                float  _HologramLineSpeed;
                float  _HologramLineSize;
                float  _HologramDistortion;
                float  _HologramAlpha;

                // Glitch
                float  _GlitchIntensity;
                float  _GlitchSpeed;
                float  _GlitchBlockSize;

                // Grayscale
                float  _GrayscaleAmount;

                // Wave
                float  _WaveAmplitude;
                float  _WaveFrequency;
                float  _WaveSpeed;

                // Wind Sway
                float  _WindStrength;
                float  _WindSpeed;
                float  _WindFrequency;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
                float3 worldPos    : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                #ifdef WIND_ON
                {
                    float heightFactor = IN.uv.y;
                    IN.positionOS.x += WindSwayOffset(heightFactor, _WindStrength, _WindSpeed, _WindFrequency, _Time.y);
                }
                #endif

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color       = IN.color;
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);

                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                #ifdef PIXELATE_ON
                {
                    float2 pixelatedUV = round(uv * _PixelateSize) / _PixelateSize;
                    uv = pixelatedUV;
                }
                #endif

                #ifdef GLITCH_ON
                {
                    uv = ApplyGlitch(uv, _GlitchIntensity, _GlitchSpeed, _GlitchBlockSize, _Time.y);
                }
                #endif

                #ifdef WAVE_ON
                {
                    uv = ApplyWave(uv, _WaveAmplitude, _WaveFrequency, _WaveSpeed, _Time.y);
                }
                #endif

                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                col *= IN.color * _Color;

                #ifdef HUE_SHIFT_ON
                {
                    float3 hsv = RGBtoHSV(col.rgb);
                    hsv.x = frac(hsv.x + _HueShift + fmod(_Time.y * _HueShiftSpeed, 1.0));
                    col.rgb = HSVtoRGB(hsv);
                }
                #endif

                #ifdef COLOR_ADJUST_ON
                {
                    col = AdjustBrightnessContrastSaturation(col, _Brightness, _Contrast, _Saturation);
                }
                #endif

                #ifdef GRAYSCALE_ON
                {
                    col = ApplyGrayscale(col, _GrayscaleAmount);
                }
                #endif

                #ifdef COLOR_OVERLAY_ON
                {
                    col.rgb = lerp(col.rgb, _OverlayColor.rgb * _OverlayColor.a, _OverlayAmount);
                }
                #endif

                #ifdef FLASH_ON
                {
                    col.rgb = lerp(col.rgb, _FlashColor.rgb, _FlashAmount * col.a);
                }
                #endif

                #ifdef DISSOLVE_ON
                {
                    float2 dissolveUV = TRANSFORM_TEX(uv, _DissolveTex);
                    col = ApplyDissolve(col,
                        TEXTURE2D_ARGS(_DissolveTex, sampler_DissolveTex),
                        dissolveUV,
                        _DissolveAmount,
                        _DissolveEdgeWidth,
                        _DissolveEdgeColor);
                }
                #endif

                #ifdef SHINE_ON
                {
                    col = ApplyShine(col, uv, _ShineColor, _ShineWidth,
                                     _ShineAngle, _ShineGlossiness, _ShineSpeed, _Time.y);
                }
                #endif

                #ifdef HOLOGRAM_ON
                {
                    col = ApplyHologram(col, uv, _HologramAlpha, _HologramColor,
                                        _HologramLineSpeed, _HologramLineSize,
                                        _HologramDistortion, _Time.y);
                }
                #endif

                float originalAlpha = col.a;

                #ifdef OUTLINE_ON
                {
                    float neighborAlpha = SampleOutlineAlpha(
                        TEXTURE2D_ARGS(_MainTex, sampler_MainTex),
                        IN.uv, _MainTex_TexelSize, _OutlineSize);

                    // Draw outline only where sprite is transparent but neighbor is not
                    if (originalAlpha < 0.01 && neighborAlpha > 0.01)
                    {
                        col.rgb = _OutlineColor.rgb;
                        col.a   = _OutlineColor.a * neighborAlpha;
                    }
                }
                #endif

                #ifdef GLOW_ON
                {
                    float glowAlpha = SampleGlowAlpha(
                        TEXTURE2D_ARGS(_MainTex, sampler_MainTex),
                        IN.uv, _MainTex_TexelSize, _GlowSize);

                    if (originalAlpha < 0.01 && glowAlpha > 0.01)
                    {
                        col.rgb = _GlowColor.rgb * _GlowIntensity;
                        col.a   = _GlowColor.a * glowAlpha;
                    }
                }
                #endif

                return col;
            }
            ENDHLSL
        }
    }

    CustomEditor "SpriteShaders.Editor.SpriteShadersEditor"
    FallBack "Universal Render Pipeline/2D/Sprite-Unlit-Default"
}
