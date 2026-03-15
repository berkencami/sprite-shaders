# SpriteShaders
A modular, multi-effect sprite shader for Unity 6 URP.


14 independent effects in a single-pass shader. Keyword-based compilation means disabled effects have zero GPU cost. Drop-in ready for any URP SpriteRenderer.


![ScreenShot](https://github.com/berkencami/sprite-shader-internal/blob/main/gif.gif)

## Effects

| Effect | Description |
|--------|-------------|
| Outline | 8-directional pixel outline |
| Glow | Soft HDR glow around sprite edges |
| Flash / Hit | Lerp to color — damage/hit feedback |
| Color Overlay | Blend sprite with a solid color |
| Hue Shift | Animated hue rotation |
| Color Adjust | Brightness, Contrast, Saturation |
| Grayscale | Desaturate with blendable amount |
| Dissolve | Noise-texture dissolution with glowing edge |
| Pixelate | Block pixelation |
| Shine | Animated diagonal highlight sweep |
| Hologram | Scanlines + flicker sci-fi effect |
| Glitch | Random block-offset distortion |
| Wave | Sine-wave UV wobble |
| Wind Sway | Height-based vertex sway |
