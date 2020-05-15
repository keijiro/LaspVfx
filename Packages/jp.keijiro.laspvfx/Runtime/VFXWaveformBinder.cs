using Unity.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Lasp.Vfx
{
    [AddComponentMenu("VFX/Property Binders/LASP/Waveform Binder")]
    [VFXBinder("LASP/Waveform")]
    sealed class VFXWaveformBinder : VFXBinderBase
    {
        #region VFX Binder Implementation

        public string TextureProperty {
            get => (string)_textureProperty;
            set => _textureProperty = value;
        }

        public string TextureWidthProperty {
            get => (string)_textureWidthProperty;
            set => _textureWidthProperty = value;
        }

        public string SampleCountProperty {
            get => (string)_sampleCountProperty;
            set => _sampleCountProperty = value;
        }

        [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
        ExposedProperty _textureProperty = "WaveformTexture";

        [VFXPropertyBinding("System.UInt32"), SerializeField]
        ExposedProperty _textureWidthProperty = "TextureWidth";

        [VFXPropertyBinding("System.UInt32"), SerializeField]
        ExposedProperty _sampleCountProperty = "SampleCount";

        public Lasp.AudioLevelTracker Target = null;

        public override bool IsValid(VisualEffect component)
          => Target != null &&
             component.HasTexture(_textureProperty) &&
             component.HasUInt(_textureWidthProperty) &&
             component.HasUInt(_sampleCountProperty);

        public override void UpdateBinding(VisualEffect component)
        {
            UpdateTexture();
            component.SetTexture(_textureProperty, _texture);
            component.SetUInt(_textureWidthProperty, (uint)MaxSamples);
            component.SetUInt(_sampleCountProperty, (uint)_sampleCount);
        }

        public override string ToString()
          => $"Waveform : '{_textureProperty}' -> {Target?.name ?? "(null)"}";

        #endregion

        #region Waveform texture generation

        const int MaxSamples = 4096;

        Texture2D _texture;
        NativeArray<float> _buffer;
        int _sampleCount;

        void OnDestroy()
        {
            if (_texture != null)
                if (Application.isPlaying)
                    Destroy(_texture);
                else
                    DestroyImmediate(_texture);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_buffer.IsCreated) _buffer.Dispose();
        }

        void UpdateTexture()
        {
            if (_texture == null)
            {
                _texture =
                  new Texture2D(MaxSamples, 1, TextureFormat.RFloat, false) {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                  };
            }

            if (!_buffer.IsCreated)
                _buffer = new NativeArray<float>
                  (MaxSamples, Allocator.Persistent,
                   NativeArrayOptions.UninitializedMemory);

            var slice = Target.audioDataSlice;
            _sampleCount = Mathf.Min(_buffer.Length, slice.Length);

            if (_sampleCount > 0)
            {
                slice.CopyTo(_buffer.GetSubArray(0, _sampleCount));
                _texture.LoadRawTextureData(_buffer);
                _texture.Apply();
            }
        }

        #endregion
    }
}
