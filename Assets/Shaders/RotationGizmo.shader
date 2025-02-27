Shader "Unlit/RotationGizmo"
{
    Properties
    {
        _HandleRadius ("Handle Radius", Float) = 0.5
        _HandleWidth ("Handle Width", Float) = 0.1
        _HandleStart ("Handle Start", Float) = 0.9
        _AxisColor ("Axis Color", Color) = (1,0,0)
        [ToggleUI] _InUse ("In Use", Int) = 0
        _ClickPosition ("Click Position", Vector) = (1,1,0,0)
        _Opacity ("Opacity", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 centerUV : TEXCOORD0;
                float2 targetUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float _HandleRadius;
            float _HandleWidth;
            float _HandleStart;
            float3 _AxisColor;
            bool _InUse;
            float2 _ClickPosition;
            float _Opacity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 centerPos = mul(unity_ObjectToWorld, float4(0,0,0,1));

                float3 localCamPos = mul(unity_WorldToObject, _WorldSpaceCameraPos);

                float2 centerUV = v.uv * 2 - 1;
                float2 camPosUV = localCamPos.xz;

                float2 targetUV = _InUse ? _ClickPosition : camPosUV;

                o.centerUV = centerUV;
                o.targetUV = targetUV;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float alignment = dot(normalize(-i.centerUV), normalize(i.targetUV));
                float viewAlignment = smoothstep(_HandleStart, 1, alignment);

                float handleDist = abs(_HandleRadius - length(i.centerUV));
                float handleCircle = 1 - step(_HandleWidth + _InUse * _HandleWidth, handleDist);
                float handle = handleCircle * viewAlignment;

                float4 regularCol = float4(_AxisColor, handle * 0.8 * _Opacity);

                float4 inUseCol = lerp(float4(_AxisColor * 0.01, 0.8 * handleCircle), float4(_AxisColor, handleCircle), handle);

                return _InUse ? inUseCol : regularCol;
            }
            ENDCG
        }
    }
}
