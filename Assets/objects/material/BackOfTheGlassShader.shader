Shader "Custom/BackOfTheGlass"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}          // Main texture for the glass
        _DistortionTex ("Distortion Map", 2D) = "black" {}  // Texture for distortion (like a normal map)
        _BlurSize ("Blur Size", Range(0, 10)) = 2.0         // Controls how much blur is applied
        _Transparency ("Transparency", Range(0, 1)) = 0.5   // Controls the transparency of the glass
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;         // The main texture
            sampler2D _DistortionTex;   // The texture that causes the distortion
            float _BlurSize;            // The amount of blur
            float _Transparency;        // How transparent the glass is

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

    float2 offset = _BlurSize * texelSize;   // Apply the calculated texel size
    float4 distortion = tex2D(_DistortionTex, i.uv);     // Sample the distortion texture
    float4 color = tex2D(_MainTex, i.uv + distortion.xy * offset); // Apply distortion and blur
    color.a *= _Transparency;  // Adjust transparency
    return color;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
