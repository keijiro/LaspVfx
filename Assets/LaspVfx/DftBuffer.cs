using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Profiling;

namespace Lasp.Vfx
{

// DFT with the C# job system and the Burst compiler

public sealed class DftBuffer : System.IDisposable
{
    #region Public properties

    public int Width { get; private set; }
    public NativeArray<float> Spectrum => _spectrum;

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        if (_input   .IsCreated) _input   .Dispose();
        if (_spectrum.IsCreated) _spectrum.Dispose();
        if (_window  .IsCreated) _window  .Dispose();
        if (_coeffsR .IsCreated) _coeffsR .Dispose();
        if (_coeffsI .IsCreated) _coeffsI .Dispose();
    }

    #endregion

    #region Public methods

    public DftBuffer(int width)
    {
        Width = width;

        // Hanning window
        var window = Enumerable.Range(0, Width).
            Select(n => 2 * math.PI * n / (Width - 1)).
            Select(x => 0.5f * (1 - math.cos(x)));

        // DFT coefficients
        var coeffs = Enumerable.Range(0, Width / 2 * Width).
            Select(i => (k: i / Width, n: i % Width)).
            Select(I => 2 * math.PI / Width * I.k * I.n);

        var coeffsR = coeffs.Select(x => math.cos(x));
        var coeffsI = coeffs.Select(x => math.sin(x));

        // Native array allocation and initialization
        var ator = Allocator.Persistent;
        _input    = new NativeArray<float>(Width            , ator);
        _spectrum = new NativeArray<float>(Width / 2        , ator);
        _window   = new NativeArray<float>(window .ToArray(), ator);
        _coeffsR  = new NativeArray<float>(coeffsR.ToArray(), ator);
        _coeffsI  = new NativeArray<float>(coeffsI.ToArray(), ator);
    }

    // Push audio data to the FIFO buffer.
    public void Push(NativeSlice<float> data)
    {
        var length = data.Length;

        if (length == 0) return;

        if (length < Width)
        {
            // The data is smaller than the buffer: Dequeue and copy
            var part = Width - length;
            NativeArray<float>.Copy(_input, Width - part, _input, 0, part);
            data.CopyTo(_input.GetSubArray(part, length));
        }
        else
        {
            // The data is larger than the buffer: Simple fill
            data.Slice(length - Width).CopyTo(_input);
        }
    }

    // Analyze the input buffer to calculate spectrum data.
    public void Analyze()
    {
        Profiler.BeginSample("Spectrum Analyer DFT");

        using (var temp = AllocateTempJobMemory<float>(Width))
        {
            // Preparation job (window function)
            var job1 = new PreparationJob
            {
                input  = _input .Reinterpret<float4>(4),
                window = _window.Reinterpret<float4>(4),
                output = temp   .Reinterpret<float4>(4),
            };

            // DFT job
            var job2 = new DftJob
            {
                input   = temp    .Reinterpret<float4>(4),
                coeffsR = _coeffsR.Reinterpret<float4>(4),
                coeffsI = _coeffsI.Reinterpret<float4>(4),
                output  = _spectrum
            };

            // Dispatch and wait.
            var handle = new JobHandle();
            handle = job1.Schedule(Width / 4, handle);
            handle = job2.Schedule(Width / 2, 4, handle);
            handle.Complete();
        }

        Profiler.EndSample();
    }

    #endregion

    #region Internal members

    NativeArray<float> _input;
    NativeArray<float> _spectrum;
    NativeArray<float> _window;
    NativeArray<float> _coeffsR;
    NativeArray<float> _coeffsI;

    NativeArray<T> AllocateTempJobMemory<T>(int size)
        where T : unmanaged => new NativeArray<T>
            (size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

    #endregion

    #region Preparation job

    [Unity.Burst.BurstCompile(CompileSynchronously = true)]
    struct PreparationJob : IJobFor
    {
        [ReadOnly] public NativeArray<float4> input;
        [ReadOnly] public NativeArray<float4> window;
        [WriteOnly] public NativeArray<float4> output;

        public void Execute(int i) => output[i] = input[i] * window[i];
    }

    #endregion

    #region DFT kernel job

    [Unity.Burst.BurstCompile(CompileSynchronously = true)]
    struct DftJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> input;
        [ReadOnly] public NativeArray<float4> coeffsR;
        [ReadOnly] public NativeArray<float4> coeffsI;
        [WriteOnly] public NativeArray<float> output;

        public void Execute(int i)
        {
            var offs = i * input.Length;

            var rl = 0.0f;
            var im = 0.0f;

            for (var n = 0; n < input.Length; n++)
            {
                var x_n = input[n];
                rl += math.dot(x_n, coeffsR[offs + n]);
                im -= math.dot(x_n, coeffsI[offs + n]);
            }

            output[i] = math.sqrt(rl * rl + im * im) * 0.5f / input.Length;
        }
    }

    #endregion
}

}
