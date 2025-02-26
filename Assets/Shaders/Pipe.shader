Shader "Unlit/Pipe"
{
    Properties
    {
        [Toggle(CORNER)] _IsCorner ("Is Corner", Int) = 0
        [ToggleUI] _FlipFluid ("Flip Fluid", Int) = 0
        _Color ("Color", Color) = (1, 1, 1)
        _WaterColor ("Water Color", Color) = (1, 1, 1)
        _FluidProgress ("Fluid Progress", Range(0, 1)) = 0
        _Opacity ("Opacity", Range(0,1)) = 1
        _OutlineColor ("Outline Color", Color) = (0,1,1,0)
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
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature CORNER

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float3 normal : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                float3 reflection : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            float3 _Color;
            float3 _WaterColor;
            float _FluidProgress;
            float _Opacity;
            bool _FlipFluid;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.localPos = v.vertex;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 viewVector = worldPos - _WorldSpaceCameraPos;
                float3 viewDir = normalize(viewVector);
                float3 reflection = reflect(viewDir, o.normal);
                o.reflection = reflection;
                return o;
            }

            #define SUN_DIRECTION float3(0, 0.980581, 0.196116)

            float getFluidPosition(v2f i)
            {
                #if CORNER
                float cornerAngle = atan2(i.localPos.z, i.localPos.x);
                cornerAngle /= UNITY_PI * 2;
                cornerAngle = (cornerAngle + 1) % 1;

                float segment1 = (i.localPos.z + 0.25) * 4;
                float segment2 = cornerAngle * 4;
                float segment3 = (i.localPos.x + 0.25) * 4;

                const float edge1 = 0.3;
                const float edge2 = 0.7;

                bool inSegment1 = segment1 < 1;
                bool inSegment3 = segment3 < 1;
                
                return inSegment1 ? lerp(0, edge1, segment1) // segment 1
                : inSegment3 ? lerp(edge2, 1, 1 - segment3) // segment 3
                : lerp(edge1, edge2, segment2); // segment 2
                #else
                return i.localPos.y + 0.5;
                #endif
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float sunD = dot(i.normal, SUN_DIRECTION);
                float lighting = sunD * 0.5 + 0.5;
                
                float specularD = dot(i.reflection, SUN_DIRECTION);
                
                lighting += pow(max(0.03, specularD * max(0, sunD)), 16) * 20;
                
                float fluidPosition = _FlipFluid ? getFluidPosition(i) : 1 - getFluidPosition(i);

                float waveAmount = smoothstep(0.5, 0.2, abs(_FluidProgress - 0.5));
                
                #if CORNER
                float waveAxis = i.localPos.y;
                #else
                float waveAxis = i.localPos.z;
                #endif
                float wave = sin(_Time.y * 3 + waveAxis * 20);
                float waveOffset = wave * 0.03;

                fluidPosition += waveOffset * waveAmount;
                
                float p = _FluidProgress * 1.01 - fluidPosition;
                float waterAmount = step(0.01, p);

                float3 col = lerp(_Color, _WaterColor, waterAmount);

                return float4(col * lighting, max(_Opacity, waterAmount));
            }
            ENDCG
        }
        Pass
        {
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                float pulse = sin(_Time.y * 3) * 0.5 + 0.5;
                float3 localPos = v.vertex + v.normal * lerp(0.01, 0.02, pulse);
                o.vertex = UnityObjectToClipPos(float4(localPos, v.vertex.w));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = _OutlineColor;
                float pulse = sin(_Time.y * 4) * 0.5 + 0.5;
                col.a *= pulse * 0.5 + 0.5;
                return col;
            }
            ENDCG
        }
    }
}