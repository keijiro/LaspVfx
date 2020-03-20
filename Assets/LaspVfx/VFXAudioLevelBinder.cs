using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Lasp.Vfx
{
    [AddComponentMenu("VFX/Property Binders/Lasp/Audio Level Binder")]
    [VFXBinder("Lasp/Audio Level")]
    class VFXAudioLevelBinder : VFXBinderBase
    {
        public string Property
          { get => (string)_property; set => _property = value; }

        [VFXPropertyBinding("System.Single"), SerializeField]
        protected ExposedProperty _property = "AudioLevel";

        public Lasp.AudioLevelTracker Target = null;

        public override bool IsValid(VisualEffect component)
          => Target != null && component.HasFloat(_property);

        public override void UpdateBinding(VisualEffect component)
          => component.SetFloat(_property, Target.normalizedLevel);

        public override string ToString()
          => $"Audio Level : '{_property}' -> {Target?.name ?? "(null)"}";
    }
}
