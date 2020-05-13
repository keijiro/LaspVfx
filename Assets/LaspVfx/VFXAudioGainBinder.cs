using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Lasp.Vfx
{
    [AddComponentMenu("VFX/Property Binders/LASP/Audio Gain Binder")]
    [VFXBinder("LASP/Audio Gain")]
    sealed class VFXAudioGainBinder : VFXBinderBase
    {
        public string Property
          { get => (string)_property; set => _property = value; }

        [VFXPropertyBinding("System.Single"), SerializeField]
        ExposedProperty _property = "AudioGain";

        public Lasp.AudioLevelTracker Target = null;

        public override bool IsValid(VisualEffect component)
          => Target != null && component.HasFloat(_property);

        public override void UpdateBinding(VisualEffect component)
          => component.SetFloat(_property, Target.currentGain);

        public override string ToString()
          => $"Audio Gain : '{_property}' -> {Target?.name ?? "(null)"}";
    }
}
