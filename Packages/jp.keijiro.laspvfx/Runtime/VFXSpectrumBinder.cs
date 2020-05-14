using Unity.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Lasp.Vfx
{
    [AddComponentMenu("VFX/Property Binders/LASP/Spectrum Binder")]
    [VFXBinder("LASP/Spectrum")]
    sealed class VFXSpectrumBinder : VFXBinderBase
    {
        #region VFX Binder Implementation

        public string TextureProperty {
            get => (string)_textureProperty;
            set => _textureProperty = value;
        }

        public string ResolutionProperty {
            get => (string)_resolutionProperty;
            set => _resolutionProperty = value;
        }

        [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
        ExposedProperty _textureProperty = "WaveformTexture";

        [VFXPropertyBinding("System.UInt32"), SerializeField]
        ExposedProperty _resolutionProperty = "Resolution";

        public Lasp.SpectrumToTexture Target = null;

        public override bool IsValid(VisualEffect component)
          => Target != null &&
             component.HasTexture(_textureProperty) &&
             component.HasUInt(_resolutionProperty);

        public override void UpdateBinding(VisualEffect component)
        {
            if (Target.texture == null) return;
            component.SetTexture(_textureProperty, Target.texture);
            component.SetUInt(_resolutionProperty, (uint)Target.texture.width);
        }

        public override string ToString()
          => $"Spectrum : '{_textureProperty}' -> {Target?.name ?? "(null)"}";

        #endregion
    }
}
