using UnityEditor;
using UnityEngine;

namespace SpriteShaders.Editor
{
    /// <summary>
    /// Custom ShaderGUI for SpriteShaders.
    /// Shows effect toggles with foldout groups and reveals properties only when the effect is active.
    /// </summary>
    public class SpriteShadersEditor : ShaderGUI
    {
        private static bool s_BaseFoldout          = true;
        private static bool s_ColorAdjustFoldout   = false;
        private static bool s_BorderFoldout         = false;
        private static bool s_DissolveFoldout       = false;
        private static bool s_SpecialFoldout        = false;
        private static bool s_RenderingFoldout      = false;

        private static GUIStyle s_HeaderStyle;
        private static GUIStyle s_SubHeaderStyle;
        private static readonly Color HeaderColor   = new Color(0.15f, 0.15f, 0.15f);
        private static readonly Color AccentColor   = new Color(0.2f, 0.6f, 1.0f);

        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            InitStyles();

            var mat = editor.target as Material;
            if (mat == null) return;

            EditorGUI.BeginChangeCheck();

            DrawHeader();
            DrawBaseSection(editor, props, mat);
            DrawColorAdjustSection(editor, props, mat);
            DrawBorderSection(editor, props, mat);
            DrawDissolveSection(editor, props, mat);
            DrawSpecialEffectsSection(editor, props, mat);
            DrawRenderingSection(editor, props, mat);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(mat);
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(4);
            var rect = EditorGUILayout.GetControlRect(false, 36);
            EditorGUI.DrawRect(rect, HeaderColor);

            var labelRect = rect;
            labelRect.x += 8;
            EditorGUI.LabelField(labelRect, "★  Sprite Shaders", s_HeaderStyle);
            EditorGUILayout.Space(4);
        }

        private void DrawBaseSection(MaterialEditor editor, MaterialProperty[] props, Material mat)
        {
            s_BaseFoldout = DrawFoldout("Base", s_BaseFoldout, AccentColor);
            if (!s_BaseFoldout) return;

            EditorGUI.indentLevel++;
            DrawProperty(editor, props, "_MainTex", "Sprite Texture");
            DrawProperty(editor, props, "_Color",   "Tint Color");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        private void DrawColorAdjustSection(MaterialEditor editor, MaterialProperty[] props, Material mat)
        {
            s_ColorAdjustFoldout = DrawFoldout("Color Adjustments", s_ColorAdjustFoldout, AccentColor);
            if (!s_ColorAdjustFoldout) return;

            EditorGUI.indentLevel++;

            DrawEffectToggle(editor, props, mat, "_HueShiftToggle", "HUE_SHIFT_ON", "Hue Shift",
                () => {
                    DrawProperty(editor, props, "_HueShift", "Hue Shift");
                    DrawProperty(editor, props, "_HueShiftSpeed", "Speed");
                });

            DrawEffectToggle(editor, props, mat, "_ColorAdjustToggle", "COLOR_ADJUST_ON", "Brightness / Contrast / Saturation",
                () =>
                {
                    DrawProperty(editor, props, "_Brightness", "Brightness");
                    DrawProperty(editor, props, "_Contrast",   "Contrast");
                    DrawProperty(editor, props, "_Saturation", "Saturation");
                });

            DrawEffectToggle(editor, props, mat, "_GrayscaleToggle", "GRAYSCALE_ON", "Grayscale",
                () => DrawProperty(editor, props, "_GrayscaleAmount", "Amount"));

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        private void DrawBorderSection(MaterialEditor editor, MaterialProperty[] props, Material mat)
        {
            s_BorderFoldout = DrawFoldout("Border Effects", s_BorderFoldout, AccentColor);
            if (!s_BorderFoldout) return;

            EditorGUI.indentLevel++;

            DrawEffectToggle(editor, props, mat, "_OutlineToggle", "OUTLINE_ON", "Outline",
                () =>
                {
                    DrawProperty(editor, props, "_OutlineColor", "Color");
                    DrawProperty(editor, props, "_OutlineSize",  "Size");
                });

            DrawEffectToggle(editor, props, mat, "_GlowToggle", "GLOW_ON", "Glow",
                () =>
                {
                    DrawProperty(editor, props, "_GlowColor",     "Color (HDR)");
                    DrawProperty(editor, props, "_GlowSize",      "Size");
                    DrawProperty(editor, props, "_GlowIntensity", "Intensity");
                });

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        private void DrawDissolveSection(MaterialEditor editor, MaterialProperty[] props, Material mat)
        {
            s_DissolveFoldout = DrawFoldout("Dissolve", s_DissolveFoldout, AccentColor);
            if (!s_DissolveFoldout) return;

            EditorGUI.indentLevel++;

            DrawEffectToggle(editor, props, mat, "_DissolveToggle", "DISSOLVE_ON", "Dissolve",
                () =>
                {
                    DrawProperty(editor, props, "_DissolveTex",       "Noise Texture");
                    DrawProperty(editor, props, "_DissolveAmount",    "Amount");
                    DrawProperty(editor, props, "_DissolveEdgeWidth", "Edge Width");
                    DrawProperty(editor, props, "_DissolveEdgeColor", "Edge Color");
                });

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        private void DrawSpecialEffectsSection(MaterialEditor editor, MaterialProperty[] props, Material mat)
        {
            s_SpecialFoldout = DrawFoldout("Special Effects", s_SpecialFoldout, AccentColor);
            if (!s_SpecialFoldout) return;

            EditorGUI.indentLevel++;

            DrawEffectToggle(editor, props, mat, "_FlashToggle", "FLASH_ON", "Flash / Hit",
                () =>
                {
                    DrawProperty(editor, props, "_FlashColor",  "Color");
                    DrawProperty(editor, props, "_FlashAmount", "Amount");
                });

            DrawEffectToggle(editor, props, mat, "_OverlayToggle", "COLOR_OVERLAY_ON", "Color Overlay",
                () =>
                {
                    DrawProperty(editor, props, "_OverlayColor",  "Color");
                    DrawProperty(editor, props, "_OverlayAmount", "Amount");
                });

            DrawEffectToggle(editor, props, mat, "_PixelateToggle", "PIXELATE_ON", "Pixelate",
                () => DrawProperty(editor, props, "_PixelateSize", "Pixel Size"));

            DrawEffectToggle(editor, props, mat, "_ShineToggle", "SHINE_ON", "Shine",
                () =>
                {
                    DrawProperty(editor, props, "_ShineColor",      "Color (HDR)");
                    DrawProperty(editor, props, "_ShineWidth",      "Width");
                    DrawProperty(editor, props, "_ShineAngle",      "Angle");
                    DrawProperty(editor, props, "_ShineGlossiness", "Glossiness");
                    DrawProperty(editor, props, "_ShineSpeed",      "Speed");
                });

            DrawEffectToggle(editor, props, mat, "_HologramToggle", "HOLOGRAM_ON", "Hologram",
                () =>
                {
                    DrawProperty(editor, props, "_HologramColor",       "Color (HDR)");
                    DrawProperty(editor, props, "_HologramLineSpeed",   "Line Speed");
                    DrawProperty(editor, props, "_HologramLineSize",    "Line Size");
                    DrawProperty(editor, props, "_HologramDistortion",  "Distortion");
                    DrawProperty(editor, props, "_HologramAlpha",       "Alpha");
                });

            DrawEffectToggle(editor, props, mat, "_GlitchToggle", "GLITCH_ON", "Glitch",
                () =>
                {
                    DrawProperty(editor, props, "_GlitchIntensity", "Intensity");
                    DrawProperty(editor, props, "_GlitchSpeed",     "Speed");
                    DrawProperty(editor, props, "_GlitchBlockSize", "Block Size");
                });

            DrawEffectToggle(editor, props, mat, "_WaveToggle", "WAVE_ON", "Wave / Wobble",
                () =>
                {
                    DrawProperty(editor, props, "_WaveAmplitude", "Amplitude");
                    DrawProperty(editor, props, "_WaveFrequency", "Frequency");
                    DrawProperty(editor, props, "_WaveSpeed",     "Speed");
                });

            DrawEffectToggle(editor, props, mat, "_WindToggle", "WIND_ON", "Wind Sway",
                () =>
                {
                    DrawProperty(editor, props, "_WindStrength",  "Strength");
                    DrawProperty(editor, props, "_WindSpeed",     "Speed");
                    DrawProperty(editor, props, "_WindFrequency", "Frequency");
                });

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        private void DrawRenderingSection(MaterialEditor editor, MaterialProperty[] props, Material mat)
        {
            s_RenderingFoldout = DrawFoldout("Rendering Options", s_RenderingFoldout, AccentColor);
            if (!s_RenderingFoldout) return;

            EditorGUI.indentLevel++;

            // Blend Mode
            var srcBlend = FindProperty("_SrcBlend", props, false);
            var dstBlend = FindProperty("_DstBlend", props, false);
            if (srcBlend != null && dstBlend != null)
            {
                EditorGUILayout.LabelField("Blend Mode", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                editor.ShaderProperty(srcBlend, "Src Blend");
                editor.ShaderProperty(dstBlend, "Dst Blend");
                EditorGUI.indentLevel--;
            }

            var zWrite = FindProperty("_ZWrite", props, false);
            var cull   = FindProperty("_Cull",   props, false);
            if (zWrite != null) editor.ShaderProperty(zWrite, "ZWrite");
            if (cull   != null) editor.ShaderProperty(cull,   "Culling");

            EditorGUILayout.Space(4);
            editor.RenderQueueField();

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        /// <summary>
        /// Draws a toggle for an effect. When enabled, shows the parameters via showParams callback.
        /// Also handles enabling/disabling the shader keyword on the material.
        /// </summary>
        private void DrawEffectToggle(
            MaterialEditor editor,
            MaterialProperty[] props,
            Material mat,
            string togglePropName,
            string keyword,
            string label,
            System.Action showParams)
        {
            var toggleProp = FindProperty(togglePropName, props, false);
            if (toggleProp == null) return;

            var isOn = toggleProp.floatValue > 0.5f;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUILayout.Toggle(label, isOn, EditorStyles.boldLabel);
            if (EditorGUI.EndChangeCheck())
            {
                toggleProp.floatValue = newValue ? 1f : 0f;
                foreach (var obj in editor.targets)
                {
                    var m = obj as Material;
                    if (m == null) continue;
                    if (newValue) m.EnableKeyword(keyword);
                    else          m.DisableKeyword(keyword);
                }
            }

            if (newValue)
            {
                EditorGUI.indentLevel++;
                showParams?.Invoke();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(1);
        }

        private void DrawProperty(MaterialEditor editor, MaterialProperty[] props, string name, string label)
        {
            var prop = FindProperty(name, props, false);
            if (prop != null)
                editor.ShaderProperty(prop, label);
        }

        private bool DrawFoldout(string title, bool state, Color accentColor)
        {
            var rect = EditorGUILayout.GetControlRect(false, 22);
            var bgColor = new Color(accentColor.r * 0.3f, accentColor.g * 0.3f, accentColor.b * 0.3f, 1f);
            EditorGUI.DrawRect(rect, bgColor);

            var labelRect = rect;
            labelRect.x += 14;

            var newState = EditorGUI.Foldout(rect, state, GUIContent.none, true);
            EditorGUI.LabelField(labelRect, title, s_SubHeaderStyle);

            return newState;
        }

        private static void InitStyles()
        {
            if (s_HeaderStyle != null) return;

            s_HeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 14,
                alignment = TextAnchor.MiddleLeft,
                normal    = { textColor = Color.white }
            };

            s_SubHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 11,
                alignment = TextAnchor.MiddleLeft,
                normal    = { textColor = Color.white }
            };
        }
    }
}
