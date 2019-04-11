using UnityEngine;

sealed class WaveformBaker : MonoBehaviour
{
    [SerializeField] RenderTexture _target = null;

    float[] _array;
    ComputeBuffer _buffer;
    Material _material;

    void Start()
    {
        var width = _target.width;

        _array = new float[width];
        _buffer = new ComputeBuffer(width * 4, sizeof(float));

        var shader = Shader.Find("Hidden/Lasp/WaveformBaker");
        _material = new Material(shader);
    }

    void OnDestroy()
    {
        _buffer.Dispose();
        Destroy(_material);
    }

    void Update()
    {
        var width = _target.width;

        Lasp.MasterInput.RetrieveWaveform(Lasp.FilterType.Bypass, _array);
        _buffer.SetData(_array, 0, 0, width);

        Lasp.MasterInput.RetrieveWaveform(Lasp.FilterType.LowPass, _array);
        _buffer.SetData(_array, 0, width, width);

        Lasp.MasterInput.RetrieveWaveform(Lasp.FilterType.BandPass, _array);
        _buffer.SetData(_array, 0, width * 2, width);

        Lasp.MasterInput.RetrieveWaveform(Lasp.FilterType.HighPass, _array);
        _buffer.SetData(_array, 0, width * 3, width);

        _material.SetBuffer("_Waveform", _buffer);
        _material.SetInt("_Width", width);

        Graphics.Blit(null, _target, _material, 0);
    }
}
