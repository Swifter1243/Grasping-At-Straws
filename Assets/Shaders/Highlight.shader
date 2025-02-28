Shader "Unlit/Highlight"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1)
        _Opacity ("Opacity", Range(0,1)) = 1
        _Brightness ("Brightness", Float) = 3
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"= "Transparent+4"
            "PreviewType"="Plane"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            float3 _Color;
            float _Opacity;
            float _Brightness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 centerUV = i.uv * 2 - 1;
                float angle = atan2(centerUV.y, centerUV.x);
                float s = sin(angle * 10 + _Time.y);
                float v = step(0, s);

                float l = 1 - length(centerUV);
                float circle = max(0, l * l * l);
                v *= circle * _Brightness;

                float4 finalCol = float4(_Color, lerp(v, circle, 0.2) * _Opacity);
                
                return finalCol;
            }
            ENDCG
        }
    }
}