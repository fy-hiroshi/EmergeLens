Shader "Hidden/BarrelDistortion"
{
    Properties
    {
        // Positive values bulge the image outward (barrel), negative pinch it
        // inward (pincushion). Tune this while looking through your actual
        // viewer's lenses -- the right value depends on your specific lens.
        _Strength ("Distortion Strength", Range(-1, 1)) = 0.25
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "BarrelDistortion"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Strength;

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 cc = uv - 0.5;            // recenter: -0.5 .. 0.5
                float r2 = dot(cc, cc) * 4.0;     // ~0 at center, ~1 at corners
                float2 sampleUV = uv + cc * r2 * _Strength;

                // Outside the source image after warping -> letterbox black
                // instead of stretching/wrapping garbage at the edges.
                if (sampleUV.x < 0.0 || sampleUV.x > 1.0 || sampleUV.y < 0.0 || sampleUV.y > 1.0)
                    return float4(0, 0, 0, 1);

                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, sampleUV);
            }
            ENDHLSL
        }
    }
}
