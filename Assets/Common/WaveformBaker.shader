Shader "Hidden/Lasp/WaveformBaker"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    StructuredBuffer<float> _Waveform;
    uint _Width;

    half4 frag(v2f_img input) : SV_Target
    {
        uint x = input.uv.x * _Width;
        uint y = input.uv.y * 4;

        return _Waveform[x + y * _Width];
    }

    ENDCG
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
