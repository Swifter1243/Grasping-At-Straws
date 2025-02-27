Shader "Unlit/SodaSkybox"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1)
        _NoiseScale ("Noise Scale", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Background"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Noise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            float3 _Color1;
            float3 _Color2;
            float _NoiseScale;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 p = _NoiseScale * i.localPos;
                float n = simplex(p + _Time.y * 0.05);
                n += simplex(p * 2 + n - _Time.y * 0.03) * 0.5;

                float3 col = lerp(_Color1, _Color2, n);
                
                return float4(col, 0);
            }
            ENDCG
        }
    }
}