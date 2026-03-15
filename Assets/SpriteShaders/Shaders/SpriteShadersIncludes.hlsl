#ifndef SPRITE_SHADERS_INCLUDES
#define SPRITE_SHADERS_INCLUDES

// ─────────────────────────────────────────────────────────────────────────────
// Color Space Conversions
// ─────────────────────────────────────────────────────────────────────────────

float3 RGBtoHSV(float3 rgb)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(rgb.bg, K.wz), float4(rgb.gb, K.xy), step(rgb.b, rgb.g));
    float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 HSVtoRGB(float3 hsv)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
    return hsv.z * lerp(K.xxx, saturate(p - K.xxx), hsv.y);
}

// ─────────────────────────────────────────────────────────────────────────────
// Hash / Noise Utilities
// ─────────────────────────────────────────────────────────────────────────────

float Hash(float2 p)
{
    p = frac(p * float2(123.34, 456.21));
    p += dot(p, p + 19.19);
    return frac(p.x * p.y);
}

float Hash1(float n) { return frac(sin(n) * 43758.5453123); }

// ─────────────────────────────────────────────────────────────────────────────
// Outline / Glow Sampling (8-directional neighbor)
// ─────────────────────────────────────────────────────────────────────────────

float SampleOutlineAlpha(TEXTURE2D_PARAM(tex, s), float2 uv, float4 texelSize, float size)
{
    float alpha = 0.0;
    float2 offsets[8] = {
        float2(-1, 0), float2(1, 0),
        float2(0, -1), float2(0,  1),
        float2(-1,-1), float2(1, -1),
        float2(-1, 1), float2(1,  1)
    };

    [unroll]
    for (int i = 0; i < 8; i++)
    {
        float2 sampleUV = uv + offsets[i] * texelSize.xy * size;
        alpha = max(alpha, SAMPLE_TEXTURE2D(tex, s, sampleUV).a);
    }
    return alpha;
}

// Softer glow: sample more neighbors at varying distances
float SampleGlowAlpha(TEXTURE2D_PARAM(tex, s), float2 uv, float4 texelSize, float size)
{
    float alpha = 0.0;
    int steps = 4;
    [unroll]
    for (int x = -steps; x <= steps; x++)
    {
        [unroll]
        for (int y = -steps; y <= steps; y++)
        {
            float dist = length(float2(x, y));
            bool valid = !(x == 0 && y == 0) && (dist <= (float)steps + 0.5);
            if (valid)
            {
                float2 sampleUV = uv + float2(x, y) * texelSize.xy * size / (float)steps;
                float a = SAMPLE_TEXTURE2D(tex, s, sampleUV).a;
                float falloff = 1.0 - saturate(dist / ((float)steps + 0.5));
                alpha = max(alpha, a * falloff);
            }
        }
    }
    return alpha;
}

// ─────────────────────────────────────────────────────────────────────────────
// Color Adjustments
// ─────────────────────────────────────────────────────────────────────────────

float4 AdjustBrightnessContrastSaturation(float4 col, float brightness, float contrast, float saturation)
{
    // Brightness
    col.rgb += brightness;

    // Contrast
    col.rgb = (col.rgb - 0.5) * (contrast + 1.0) + 0.5;

    // Saturation
    float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));
    col.rgb = lerp(float3(luminance, luminance, luminance), col.rgb, saturation + 1.0);

    col.rgb = saturate(col.rgb);
    return col;
}

// ─────────────────────────────────────────────────────────────────────────────
// Dissolve
// ─────────────────────────────────────────────────────────────────────────────

float4 ApplyDissolve(float4 col, TEXTURE2D_PARAM(dissolveTex, dissolveS),
                     float2 uv, float amount, float edgeWidth, float4 edgeColor)
{
    float noise = SAMPLE_TEXTURE2D(dissolveTex, dissolveS, uv).r;

    // Clip fully dissolved pixels
    clip(noise - amount);

    // Edge glow
    float edge = saturate((noise - amount) / max(edgeWidth, 0.0001));
    col.rgb = lerp(edgeColor.rgb, col.rgb, edge);
    col.a  *= saturate(edge * 10.0); // sharp alpha near edge

    return col;
}

// ─────────────────────────────────────────────────────────────────────────────
// Shine (animated diagonal highlight)
// ─────────────────────────────────────────────────────────────────────────────

float4 ApplyShine(float4 col, float2 uv, float4 shineColor, float width,
                  float angle, float glossiness, float speed, float time)
{
    float angleRad = angle * (3.14159265 / 180.0);
    float2 dir = float2(cos(angleRad), sin(angleRad));
    float projected = dot(uv - 0.5, dir);

    float t = frac(time * speed);
    float shineLine = smoothstep(t - width, t, projected + 0.5) *
                      (1.0 - smoothstep(t, t + width * glossiness, projected + 0.5));

    float mask = col.a; // Only shine where sprite is visible
    col.rgb += shineColor.rgb * shineLine * mask * shineColor.a;
    return col;
}

// ─────────────────────────────────────────────────────────────────────────────
// Hologram
// ─────────────────────────────────────────────────────────────────────────────

float4 ApplyHologram(float4 col, float2 uv, float hologramAlpha,
                     float4 hologramColor, float lineSpeed, float lineSize,
                     float distortion, float time)
{
    // Scanlines
    float scanline = step(lineSize, frac((uv.y + time * lineSpeed) / (lineSize * 2.0)));

    // Horizontal distortion
    float glitchOffset = sin(uv.y * 50.0 + time * 10.0) * distortion;
    uv.x += glitchOffset;

    // Color shift
    col.rgb = lerp(col.rgb, hologramColor.rgb, hologramAlpha);

    // Apply scanlines (darken between lines)
    col.rgb *= lerp(0.3, 1.0, scanline);

    // Flicker alpha
    float flicker = 0.9 + 0.1 * sin(time * 30.0);
    col.a *= flicker * (0.5 + hologramAlpha * 0.5);

    return col;
}

// ─────────────────────────────────────────────────────────────────────────────
// Glitch
// ─────────────────────────────────────────────────────────────────────────────

float2 ApplyGlitch(float2 uv, float intensity, float speed, float blockSize, float time)
{
    if (intensity <= 0.0) return uv;

    float t = floor(fmod(time * max(speed, 0.001) * 10.0, 997.0));
    float blockY = floor(uv.y / max(blockSize, 0.001));

    float rand = Hash(float2(blockY, t));
    float threshold = 1.0 - intensity;

    float offset = (rand > threshold) ? (rand - threshold) / max(1.0 - threshold, 0.001) * 2.0 - 1.0 : 0.0;
    uv.x += offset * intensity * 0.2;

    return uv;
}

// ─────────────────────────────────────────────────────────────────────────────
// Grayscale
// ─────────────────────────────────────────────────────────────────────────────

float4 ApplyGrayscale(float4 col, float amount)
{
    float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
    col.rgb = lerp(col.rgb, float3(gray, gray, gray), amount);
    return col;
}

// ─────────────────────────────────────────────────────────────────────────────
// Wave / Wobble
// ─────────────────────────────────────────────────────────────────────────────

float2 ApplyWave(float2 uv, float amplitude, float frequency, float speed, float time)
{
    uv.x += sin(uv.y * frequency + time * speed) * amplitude;
    uv.y += cos(uv.x * frequency + time * speed) * amplitude * 0.5;
    return uv;
}

// ─────────────────────────────────────────────────────────────────────────────
// Wind Sway
// ─────────────────────────────────────────────────────────────────────────────

float WindSwayOffset(float heightFactor, float strength, float speed, float frequency, float time)
{
    return sin(time * speed * frequency) * strength * heightFactor;
}

#endif // SPRITE_SHADERS_INCLUDES
