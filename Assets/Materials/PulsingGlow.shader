Shader "Custom/PulsingGlow"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2.0
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.0
        _Transparency ("Transparency", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha

        struct Input
        {
            float2 uv_MainTex;
        };

        // Properties
        fixed4 _BaseColor;
        fixed4 _GlowColor;
        float _GlowIntensity;
        float _PulseSpeed;
        float _Transparency;

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Compute pulsing effect
            float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5; // Normalize [0, 1]
            float glow = _GlowIntensity * pulse;

            // Apply glow effect
            fixed4 glowEffect = _GlowColor * glow;

            // Set final output color
            o.Albedo = _BaseColor.rgb;
            o.Emission = glowEffect.rgb;
            o.Alpha = _Transparency;
        }
        ENDCG
    }
}
