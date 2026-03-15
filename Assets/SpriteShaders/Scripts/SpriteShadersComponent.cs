using System.Collections;
using UnityEngine;

namespace SpriteShaders
{
    /// <summary>
    /// Runtime component for controlling SpriteShaders effects via script.
    /// Attach to any GameObject that has a Renderer component (SpriteRenderer, etc.).
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    [AddComponentMenu("SpriteShaders/Sprite Shaders Component")]
    public class SpriteShadersComponent : MonoBehaviour
    {
        private const string ShaderName = "SpriteShaders/SpriteShaders";

        private static readonly int PropColor             = Shader.PropertyToID("_Color");
        private static readonly int PropOutlineColor      = Shader.PropertyToID("_OutlineColor");
        private static readonly int PropOutlineSize       = Shader.PropertyToID("_OutlineSize");
        private static readonly int PropGlowColor         = Shader.PropertyToID("_GlowColor");
        private static readonly int PropGlowSize          = Shader.PropertyToID("_GlowSize");
        private static readonly int PropGlowIntensity     = Shader.PropertyToID("_GlowIntensity");
        private static readonly int PropFlashColor        = Shader.PropertyToID("_FlashColor");
        private static readonly int PropFlashAmount       = Shader.PropertyToID("_FlashAmount");
        private static readonly int PropOverlayColor      = Shader.PropertyToID("_OverlayColor");
        private static readonly int PropOverlayAmount     = Shader.PropertyToID("_OverlayAmount");
        private static readonly int PropHueShift          = Shader.PropertyToID("_HueShift");
        private static readonly int PropBrightness        = Shader.PropertyToID("_Brightness");
        private static readonly int PropContrast          = Shader.PropertyToID("_Contrast");
        private static readonly int PropSaturation        = Shader.PropertyToID("_Saturation");
        private static readonly int PropDissolveAmount    = Shader.PropertyToID("_DissolveAmount");
        private static readonly int PropDissolveEdge      = Shader.PropertyToID("_DissolveEdgeWidth");
        private static readonly int PropDissolveEdgeColor = Shader.PropertyToID("_DissolveEdgeColor");
        private static readonly int PropPixelateSize      = Shader.PropertyToID("_PixelateSize");
        private static readonly int PropShineColor        = Shader.PropertyToID("_ShineColor");
        private static readonly int PropShineWidth        = Shader.PropertyToID("_ShineWidth");
        private static readonly int PropShineAngle        = Shader.PropertyToID("_ShineAngle");
        private static readonly int PropShineGlossiness   = Shader.PropertyToID("_ShineGlossiness");
        private static readonly int PropShineSpeed        = Shader.PropertyToID("_ShineSpeed");
        private static readonly int PropHologramColor     = Shader.PropertyToID("_HologramColor");
        private static readonly int PropHologramLineSpeed = Shader.PropertyToID("_HologramLineSpeed");
        private static readonly int PropHologramLineSize  = Shader.PropertyToID("_HologramLineSize");
        private static readonly int PropHologramDistortion = Shader.PropertyToID("_HologramDistortion");
        private static readonly int PropHologramAlpha     = Shader.PropertyToID("_HologramAlpha");
        private static readonly int PropGlitchIntensity   = Shader.PropertyToID("_GlitchIntensity");
        private static readonly int PropGlitchSpeed       = Shader.PropertyToID("_GlitchSpeed");
        private static readonly int PropGlitchBlockSize   = Shader.PropertyToID("_GlitchBlockSize");
        private static readonly int PropGrayscaleAmount   = Shader.PropertyToID("_GrayscaleAmount");
        private static readonly int PropWaveAmplitude     = Shader.PropertyToID("_WaveAmplitude");
        private static readonly int PropWaveFrequency     = Shader.PropertyToID("_WaveFrequency");
        private static readonly int PropWaveSpeed         = Shader.PropertyToID("_WaveSpeed");
        private static readonly int PropWindStrength      = Shader.PropertyToID("_WindStrength");
        private static readonly int PropWindSpeed         = Shader.PropertyToID("_WindSpeed");
        private static readonly int PropWindFrequency     = Shader.PropertyToID("_WindFrequency");

        [Header("Base")]
        public Color baseColor = Color.white;

        [Header("Outline")]
        public bool outline;
        public Color outlineColor = Color.black;
        [Range(0f, 10f)] public float outlineSize = 1f;

        [Header("Glow")]
        public bool glow;
        public Color glowColor = new Color(0f, 1f, 1f, 1f);
        [Range(0f, 10f)] public float glowSize = 2f;
        [Range(0f, 10f)] public float glowIntensity = 3f;

        [Header("Flash")]
        public bool flash;
        public Color flashColor = Color.white;
        [Range(0f, 1f)] public float flashAmount;

        [Header("Color Overlay")]
        public bool colorOverlay;
        public Color overlayColor = Color.red;
        [Range(0f, 1f)] public float overlayAmount = 0.5f;

        [Header("Hue Shift")]
        public bool hueShift;
        [Range(0f, 1f)] public float hueShiftAmount;

        [Header("Color Adjust")]
        public bool colorAdjust;
        [Range(-1f, 1f)] public float brightness;
        [Range(-1f, 1f)] public float contrast;
        [Range(-1f, 1f)] public float saturation;

        [Header("Dissolve")]
        public bool dissolve;
        public Texture2D dissolveTex;
        [Range(0f, 1f)] public float dissolveAmount;
        [Range(0f, 0.3f)] public float dissolveEdgeWidth = 0.05f;
        public Color dissolveEdgeColor = new Color(1f, 0.5f, 0f, 1f);

        [Header("Pixelate")]
        public bool pixelate;
        [Range(2f, 512f)] public float pixelateSize = 32f;

        [Header("Shine")]
        public bool shine;
        public Color shineColor = Color.white;
        [Range(0f, 1f)] public float shineWidth = 0.1f;
        [Range(-180f, 180f)] public float shineAngle = 45f;
        [Range(0f, 1f)] public float shineGlossiness = 0.5f;
        public float shineSpeed = 1f;

        [Header("Hologram")]
        public bool hologram;
        public Color hologramColor = new Color(0f, 1f, 1f, 1f);
        public float hologramLineSpeed = 2f;
        [Range(0.01f, 1f)] public float hologramLineSize = 0.05f;
        [Range(0f, 0.1f)] public float hologramDistortion = 0.01f;
        [Range(0f, 1f)] public float hologramAlpha = 0.5f;

        [Header("Glitch")]
        public bool glitch;
        [Range(0f, 1f)] public float glitchIntensity = 0.3f;
        public float glitchSpeed = 1f;
        [Range(0.01f, 1f)] public float glitchBlockSize = 0.1f;

        [Header("Grayscale")]
        public bool grayscale;
        [Range(0f, 1f)] public float grayscaleAmount = 1f;

        [Header("Wave")]
        public bool wave;
        [Range(0f, 0.1f)] public float waveAmplitude = 0.02f;
        [Range(0f, 20f)]  public float waveFrequency = 5f;
        public float waveSpeed = 1f;

        [Header("Wind Sway")]
        public bool windSway;
        [Range(0f, 0.15f)] public float windStrength = 0.03f;
        public float windSpeed = 1.5f;
        [Range(0f, 10f)] public float windFrequency = 1f;

        private Renderer _renderer;
        private Material _materialInstance;
        private Coroutine _flashCoroutine;

        private void OnEnable()
        {
            _renderer = GetComponent<Renderer>();
            EnsureMaterialInstance();
            ApplyAllProperties();
        }

        private void EnsureMaterialInstance()
        {
            if (_materialInstance != null) return;
            _renderer ??= GetComponent<Renderer>();
            CreateMaterialInstance();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureMaterialInstance();
            ApplyAllProperties();
        }
#endif

        private void OnDestroy()
        {
            if (_materialInstance != null)
                Destroy(_materialInstance);
        }

        private void CreateMaterialInstance()
        {
            var shader = Shader.Find(ShaderName);
            if (shader == null)
            {
                Debug.LogError($"[SpriteShadersComponent] Shader '{ShaderName}' not found. " +
                               "Make sure the shader asset is in the project.", this);
                return;
            }

            _materialInstance = new Material(shader) { name = $"{gameObject.name}_SpriteShadersMat" };
            _renderer.material = _materialInstance;
        }

        /// <summary>Pushes all serialized fields to the material instance.</summary>
        public void ApplyAllProperties()
        {
            if (_materialInstance == null) return;

            _materialInstance.SetColor(PropColor, baseColor);

            SetKeywordAndProperties("OUTLINE_ON",       outline,      SetOutlineProperties);
            SetKeywordAndProperties("GLOW_ON",          glow,         SetGlowProperties);
            SetKeywordAndProperties("FLASH_ON",         flash,        SetFlashProperties);
            SetKeywordAndProperties("COLOR_OVERLAY_ON", colorOverlay, SetOverlayProperties);
            SetKeywordAndProperties("HUE_SHIFT_ON",     hueShift,     SetHueShiftProperties);
            SetKeywordAndProperties("COLOR_ADJUST_ON",  colorAdjust,  SetColorAdjustProperties);
            SetKeywordAndProperties("DISSOLVE_ON",      dissolve,     SetDissolveProperties);
            SetKeywordAndProperties("PIXELATE_ON",      pixelate,     SetPixelateProperties);
            SetKeywordAndProperties("SHINE_ON",         shine,        SetShineProperties);
            SetKeywordAndProperties("HOLOGRAM_ON",      hologram,     SetHologramProperties);
            SetKeywordAndProperties("GLITCH_ON",        glitch,       SetGlitchProperties);
            SetKeywordAndProperties("GRAYSCALE_ON",     grayscale,    SetGrayscaleProperties);
            SetKeywordAndProperties("WAVE_ON",          wave,         SetWaveProperties);
            SetKeywordAndProperties("WIND_ON",          windSway,     SetWindSwayProperties);
        }

        private void SetKeywordAndProperties(string keyword, bool enabled, System.Action setProps)
        {
            if (enabled)
            {
                _materialInstance.EnableKeyword(keyword);
                setProps?.Invoke();
            }
            else
            {
                _materialInstance.DisableKeyword(keyword);
            }
        }

        private void SetOutlineProperties()
        {
            _materialInstance.SetColor(PropOutlineColor, outlineColor);
            _materialInstance.SetFloat(PropOutlineSize,  outlineSize);
        }

        private void SetGlowProperties()
        {
            _materialInstance.SetColor(PropGlowColor,     glowColor);
            _materialInstance.SetFloat(PropGlowSize,      glowSize);
            _materialInstance.SetFloat(PropGlowIntensity, glowIntensity);
        }

        private void SetFlashProperties()
        {
            _materialInstance.SetColor(PropFlashColor,  flashColor);
            _materialInstance.SetFloat(PropFlashAmount, flashAmount);
        }

        private void SetOverlayProperties()
        {
            _materialInstance.SetColor(PropOverlayColor,  overlayColor);
            _materialInstance.SetFloat(PropOverlayAmount, overlayAmount);
        }

        private void SetHueShiftProperties()
        {
            _materialInstance.SetFloat(PropHueShift, hueShiftAmount);
        }

        private void SetColorAdjustProperties()
        {
            _materialInstance.SetFloat(PropBrightness, brightness);
            _materialInstance.SetFloat(PropContrast,   contrast);
            _materialInstance.SetFloat(PropSaturation, saturation);
        }

        private void SetDissolveProperties()
        {
            if (dissolveTex != null)
                _materialInstance.SetTexture("_DissolveTex", dissolveTex);
            _materialInstance.SetFloat(PropDissolveAmount,    dissolveAmount);
            _materialInstance.SetFloat(PropDissolveEdge,      dissolveEdgeWidth);
            _materialInstance.SetColor(PropDissolveEdgeColor, dissolveEdgeColor);
        }

        private void SetPixelateProperties()
        {
            _materialInstance.SetFloat(PropPixelateSize, pixelateSize);
        }

        private void SetShineProperties()
        {
            _materialInstance.SetColor(PropShineColor,      shineColor);
            _materialInstance.SetFloat(PropShineWidth,      shineWidth);
            _materialInstance.SetFloat(PropShineAngle,      shineAngle);
            _materialInstance.SetFloat(PropShineGlossiness, shineGlossiness);
            _materialInstance.SetFloat(PropShineSpeed,      shineSpeed);
        }

        private void SetHologramProperties()
        {
            _materialInstance.SetColor(PropHologramColor,       hologramColor);
            _materialInstance.SetFloat(PropHologramLineSpeed,   hologramLineSpeed);
            _materialInstance.SetFloat(PropHologramLineSize,    hologramLineSize);
            _materialInstance.SetFloat(PropHologramDistortion,  hologramDistortion);
            _materialInstance.SetFloat(PropHologramAlpha,       hologramAlpha);
        }

        private void SetGlitchProperties()
        {
            _materialInstance.SetFloat(PropGlitchIntensity, glitchIntensity);
            _materialInstance.SetFloat(PropGlitchSpeed,     glitchSpeed);
            _materialInstance.SetFloat(PropGlitchBlockSize, glitchBlockSize);
        }

        private void SetGrayscaleProperties()
        {
            _materialInstance.SetFloat(PropGrayscaleAmount, grayscaleAmount);
        }

        private void SetWaveProperties()
        {
            _materialInstance.SetFloat(PropWaveAmplitude, waveAmplitude);
            _materialInstance.SetFloat(PropWaveFrequency, waveFrequency);
            _materialInstance.SetFloat(PropWaveSpeed,     waveSpeed);
        }

        private void SetWindSwayProperties()
        {
            _materialInstance.SetFloat(PropWindStrength,  windStrength);
            _materialInstance.SetFloat(PropWindSpeed,     windSpeed);
            _materialInstance.SetFloat(PropWindFrequency, windFrequency);
        }

        /// <summary>Enables or disables an effect keyword on the material.</summary>
        public void SetEffect(string keyword, bool enabled)
        {
            if (_materialInstance == null) return;
            if (enabled) _materialInstance.EnableKeyword(keyword);
            else         _materialInstance.DisableKeyword(keyword);
        }

        /// <param name="size">Outline pixel size (0-10).</param>
        public void SetOutlineSize(float size)
        {
            outlineSize = size;
            _materialInstance?.SetFloat(PropOutlineSize, size);
        }

        /// <param name="amount">Flash amount 0-1 (0 = no flash, 1 = full flash color).</param>
        public void SetFlashAmount(float amount)
        {
            flashAmount = amount;
            _materialInstance?.SetFloat(PropFlashAmount, amount);
        }

        /// <param name="amount">Dissolve progress 0-1.</param>
        public void SetDissolveAmount(float amount)
        {
            dissolveAmount = amount;
            _materialInstance?.SetFloat(PropDissolveAmount, amount);
        }

        /// <summary>
        /// Animates a flash-to-color effect over the given duration.
        /// </summary>
        public void FlashWhite(float duration = 0.2f)
        {
            Flash(Color.white, duration);
        }

        /// <summary>
        /// Animates a flash to a given color over the given duration.
        /// </summary>
        public void Flash(Color color, float duration = 0.2f)
        {
            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(FlashRoutine(color, duration));
        }

        /// <summary>
        /// Animates a dissolve-out effect.
        /// </summary>
        public void DissolveOut(float duration = 1f, System.Action onComplete = null)
        {
            StartCoroutine(DissolveRoutine(0f, 1f, duration, onComplete));
        }

        /// <summary>
        /// Animates a dissolve-in effect.
        /// </summary>
        public void DissolveIn(float duration = 1f, System.Action onComplete = null)
        {
            StartCoroutine(DissolveRoutine(1f, 0f, duration, onComplete));
        }

        private IEnumerator FlashRoutine(Color color, float duration)
        {
            var wasEnabled = flash;
            var prevColor = flashColor;

            flash = true;
            flashColor = color;
            SetEffect("FLASH_ON", true);
            _materialInstance.SetColor(PropFlashColor, color);

            var half = duration * 0.5f;
            var t = 0f;

            // Fade in
            while (t < half)
            {
                t += Time.deltaTime;
                var amount = Mathf.Clamp01(t / half);
                _materialInstance.SetFloat(PropFlashAmount, amount);
                yield return null;
            }

            // Fade out
            t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                var amount = 1f - Mathf.Clamp01(t / half);
                _materialInstance.SetFloat(PropFlashAmount, amount);
                yield return null;
            }

            flash = wasEnabled;
            flashColor = prevColor;
            flashAmount = 0f;
            _materialInstance.SetFloat(PropFlashAmount, 0f);
            if (!wasEnabled) SetEffect("FLASH_ON", false);
        }

        private IEnumerator DissolveRoutine(float from, float to, float duration, System.Action onComplete)
        {
            var wasEnabled = dissolve;
            dissolve = true;
            SetEffect("DISSOLVE_ON", true);

            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                var amount = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
                _materialInstance.SetFloat(PropDissolveAmount, amount);
                yield return null;
            }

            _materialInstance.SetFloat(PropDissolveAmount, to);
            dissolveAmount = to;

            if (!wasEnabled && to <= 0f)
            {
                dissolve = false;
                SetEffect("DISSOLVE_ON", false);
            }

            onComplete?.Invoke();
        }
    }
}
