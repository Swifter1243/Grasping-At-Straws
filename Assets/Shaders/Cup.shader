Shader "Unlit/Cup"
{
    Properties
    {
        _BodyColor ("Body Color", Color) = (1, 0.3, 0.3)
        _LidColor ("Lid Color", Color) = (0.8, 0.8, 0.8)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
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
                float color : COLOR;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float color : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 reflection : TEXCOORD2;
                float3 localPos : TEXCOORD3;
            };

            #define SUN_DIRECTION float3(0, 0.980581, 0.196116)

            float3 _BodyColor;
            float3 _LidColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;

                o.normal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 viewVector = worldPos - _WorldSpaceCameraPos;
                float3 viewDir = normalize(viewVector);
                float3 reflection = reflect(viewDir, o.normal);
                o.reflection = reflection;
                o.localPos = v.vertex;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float sunD = dot(i.normal, SUN_DIRECTION);
                float lighting = sunD * 0.5 + 0.5;

                float specularD = dot(i.reflection, SUN_DIRECTION);

                lighting += pow(max(0.03, specularD * max(0, sunD)), 3) * 20;

                float3 col = lerp(_BodyColor, _LidColor, i.color);

                bool isLid = i.localPos.y > 0.5 && i.color > 0.8;
                float alpha = lerp(1, 0.6, isLid);

                return float4(col * lighting, alpha);
            }
            ENDCG
        }
    }
}