Shader "Unlit/Pipe"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1)
        _WaterColor ("Water Color", Color) = (1, 1, 1)
        _Progress ("Progress", Range(0, 1)) = 0
        _Opacity ("Opacity", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float3 normal : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            float3 _Color;
            float3 _WaterColor;
            float _Progress;
            float _Opacity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.localPos = v.vertex;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 viewVector = worldPos - _WorldSpaceCameraPos;
                o.viewDir = normalize(viewVector);
                return o;
            }

            #define SUN_DIRECTION float3(0, 1, 0)

            fixed4 frag(v2f i) : SV_Target
            {
                float sunD = dot(i.normal, SUN_DIRECTION);
                float lighting = sunD * 0.5 + 0.5;

                float3 reflection = reflect(i.viewDir, i.normal);
                float specularD = dot(reflection, SUN_DIRECTION);
                lighting += pow(max(0, specularD), 4) * 3;

                float z = i.localPos.y * 0.5 - 0.5;
                float y = z + (i.localPos.z - 0.6) * 0.1;

                float distortion = voronoi(float3(i.localPos.xz * 3, _Time.y * 2)) * 0.7;
                distortion += sin(_Time.y * 3 + i.localPos.xz * 5);
                
                y += (distortion - 0.7) * 0.02;
                float p = y + _Progress * 1.2;
                float waterAmount = smoothstep(0, 0.01, p);

                float3 col = lerp(_Color, _WaterColor, waterAmount);

                return float4(col * lighting, _Opacity);
            }
            ENDCG
        }
    }
}