Shader "Custom/Blurrr"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            uniform float _BlurSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = float2(_BlurSize, _BlurSize) / _ScreenParams.xy;
                fixed4 color = tex2D(_MainTex, i.uv) * 0.2;
                color += tex2D(_MainTex, i.uv + offset) * 0.2;
                color += tex2D(_MainTex, i.uv - offset) * 0.2;
                color += tex2D(_MainTex, i.uv + float2(offset.x, -offset.y)) * 0.2;
                color += tex2D(_MainTex, i.uv - float2(offset.x, -offset.y)) * 0.2;

                color.a = 0.5; // Adjust transparency (0 = fully see-through, 1 = fully opaque)
                return color;
            }
            ENDCG
        }
    }
}