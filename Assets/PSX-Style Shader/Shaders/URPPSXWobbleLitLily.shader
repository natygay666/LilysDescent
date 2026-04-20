Shader "URP/PSX/WobbleLitLily"
{
Properties
{
_BaseColor("Color", Color) = (0.7,0.7,0.7,1)

    _SnapResolution("Snap Resolution", Vector) = (320,240,0,0)
    _SnapStrength("Snap Strength", Range(0,1)) = 1

    _WobbleStrength("Wobble Strength", Range(0,0.1)) = 0.02
    _WobbleSpeed("Wobble Speed", Range(0,10)) = 2
}

SubShader
{
    Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

    Pass
    {
        Name "ForwardLit"
        Tags { "LightMode"="UniversalForward" }

        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normal : NORMAL;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float3 normalWS : TEXCOORD0;
        };

        float4 _BaseColor;

        float4 _SnapResolution;
        float _SnapStrength;

        float _WobbleStrength;
        float _WobbleSpeed;

        // pseudo random
        float hash(float3 p)
        {
            return frac(sin(dot(p, float3(12.9898,78.233,45.164))) * 43758.5453);
        }

        Varyings vert (Attributes v)
        {
            Varyings o;

            float3 pos = v.positionOS.xyz;
            
            float t = _Time.y * _WobbleSpeed;
            float noise = hash(pos + t);
            pos += (noise - 0.5) * _WobbleStrength;
            
            float2 res = max(_SnapResolution.xy, 1.0);

            float4 clip = TransformObjectToHClip(pos);
            float2 ndc = clip.xy / clip.w;

            float2 pixelStep = 2.0 / res;
            float2 snapped = floor(ndc / pixelStep + 0.5) * pixelStep;

            ndc = lerp(ndc, snapped, _SnapStrength);

            clip.xy = ndc * clip.w;

            o.positionHCS = clip;
            o.normalWS = TransformObjectToWorldNormal(v.normal);

            return o;
        }

        float4 frag (Varyings i) : SV_Target
        {
            float3 lightDir = normalize(float3(0.3,0.7,0.5));
            float NdotL = saturate(dot(i.normalWS, lightDir));

            // iluminación simple tipo PS1
            float light = floor(NdotL * 4) / 4;

            float3 col = _BaseColor.rgb * light;

            return float4(col, 1.0);
        }

        ENDHLSL
    }
}

}