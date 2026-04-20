Shader "URP/PSX/FakeVolumetricFog"
{
Properties
{
_Color("Fog Color", Color) = (0.8,0.8,0.8,0.5)
_MainTex("Noise Texture", 2D) = "white" {}

    _ScrollSpeed("Scroll Speed", Vector) = (0.1,0.05,0,0)
    _Intensity("Intensity", Range(0,2)) = 1

    _Softness("Softness", Range(0,5)) = 2
}

SubShader
{
    Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }

    Pass
    {
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        float4 _Color;
        float4 _ScrollSpeed;
        float _Intensity;
        float _Softness;

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 worldPos : TEXCOORD1;
        };

        Varyings vert (Attributes v)
        {
            Varyings o;
            o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
            o.uv = v.uv;
            o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
            return o;
        }

        float4 frag (Varyings i) : SV_Target
        {
            float2 uv = i.uv;
            
            uv += _Time.y * _ScrollSpeed.xy;

            float noise = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).r;

            float alpha = noise * _Color.a * _Intensity;
            
            float dist = distance(_WorldSpaceCameraPos, i.worldPos);
            alpha *= saturate(1.0 / (dist * 0.1 + 1));

            return float4(_Color.rgb, alpha);
        }

        ENDHLSL
    }
}

}